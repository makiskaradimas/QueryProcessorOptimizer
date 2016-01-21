using System;
using System.Collections.Generic;
using System.Linq;

namespace QPO.Optimizer
{
    using QPO.QueryTree;
    using QPO.QueryTree.Condition;

    class SelectionOptimizer
    {
        public static void conjuctiveSelectionsSeparation(Querytree tree)
        {
            if (tree != null)
                conjuctiveSelectionSeparation(tree.Root, null);
        }

        public static void conjuctiveSelectionSeparation(Treenode node, Treenode previous)
        {
            if (node != null)
            {
                if (node.NodeType == Treenode.Type.sel && node.Condition.ConditionType == CompositeCondition.Type.conjunctions)
                {
                    Treenode next = node.Operand1;
                    CompositeCondition condition = node.Condition;

                    int i = 0;
                    foreach (AtomicCondition atcond in condition)
                    {
                        i++;
                        Treenode newnode = new Treenode(Treenode.Type.sel, previous, null, null, new CompositeCondition(atcond.ToString()), null);

                        if (previous != null)
                        {
                            if (previous.NodeType == Treenode.Type.sel || previous.NodeType == Treenode.Type.proj)
                                previous.Operand1 = newnode;
                            else if (previous.NodeType == Treenode.Type.join)
                            {
                                if (previous.Operand1 == node)
                                    previous.Operand1 = newnode;
                                else if (previous.Operand2 == node)
                                    previous.Operand2 = newnode;
                            }
                        }
                        previous = newnode;

                        if (i == condition.Count())
                        {
                            newnode.Operand1 = next;

                            if (next != null)
                                next.Parent = newnode;
                        }
                    }
                }

                if (node.Operand1 != null)
                    conjuctiveSelectionSeparation(node.Operand1, node);
                if (node.Operand2 != null)
                    conjuctiveSelectionSeparation(node.Operand1, node);
            }
        }

        public static void selectionPushDowns(Querytree tree)
        {
            if (tree != null)
            {
                List<Treenode> nodes = new List<Treenode>();
                findSelectNodes(nodes, tree.Root);

                foreach (Treenode node in nodes)
                    selectionPushDown(node, node, node.Parent, node.Operand1);
            }
        }

        public static void selectionPushDown(Treenode node, Treenode pushnode, Treenode previous, Treenode next)
        {
            if (node != null)
            {
                if (node.NodeType == Treenode.Type.relation)
                {
                    AtomicCondition atcond = pushnode.Condition[0];

                    if (atcond.Attribute1.RelationName.Equals(node.RelationName))
                    {
                        Treenode tempprevious = node.Parent;
                        pushnode.Parent = tempprevious;
                        node.Parent = pushnode;
                        pushnode.Operand1 = node;
                        next.Parent = previous;

                        if (previous != null)
                        {
                            if (previous.Operand1 == pushnode)
                                previous.Operand1 = next;
                            else if (previous.Operand2 == pushnode)
                                previous.Operand2 = next;
                        }

                        if (tempprevious != null)
                        {
                            if (tempprevious.Operand1 == node)
                                tempprevious.Operand1 = pushnode;
                            else if (tempprevious.Operand2 == node)
                                tempprevious.Operand2 = pushnode;
                        }
                    }
                }
                else
                {
                    if (node.Operand1 != null)
                        selectionPushDown(node.Operand1, pushnode, previous, next);
                    if (node.Operand2 != null)
                        selectionPushDown(node.Operand2, pushnode, previous, next);
                }
            }
        }

        public static void findSelectNodes(List<Treenode> nodes, Treenode node)
        {
            if (node != null)
            {
                if (node.NodeType == Treenode.Type.sel)
                    nodes.Add(node);

                if (node.Operand1 != null)
                    findSelectNodes(nodes, node.Operand1);
                if (node.Operand2 != null)
                    findSelectNodes(nodes, node.Operand2);
            }
        }
    }
}

