using System;
using System.Collections.Generic;

namespace QPO.CatalogObjects
{
    class Attribute
    {
        public bool IsIndex { get; private set; }
        public string AttributeName { get; private set; }
        public IndexTypes IndexType { get; private set; }
        public AttrUses AttrUse { get; private set; }
        public AttrTypes AttrType { get; private set; }
        public long UniqueValues { get; private set; }
        public long MaxValue { get; set; }
        public long MinValue { get; set; }
        public int BplusTreeLength { get; private set; }

        public enum IndexTypes
        {
            bPlusTree,
            staticHashing,
            extendibleHashing
        };

        public enum AttrUses
        {
            simple,
            primaryKey,
            secondaryKey,
            foreignKey
        };

        public enum AttrTypes
        {
            integer,
            varchar
        };

        public Attribute(string relation, string name)
        {

            MaxValue = long.MaxValue;
            IsIndex = false;
            MinValue = long.MinValue;
            AttributeName = name;

            string IndexTypeStr = Parameters.getElement(relation + "." + name + ".IndexType");
            if (IndexTypeStr != null)
            {
                IndexType = (IndexTypes)Enum.Parse(typeof(IndexTypes), IndexTypeStr);
                IsIndex = true;
            }

            string AttrUseStr = Parameters.getElement(relation + "." + name + ".AttributeUse");
            if (AttrUseStr != null)
                AttrUse = (AttrUses)Enum.Parse(typeof(AttrUses), AttrUseStr);

            string AttrTypeStr = Parameters.getElement(relation + "." + name + ".AttributeType");
            if (AttrTypeStr != null)
                AttrType = (AttrTypes)Enum.Parse(typeof(AttrTypes), AttrTypeStr);

            string UniqueValuesStr = Parameters.getElement(relation + "." + name + ".NumOfUniqueValues");
            UniqueValues = String.IsNullOrEmpty(UniqueValuesStr) ? 0 : long.Parse(UniqueValuesStr);

            string MinValueStr = Parameters.getElement(relation + "." + name + ".MinValue");
            if (String.IsNullOrEmpty(MinValueStr) == false)
                MinValue = long.Parse(MinValueStr);

            string MaxValueStr = Parameters.getElement(relation + "." + name + ".MaxValue");
            if (String.IsNullOrEmpty(MaxValueStr) == false)
                MaxValue = long.Parse(MaxValueStr);

            BplusTreeLength = Convert.ToInt32(Parameters.getElement(relation + "." + name + ".BplusTreeLength"));
        }
    }
}
