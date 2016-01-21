using System;
using System.Collections.Generic;

namespace QPO.QueryTree
{   
    class ProjectionCollection: List<ProjectionElement>
    {
        public ProjectionCollection() { }

        public ProjectionCollection(List<ProjectionElement> contents)
        {
            foreach (ProjectionElement content in contents)
                this.Add(content);
        }

        public ProjectionCollection(string str)
        {
            string[] projectionElements = str.Split(new string[] { " , " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in projectionElements)
                this.Add(new ProjectionElement(s));
        }

        public override string ToString()
        {
            string s = String.Empty;
            foreach (ProjectionElement element in this)
            {
                if (s.Equals(String.Empty))
                    s = element.ToString();
                else
                    s = s + " , " + element;
            }

            return s;
        }
    }
}
