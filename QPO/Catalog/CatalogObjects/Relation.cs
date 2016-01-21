using System;
using System.Collections.Generic;

namespace QPO.CatalogObjects
{
    class Relation
    {
        public string Name { get; private set; }
        public long cardinality { get; private set; }
        public long TupleSize { get; private set; }
        public List<Attribute> Attributes { get; set; }
        public bool isSorted { get; private set; }
        public string SortedTo { get; private set; }

        public Relation(string name)
        {
            isSorted = false;
            Name = name;

            cardinality = long.Parse(Parameters.getElement("Relation." + Name + ".Cardinality"));
            TupleSize = long.Parse(Parameters.getElement("Relation." + Name + ".TupleSize"));

            string sortedTo = Parameters.getElement("Relation." + Name + ".SortedTo");
            if (sortedTo != null)
            {
                SortedTo = sortedTo;
                isSorted = true;
            }
        }
    }
}
