using System;
using System.Collections.Generic;

namespace QPO.QueryTree
{
    class ProjectionElement
    {
        public string RelationName { get; private set; }
        public string Attribute { get; private set; }

        public ProjectionElement() { }

        public ProjectionElement(string relName, string attr)
        {
            RelationName = relName;
            Attribute = attr;
        }

        public ProjectionElement(string reldotattr)
        {
            string[] s = reldotattr.Split('.');
            if (s.Length > 1)
            {
                RelationName = s[0];
                Attribute = s[1];
            }
            else
            {
                Console.WriteLine("Error: Projection Element without domain. Exiting...");
                Environment.Exit(-2);
            }
        }

        public override string ToString()
        {
            return RelationName + '.' + Attribute;
        }

    }
}
