using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace QPO.Optimizer
{
    using QPO.CatalogObjects;
    using QPO.QueryTree;
    
    class Evaluator
    {
        public static long totalCost
        {
            //[MethodImpl(MethodImplOptions.Synchronized)]
            get;
            //[MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public static void selectOperators(Querytree tree)
        {
            if (tree != null)
                selectOptimalOperator(tree.Root);
        }

        public static void selectOptimalOperator(Treenode node)
        {
            if (node.Operand1 != null)
                selectOptimalOperator(node.Operand1);
            if (node.Operand2 != null)
                selectOptimalOperator(node.Operand2);

            long minCost = long.MaxValue, tempCost = long.MaxValue;

            if (node.NodeType == Treenode.Type.sel)
            {
                Operations.SelectionApproach sapproach = Operations.SelectionApproach.LinearScan;

                tempCost = Operations.nodeIOCostEstimation(node, null, Operations.SelectionApproach.LinearScan, null);
                if (tempCost < minCost)
                {
                    minCost = tempCost;
                    node.IntData.selApproach = Operations.SelectionApproach.LinearScan;
                }

                tempCost = Operations.nodeIOCostEstimation(node, null, Operations.SelectionApproach.UsingIndex, null);
                if (tempCost < minCost)
                {
                    sapproach = Operations.SelectionApproach.UsingIndex;
                    minCost = tempCost;
                    node.IntData.selApproach = Operations.SelectionApproach.UsingIndex;
                }

                tempCost = Operations.nodeIOCostEstimation(node, null, Operations.SelectionApproach.BinarySearch, null);
                if (tempCost < minCost)
                {
                    sapproach = Operations.SelectionApproach.BinarySearch;
                    minCost = tempCost;
                    node.IntData.selApproach = Operations.SelectionApproach.BinarySearch;
                }

                Operations.nodeIOCostEstimation(node, null, sapproach, null);
                node.IntData.numberOfIOs = minCost;
            }
            else if (node.NodeType == Treenode.Type.join)
            {
                Operations.JoinApproach japproach = Operations.JoinApproach.NestedLoops;

                tempCost = Operations.nodeIOCostEstimation(node, Operations.JoinApproach.NestedLoops, null, null);
                if (tempCost < minCost)
                {
                    minCost = tempCost;
                    node.IntData.joinApproach = Operations.JoinApproach.NestedLoops;
                }

                tempCost = Operations.nodeIOCostEstimation(node, Operations.JoinApproach.HashBased, null, null);
                if (tempCost < minCost)
                {
                    japproach = Operations.JoinApproach.HashBased;
                    minCost = tempCost;
                    node.IntData.joinApproach = Operations.JoinApproach.HashBased;
                }

                tempCost = Operations.nodeIOCostEstimation(node, Operations.JoinApproach.SortMerge, null, null);
                if (tempCost < minCost)
                {
                    japproach = Operations.JoinApproach.SortMerge;
                    minCost = tempCost;
                    node.IntData.joinApproach = Operations.JoinApproach.SortMerge;
                }

                Operations.nodeIOCostEstimation(node, japproach, null, null);
                node.IntData.numberOfIOs = minCost;
            }
            else if (node.NodeType == Treenode.Type.proj)
            {
                Operations.ProjectionApproach papproach = Operations.ProjectionApproach.Hashing;

                tempCost = Operations.nodeIOCostEstimation(node, null, null, Operations.ProjectionApproach.Hashing);
                if (tempCost < minCost)
                {
                    minCost = tempCost;
                    node.IntData.prApproach = Operations.ProjectionApproach.Hashing;
                }

                tempCost = Operations.nodeIOCostEstimation(node, null, null, Operations.ProjectionApproach.Sorting);
                if (tempCost < minCost)
                {
                    papproach = Operations.ProjectionApproach.Sorting;
                    minCost = tempCost;
                    node.IntData.prApproach = Operations.ProjectionApproach.Sorting;
                }

                Operations.nodeIOCostEstimation(node, null, null, papproach);
                node.IntData.numberOfIOs = minCost;
            }
            else
                Operations.nodeIOCostEstimation(node, null, null, null);
        }
    }
}
