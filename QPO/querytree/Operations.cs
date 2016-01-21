using System;
using System.Collections.Generic;

namespace QPO.QueryTree
{
    using QPO.CatalogObjects;

    class Operations
    {
        public enum SelectionApproach
        {
            LinearScan,
            BinarySearch,
            UsingIndex
        };


        public enum ProjectionApproach
        {
            Sorting,
            Hashing
        };

        public enum JoinApproach
        {
            SortMerge,
            HashBased,
            NestedLoops
        };

        static long logBaseCeil(long num, long lBase)
        {
            return (long)Math.Ceiling(Math.Log(num) / Math.Log(lBase));
        }

        public static long nodeIOCostEstimation(Treenode node, JoinApproach? japproach, SelectionApproach? sapproach, ProjectionApproach? papproach)
        {
            if (node.NodeType == Treenode.Type.join)
            {
                if (japproach == JoinApproach.NestedLoops)
                {
                    string relName1 = node.Condition[0].Attribute1.RelationName;
                    Relation rel1 = Catalog.Data[relName1];                                
                    long tuplesPerPage1 = Parameters.BufferSize / node.Operand1.IntData.TupleSize;
                    long pageBuffers1 = node.Operand1.IntData.Tuples / tuplesPerPage1;
                    long tuplesPerPage2 = Parameters.BufferSize / node.Operand2.IntData.TupleSize;
                    long pageBuffers2 = node.Operand2.IntData.Tuples / tuplesPerPage2;

                    node.produceIntData();

                    return (long)Decimal.Ceiling(pageBuffers1 / (Parameters.AvailableBuffers - 2)) * pageBuffers2 + pageBuffers1;
                }
                else if (japproach == JoinApproach.SortMerge)
                {
                    string relName1 = node.Condition[0].Attribute1.RelationName;
                    Relation rel1 = Catalog.Data[relName1];
                    string relName2 = node.Condition[0].Attribute2.RelationName;
                    Relation rel2 = Catalog.Data[relName2];
                    long tuplesPerPage1 = Parameters.BufferSize / node.Operand1.IntData.TupleSize;
                    long pageBuffers1 = node.Operand1.IntData.Tuples / tuplesPerPage1;
                    long tuplesPerPage2 = Parameters.BufferSize / node.Operand2.IntData.TupleSize;
                    long pageBuffers2 = node.Operand2.IntData.Tuples / tuplesPerPage2;
                    List<Attribute> attrlist1 = rel1.Attributes;
                    List<Attribute> attrlist2 = rel1.Attributes;
                    string attrName1 = node.Condition[0].Attribute1.Attribute;
                    string attrName2 = node.Condition[0].Attribute2.Attribute;

                    node.produceIntData();

                    foreach (Attribute attr1 in attrlist1)
                        foreach (Attribute attr2 in attrlist2)
                            if (attr1.AttributeName.Equals(attrName1) && attr2.AttributeName.Equals(attrName2))
                                if (rel1.isSorted && rel2.isSorted && attr1.AttributeName.Equals(rel1.SortedTo) && attr2.AttributeName.Equals(rel2.SortedTo))                                  
                                    return pageBuffers1 + pageBuffers2;
                                else if (rel2.isSorted && attr2.AttributeName.Equals(rel2.SortedTo))
                                    return pageBuffers2 + pageBuffers1 * (2 * logBaseCeil(pageBuffers1, Parameters.AvailableBuffers) + 2);
                                else if (rel1.isSorted && attr1.AttributeName.Equals(rel1.SortedTo))
                                    return pageBuffers1 + pageBuffers2 * (2 * logBaseCeil(pageBuffers2, Parameters.AvailableBuffers) + 2);
                                else
                                    return pageBuffers1 * (2 * logBaseCeil(pageBuffers1, Parameters.AvailableBuffers) + 2) +
                                           pageBuffers2 * (2 * logBaseCeil(pageBuffers2, Parameters.AvailableBuffers) + 2);
                }
                else if (japproach == JoinApproach.HashBased)
                {
                    node.produceIntData();
                    
                    return 3 * (node.Operand1.IntData.TupleSize + node.Operand2.IntData.TupleSize);
                }
            }
            else if (node.NodeType == Treenode.Type.sel)
            {
                if (sapproach == SelectionApproach.BinarySearch)
                {
                    long tuplesPerPage1 = Parameters.BufferSize / node.Operand1.IntData.TupleSize;
                    long pageBuffers1 = node.Operand1.IntData.Tuples / tuplesPerPage1;
                    string relName1 = node.Condition[0].Attribute1.RelationName;
                    Relation rel1 = Catalog.Data[relName1];
                    List<Attribute> attrlist1 = rel1.Attributes;
                    string attrName1 = node.Condition[0].Attribute1.Attribute;

                    node.produceIntData();

                    foreach (Attribute attr in attrlist1)
                        if (attr.AttributeName.Equals(attrName1))
                            if (rel1.isSorted && attr.AttributeName.Equals(rel1.SortedTo))
                                return (long)logBaseCeil(pageBuffers1, 2);
                    return long.MaxValue;
                }
                else if (sapproach == SelectionApproach.LinearScan)
                {
                    long tuplesPerPage1 = Parameters.BufferSize / node.Operand1.IntData.TupleSize;
                    long pageBuffers1 = node.Operand1.IntData.Tuples / tuplesPerPage1;
                    string relName1 = node.Condition[0].Attribute1.RelationName;
                    Relation rel1 = Catalog.Data[relName1];
                    List<Attribute> attrlist1 = rel1.Attributes;
                    string attrName1 = node.Condition[0].Attribute1.Attribute;

                    node.produceIntData();

                    foreach (Attribute attr in attrlist1)
                        if (attr.AttributeName.Equals(attrName1) && attr.AttrUse == Attribute.AttrUses.primaryKey)
                            return (long)Decimal.Ceiling(pageBuffers1 / 2);
                    return pageBuffers1;
                }
                else if (sapproach == SelectionApproach.UsingIndex)
                {
                    long tuplesPerPage1 = Parameters.BufferSize / node.Operand1.IntData.TupleSize;
                    long pageBuffers1 = node.Operand1.IntData.Tuples / tuplesPerPage1;
                    string relName1 = node.Condition[0].Attribute1.RelationName;
                    Relation rel1 = Catalog.Data[relName1];
                    List<Attribute> attrlist1 = rel1.Attributes;
                    string attrName1 = node.Condition[0].Attribute1.Attribute;

                    node.produceIntData();

                    foreach (Attribute attr in attrlist1)
                        if (attr.AttributeName.Equals(attrName1) && attr.IsIndex)
                            if (attr.IndexType == Attribute.IndexTypes.bPlusTree)
                                return attr.BplusTreeLength;
                            else if (attr.IndexType == Attribute.IndexTypes.extendibleHashing)
                                return 1;
                            else if (attr.IndexType == Attribute.IndexTypes.staticHashing)
                                return attr.BplusTreeLength + 1;
                    return long.MaxValue;
                }

            }
            else if (node.NodeType == Treenode.Type.proj)
            {
                if (papproach == ProjectionApproach.Sorting)
                {
                    long tuplesPerPage1 = Parameters.BufferSize / node.Operand1.IntData.TupleSize;
                    long pageBuffers1 = node.Operand1.IntData.Tuples / tuplesPerPage1;

                    node.produceIntData();
                    return pageBuffers1 * (2 * logBaseCeil(pageBuffers1, Parameters.AvailableBuffers) + 1);
                }
                else if (papproach == ProjectionApproach.Hashing)
                {
                    long tuplesPerPage1 = Parameters.BufferSize / node.Operand1.IntData.TupleSize;
                    long pageBuffers1 = node.Operand1.IntData.Tuples / tuplesPerPage1;

                    node.produceIntData();
                    return 4 * pageBuffers1;
                }
            }
            else if (node.NodeType == Treenode.Type.relation)
            {
                string relName1 = node.RelationName;
                Relation rel1 = Catalog.Data[relName1];
                node.IntData.TupleSize = rel1.TupleSize;
                node.IntData.Tuples = rel1.cardinality;
                node.IntData.TotalSize = rel1.cardinality * rel1.TupleSize;

                node.produceIntData();
            }
            return -1;
        }
    }
}
