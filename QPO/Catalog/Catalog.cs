using System;
using System.Collections;
using System.Collections.Generic;


namespace QPO
{
    using CatalogObjects;

    class Catalog
    {
        public static Dictionary<String, Relation> Data { get; private set; }
  
        static Catalog()
        {
            Data = new Dictionary<String, Relation>();

            Relation[] relations = Parameters.getRelations();
            foreach (Relation rel in relations)
                Data.Add(rel.Name, rel);
        }
    }
}
