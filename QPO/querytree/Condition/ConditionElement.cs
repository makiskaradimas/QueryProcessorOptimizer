using System;
using System.Collections.Generic;

namespace QPO.QueryTree.Condition
{
    class ConditionElement
    {
        public string RelationName { get; private set; }
        public string Attribute { get; private set; }
        bool hasRelationName;

        public ConditionElement(string attr)
        {
            string[] s = attr.Split('.');
            if (s.Length > 1)
            {
                RelationName = s[0];
                Attribute = s[1];
                hasRelationName = true;
            }
            else
            {
                Attribute = attr;
                hasRelationName = false;
            }
        }

        public override string ToString()
        {
            if (hasRelationName == true)
                return RelationName + '.' + Attribute;
            else return Attribute;
        }
    }
}
