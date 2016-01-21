using System;
using System.Collections.Generic;

namespace QPO
{
    using QueryTree;
    using QueryTree.Condition;

    class Parser
    {
        public static Querytree parseQuery(string query)
        {
            Querytree tree = new Querytree();
            string[] operations = query.Split('(');
            string first = operations[0];

            if (first.StartsWith("sel") || first.StartsWith("proj") || first.StartsWith("join"))
            {
                Treenode node = parse(0, null, first.Replace(")", String.Empty), operations, 1);
                tree.Root = node;
            }
            else
            {
                Console.WriteLine("Invalid query");
                Environment.Exit(1);
            }

            return tree;
        }

        public static Treenode parse(int index, Treenode parent, string query, string[] elements, int operand)
        {
            Treenode node = new Treenode();
            string[] array1 = query.Split('[');

            if (array1[0].Equals("sel"))
            {
                node.NodeType = Treenode.Type.sel;
                string[] array2 = array1[1].Split(']');
                node.Condition = new CompositeCondition(array2[0]);
                node.Parent = parent;

                if (parent != null)
                {
                    if (operand == 1)
                        parent.Operand1 = node;
                    else parent.Operand2 = node;
                }

                parent = node;
                parse(index + 1, parent, elements[index + 1], elements, 1);
            }
            else if (array1[0].Equals("proj"))
            {
                node.NodeType = Treenode.Type.proj;
                string[] array2 = array1[1].Split(']');
                node.ProjectionColl = new ProjectionCollection(array2[0]);
                node.Parent = parent;

                if (parent != null)
                {
                    if (operand == 1)
                        parent.Operand1 = node;
                    else parent.Operand2 = node;
                }

                parent = node;
                parse(index + 1, parent, elements[index + 1], elements, 1);
            }
            else if (array1[0].Equals("join"))
            {
                node.NodeType = Treenode.Type.join;
                string[] array2 = array1[1].Split(']');
                node.Condition = new CompositeCondition(array2[0]);
                node.Parent = parent;

                if (parent != null)
                {
                    if (operand == 1)
                        parent.Operand1 = node;
                    else parent.Operand2 = node;
                }

                int Operand2index = -1;
                int rightbrackets = 0;
                int leftbrackets = 0;

                for (int i = index; i < elements.Length; i++)
                {
                    rightbrackets += elements[i].Length - elements[i].ToLower().Replace(")", String.Empty).Length;
                    leftbrackets++;

                    if (rightbrackets >= leftbrackets && rightbrackets > 0)
                    {
                        Operand2index = i;
                        break;
                    }
                    else if (rightbrackets == leftbrackets - 1 && rightbrackets > 0)
                    {
                        Operand2index = i + 1;
                        break;
                    }
                }

                parent = node;
                parse(index + 1, parent, elements[index + 1], elements, 1);
                parse(Operand2index, parent, elements[Operand2index], elements, 2);
            }
            else
            {
                node.NodeType = Treenode.Type.relation;
                node.RelationName = query.Replace(")", String.Empty);
                node.Parent = parent;
                
                if (parent != null)
                {
                    if (operand == 1)
                        parent.Operand1 = node;
                    else
                        parent.Operand2 = node;
                }
            }
            return node;
        }
    }
}

