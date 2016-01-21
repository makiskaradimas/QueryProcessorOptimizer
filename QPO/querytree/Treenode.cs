using System;
using System.Collections.Generic;

namespace QPO.QueryTree
{
    using QPO.CatalogObjects;
    using Condition;

    class Treenode
    {
        public enum Type
        {
            relation,
            proj,
            sel,
            join
        };

        public Type NodeType { get; set; }
        public string RelationName { get; set; }
        public Treenode Parent { get; set; }
        public Treenode Operand1 { get; set; }
        public Treenode Operand2 { get; set; }
        public CompositeCondition Condition { get; set; }
        public ProjectionCollection ProjectionColl { get; set; }
        public IntermediateData IntData { get; private set;}

        public Treenode()
        {
           IntData = new IntermediateData(0, 0, 0, 0);
        }

        public Treenode(Type nodetype, Treenode father, Treenode oper1, Treenode oper2, CompositeCondition cCondition, ProjectionCollection projCollection)
        {
            NodeType = nodetype;
            Parent = father;
            Operand1 = oper1;
            Operand2 = oper2;
            Condition = cCondition;
            ProjectionColl = projCollection;
            IntData = new IntermediateData(0, 0, 0, 0);
        }

        public void produceIntData()
        {
            if (NodeType == Treenode.Type.proj)
            {
                long result = 0;
                long resultSize = 0;

                foreach (ProjectionElement pelement in ProjectionColl)
                {
                    if (result == 0)
                        result = 1;

                    Relation temprel = Catalog.Data[pelement.RelationName];
                    List<Attribute> attrList = temprel.Attributes;

                    foreach (Attribute attr in attrList)
                    {
                        if (attr.AttributeName.Equals(pelement.Attribute))
                        {  
                            result *= attr.UniqueValues;
                            if (attr.AttrType == Attribute.AttrTypes.integer)
                                resultSize += 4;
                            else if (attr.AttrType == Attribute.AttrTypes.varchar)
                                resultSize += 10; // varchar assumed to be 10 bytes
                            break;
                        }
                        
                    }
                    IntData.Tuples = result;
                    IntData.TupleSize = resultSize;
                    IntData.TotalSize = resultSize * result;
                }
            }
            else if (NodeType == Treenode.Type.sel)
            {
                if (Condition[0].ConditionOperator.Equals("="))
                {
                    string relName = Condition[0].Attribute1.RelationName;
                    string attrName = Condition[0].Attribute1.Attribute;
                    Relation temprel = Catalog.Data[relName];
                    List<Attribute> attributes = temprel.Attributes;

                    foreach (Attribute attr in attributes)
                        if (attr.AttributeName.Equals(attrName))
                        {
                            IntData.Tuples = Operand1.IntData.Tuples / attr.UniqueValues;
                            IntData.TupleSize = Operand1.IntData.TupleSize;
                            IntData.TotalSize = IntData.Tuples * IntData.TupleSize;
                            break;
                        }
                }
                else if (Condition[0].ConditionOperator.Equals(">"))
                {
                    string relName = Condition[0].Attribute1.RelationName;
                    string attrName = Condition[0].Attribute1.Attribute;
                    Relation temprel = Catalog.Data[relName];
                    List<Attribute> attributes = temprel.Attributes;

                    foreach (Attribute attr in attributes)
                        if (attr.AttributeName.Equals(attrName))
                        {
                            IntData.Tuples = (Operand1.IntData.Tuples * (attr.MaxValue - Int32.Parse(Condition[0].Attribute2.Attribute)))  / (attr.MaxValue - attr.MinValue);
                            IntData.TupleSize = Operand1.IntData.TupleSize;
                            IntData.TotalSize = IntData.Tuples * IntData.TupleSize;
                            break;
                        }
                }
                else if (Condition[0].ConditionOperator.Equals(">="))
                {
                    string relName = Condition[0].Attribute1.RelationName;
                    string attrName = Condition[0].Attribute1.Attribute;
                    Relation temprel = Catalog.Data[relName];
                    List<Attribute> attributes = temprel.Attributes;

                    foreach (Attribute attr in attributes)
                        if (attr.AttributeName.Equals(attrName))
                        {
                            IntData.Tuples = (Operand1.IntData.Tuples * (attr.MaxValue - Int32.Parse(Condition[0].Attribute2.Attribute))) / (attr.MaxValue - attr.MinValue) + 1;
                            IntData.TupleSize = Operand1.IntData.TupleSize;
                            IntData.TotalSize = IntData.Tuples * IntData.TupleSize;
                            break;
                        }
                }
                else if (Condition[0].ConditionOperator.Equals("<"))
                {
                    string relName = Condition[0].Attribute1.RelationName;
                    string attrName = Condition[0].Attribute1.Attribute;
                    Relation temprel = Catalog.Data[relName];
                    List<Attribute> attributes = temprel.Attributes;

                    foreach (Attribute attr in attributes)
                        if (attr.AttributeName.Equals(attrName))
                        {
                            IntData.Tuples = (Operand1.IntData.Tuples * (Int32.Parse(Condition[0].Attribute2.Attribute) - attr.MinValue)) / (attr.MaxValue - attr.MinValue);
                            IntData.TupleSize = Operand1.IntData.TupleSize;
                            IntData.TotalSize = IntData.Tuples * IntData.TupleSize;
                            break;
                        }
                }
                else if (Condition[0].ConditionOperator.Equals("<="))
                {
                    string relName = Condition[0].Attribute1.RelationName;
                    string attrName = Condition[0].Attribute1.Attribute;
                    Relation temprel = Catalog.Data[relName];
                    List<Attribute> attributes = temprel.Attributes;

                    foreach (Attribute attr in attributes)
                        if (attr.AttributeName.Equals(attrName))
                        {
                            IntData.Tuples = (Operand1.IntData.Tuples * (Int32.Parse(Condition[0].Attribute2.Attribute) - attr.MinValue)) / (attr.MaxValue - attr.MinValue) + 1;
                            IntData.TupleSize = Operand1.IntData.TupleSize;
                            IntData.TotalSize = IntData.Tuples * IntData.TupleSize;
                            break;
                        }
                }
                else if (Condition[0].ConditionOperator.Equals("!="))
                {
                    string relName = Condition[0].Attribute1.RelationName;
                    string attrName = Condition[0].Attribute1.Attribute;
                    Relation temprel = Catalog.Data[relName];
                    List<Attribute> attributes = temprel.Attributes;

                    foreach (Attribute attr in attributes)
                        if (attr.AttributeName.Equals(attrName))
                        {
                            IntData.Tuples = Operand1.IntData.Tuples - (Operand1.IntData.Tuples / attr.UniqueValues);
                            IntData.TupleSize = Operand1.IntData.TupleSize;
                            IntData.TotalSize = IntData.Tuples * IntData.TupleSize;
                            break;
                        }
                }
            }
            else if (NodeType == Treenode.Type.join)
            {
                ConditionElement celem1 = Condition[0].Attribute1;
                ConditionElement celem2 = Condition[0].Attribute2;

                Relation rel1 = Catalog.Data[celem1.RelationName];
                Relation rel2 = Catalog.Data[celem2.RelationName];

                List<Attribute> attrlist1 = rel1.Attributes;
                List<Attribute> attrlist2 = rel2.Attributes;
                bool haveCommon = false;
                foreach (Attribute attr1 in rel1.Attributes)
                {
                    foreach (Attribute attr2 in rel2.Attributes)
                    {
                        if (attr1.AttributeName.Equals(attr2.AttributeName))
                        {
                            haveCommon = true;
                            if (attr1.AttrUse == Attribute.AttrUses.primaryKey || attr2.AttrUse == Attribute.AttrUses.primaryKey)
                                IntData.Tuples = rel2.cardinality;
                            else
                            {
                                long num1 = (rel1.cardinality * rel2.cardinality) / attr1.UniqueValues;
                                long num2 = (rel1.cardinality * rel2.cardinality) / attr2.UniqueValues;

                                if (num1 < num2)
                                {
                                    IntData.Tuples = num1;
                                }
                                else IntData.Tuples = num2;
                            }
                            IntData.TupleSize = Operand1.IntData.TupleSize + Operand2.IntData.TupleSize;
                            IntData.TotalSize = IntData.Tuples * IntData.TupleSize;
                            break;
                        }
                    }
                }
                if (haveCommon == false)
                {
                    IntData.Tuples = Operand1.IntData.Tuples * Operand2.IntData.Tuples;
                    IntData.TupleSize = Operand1.IntData.TupleSize + Operand2.IntData.TupleSize;
                    IntData.TotalSize = IntData.Tuples * IntData.TupleSize;
                }
            }
            else if (NodeType == Treenode.Type.relation)
            {
                var temprel = Catalog.Data[RelationName];
                IntData.Tuples = temprel.cardinality;
                IntData.TupleSize = temprel.TupleSize;
                IntData.TotalSize = IntData.Tuples * IntData.TupleSize;
            }
        }

        public static int smallerNode(Treenode node1, Treenode node2)
        {

            node1.produceIntData();
            node2.produceIntData();
            if (node1.IntData.TotalSize < node2.IntData.TotalSize)
                return -1;
            else if (node1.IntData.TotalSize > node2.IntData.TotalSize)
                return 1;
            else return 0;
        }
    }
}
