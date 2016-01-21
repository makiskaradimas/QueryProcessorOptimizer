using System;
using System.Collections.Generic;
using System.IO;

namespace QPO.CatalogObjects
{   
    class Parameters
    {
        static string file = "stats.txt";
        public static long AvailableBuffers { get; private set; }
        public static long BufferSize { get; private set; }

        public static String getElement(string element)
        {
            string str = null;
            try
            {
                using (var r = new StreamReader(file))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        if (!line.StartsWith("#") && line.StartsWith(element))
                        {
                            string[] s = line.Split('=');
                            str = s[1];
                            break;
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("IOException in config.txt: " + e);
                Environment.Exit(-1);
            }

            return str;
        }

        public static string[] getElementsStartingWith(string element)
        {
            List<String> lst = new List<String>();
            try
            {
                var r = new StreamReader(file);
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    if (!line.StartsWith("#") && line.StartsWith(element))
                    {
                        string[] s = line.Split('=');
                        lst.Add(s[1]);
                    }
                }
                r.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("IOException in stats.txt: " + e);
                Environment.Exit(-1);
            }

            return lst.ToArray();
        }

        public static Relation[] getRelations()
        {
            var relationList = new List<Relation>();
            string[] relationNames = Parameters.getElementsStartingWith("RelationName");

            foreach (String relationName in relationNames)
            {
                var newRelation = new Relation(relationName);
                newRelation.Attributes = Parameters.getRelationAttributes(relationName);
                relationList.Add(newRelation);
            }

            return relationList.ToArray();
        }

        public static List<Attribute> getRelationAttributes(string relationName)
        {
            List<Attribute> attributeList = new List<Attribute>();
            string[] attributeNames = Parameters.getElementsStartingWith(relationName + ".AttributeName");

            foreach (String attributeName in attributeNames)
            {
                Attribute newAttribute = new Attribute(relationName, attributeName);
                attributeList.Add(newAttribute);
            }

            return attributeList;
        }

        static Parameters()
        {
            AvailableBuffers = long.Parse(Parameters.getElement("QPO.AvailableBuffers"));
            BufferSize = long.Parse(Parameters.getElement("QPO.BufferSize"));
        }
    }
}
