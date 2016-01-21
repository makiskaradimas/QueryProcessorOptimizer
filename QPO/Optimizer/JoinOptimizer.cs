using System;
using System.Collections.Generic;

namespace QPO.Optimizer
{
    using QPO.QueryTree;
    using QPO.QueryTree.Condition;   

    class JoinOptimizer
    {
        public static void findNodes(List<Treenode> nodeList, List<Treenode> operandsList, Treenode node)
        {
            if (node != null)
                if (node.NodeType == Treenode.Type.join)
                {
                    nodeList.Add(node);
                    if (node.Operand1 != null)
                    {
                        if (node.Operand1.NodeType != Treenode.Type.join)
                            operandsList.Add(node.Operand1);
                        if (node.Operand2.NodeType != Treenode.Type.join)
                            operandsList.Add(node.Operand2);
                    }
                }

            if (node.Operand1 != null)
                findNodes(nodeList, operandsList, node.Operand1);
            if (node.Operand2 != null)
                findNodes(nodeList, operandsList, node.Operand2);
        }
        
        public static Treenode getOperand(Treenode node, List<Treenode> operands)
        {
            Treenode result = null;
            CompositeCondition compcond = node.Condition;
            AtomicCondition atomcond = compcond[0];
            ConditionElement condelem1 = atomcond.Attribute1;
            ConditionElement condelem2 = atomcond.Attribute2;

            foreach (Treenode fnode in operands)
            {
                Treenode initialNode = fnode;
                Treenode tempnode = initialNode;

                while (tempnode.NodeType != Treenode.Type.relation)
                    if (tempnode.Operand1 != null)
                        tempnode = tempnode.Operand1;

                if (tempnode.RelationName.Equals(condelem1.RelationName) || tempnode.RelationName.Equals(condelem2.RelationName))
                    result = initialNode;
            }

            if (result != null)
                operands.Remove(result);

            return result;
        }

        public static void TransformOrdering(Querytree tree, List<Treenode> nodelist, List<Treenode> operands)
        {
            Treenode lastnode = null;

            foreach (Treenode node in nodelist)
            {
                Treenode operand1 = JoinOptimizer.getOperand(node, operands);
                Treenode operand2 = JoinOptimizer.getOperand(node, operands);

                if (operand2 != null)
                {
                    node.Operand1 = operand1;
                    operand1.Parent = node;
                    node.Operand2 = operand2;
                    operand2.Parent = node;
                }
                else
                {
                    lastnode.Parent = node;
                    node.Operand1 = lastnode;
                    node.Operand2 = operand1;
                }
                lastnode = node;
            }
            Treenode checknode = tree.Root;

            while (checknode.Operand1.NodeType != Treenode.Type.join)
                checknode = checknode.Operand1;

            checknode.Operand1 = lastnode;
            lastnode.Parent = checknode;
        }
    }
}
