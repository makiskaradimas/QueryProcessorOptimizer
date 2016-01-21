using System;
using System.Collections.Generic;

namespace QPO.Optimizer
{    using QPO.QueryTree.Condition;
    using QPO.QueryTree;
    
    class ProjectionOptimizer
    {
        public static void startPushDown(List<ConditionElement> elements, Querytree tree)
        {
            if (tree != null)
                pushDown(elements, tree.Root);
        }

        public static void pushDown(List<ConditionElement> elements, Treenode node)
        {
            if (node.NodeType == Treenode.Type.proj)
            {
                ProjectionCollection pcollection = node.ProjectionColl;
                foreach (ProjectionElement pelement in pcollection)
                    elements.Add(new ConditionElement(pelement.RelationName + '.' + pelement.Attribute));
            }
            else if (node.NodeType == Treenode.Type.join)
            {
                AtomicCondition atcond = node.Condition[0];
                elements.Add(atcond.Attribute1);
                elements.Add(atcond.Attribute2);
            }
            else if (node.NodeType == Treenode.Type.relation)
            {
                string relName = node.RelationName;
                ProjectionCollection pcollection = new ProjectionCollection();
                foreach (ConditionElement celement in elements)
                    if (relName.Equals(celement.RelationName))
                        pcollection.Add(new ProjectionElement(celement.RelationName, celement.Attribute));

                Treenode newproj = new Treenode(Treenode.Type.proj, node.Parent, node, null, null, pcollection);

                if (node.Parent.NodeType == Treenode.Type.sel)
                {
                    if (node.Parent.Parent.Operand1 == node.Parent)
                        node.Parent.Parent.Operand1 = newproj;
                    else if (node.Parent.Parent.Operand2 == node.Parent)
                        node.Parent.Parent.Operand2 = newproj;

                    node.Parent.Parent = newproj;
                    newproj.Operand1 = node.Parent;
                }
                else
                {
                    if (node.Parent.Operand1 == node)
                        node.Parent.Operand1 = newproj;
                    else if (node.Parent.Operand2 == node)
                        node.Parent.Operand2 = newproj;

                    node.Parent = newproj;
                    newproj.Operand1 = node;
                }
            }

            if (node.Operand1 != null)
                pushDown(elements, node.Operand1);
            if (node.Operand2 != null)
                pushDown(elements, node.Operand2);
        }
    }
}
