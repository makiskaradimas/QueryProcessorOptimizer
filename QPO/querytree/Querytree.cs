using System;
using System.Collections.Generic;

namespace QPO.QueryTree
{
    using QPO.Optimizer;
    
    class Querytree
    {
        public Treenode Root { get; set; }

        public Querytree()
        {
            Root = new Treenode();
        }

        public void PrintNodes(Treenode node)
        {
            if (node != null)
            {
                if (node.NodeType == Treenode.Type.relation)
                    Console.Write(node.RelationName);
                else if (node.NodeType == Treenode.Type.sel || node.NodeType == Treenode.Type.join)
                    Console.Write(node.NodeType.ToString() + '[' + node.Condition.ToString() + ']');
                else Console.Write(node.NodeType.ToString() + '[' + node.ProjectionColl.ToString() + ']');
            }

            if (node.Operand1 != null)
            {
                Console.Write('(');
                PrintNodes(node.Operand1);
                Console.Write(')');
            }
            if (node.Operand2 != null)
            {
                Console.Write('(');
                PrintNodes(node.Operand2);
                Console.Write(')');
            }
        }

        public void PrintCost(Treenode node)
        {
            if (node.NodeType == Treenode.Type.sel)
            {
                Console.WriteLine("\n" + node.NodeType.ToString() + '[' + node.Condition.ToString() + ']');
                Console.WriteLine(node.IntData.selApproach.ToString());
                Console.WriteLine("I/O Cost: " + node.IntData.numberOfIOs.ToString());
                Evaluator.totalCost += node.IntData.numberOfIOs;
            }
            else if (node.NodeType == Treenode.Type.proj)
            {
                Console.WriteLine("\n" + node.NodeType.ToString() + '[' + node.ProjectionColl.ToString() + ']');
                Console.WriteLine(node.IntData.prApproach.ToString());
                Console.WriteLine("I/O Cost: " + node.IntData.numberOfIOs.ToString());
                Evaluator.totalCost += node.IntData.numberOfIOs;
            }
            else if (node.NodeType == Treenode.Type.join)
            {
                Console.WriteLine("\n" + node.NodeType.ToString() + '[' + node.Condition.ToString() + ']');
                Console.WriteLine(node.IntData.joinApproach.ToString());
                Console.WriteLine("I/O Cost: " + node.IntData.numberOfIOs.ToString());
                Evaluator.totalCost += node.IntData.numberOfIOs;
            }

            if (node.Operand1 != null)
                PrintCost(node.Operand1);
            if (node.Operand2 != null)
                PrintCost(node.Operand2);
        }
    } 
}
