using System;
using System.Collections.Generic;
using System.Linq;

namespace QPO.QueryTree.Condition
{
    class CompositeCondition: List<AtomicCondition>
    {
        public enum Type
        {
            simple,
            conjunctions,
            disjunctions
        };
        public Type ConditionType { get; private set; }

        public CompositeCondition(string str)
        {
            string[] elements;

            if (str.Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
            {
                ConditionType = Type.conjunctions;
                elements = str.Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string element in elements)
                    this.Add(new AtomicCondition(element));
            }
            else if (str.Split(new string[] { " or " }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
            {
                ConditionType = Type.disjunctions;
                elements = str.Split(new string[] { " or " }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string element in elements)
                    this.Add(new AtomicCondition(element));
            }
            else
            {
                ConditionType = Type.simple;
                this.Add(new AtomicCondition(str));
            }
        }

        public override string ToString()
        {
            string s = String.Empty;
            if (ConditionType == Type.conjunctions)
            {
                foreach (AtomicCondition atom in this)
                {
                    if (s.Equals(String.Empty))
                        s = atom.ToString();
                    else
                        s = s + " and " + atom;
                }
                return s;
            }
            else if (ConditionType == Type.disjunctions)
            {
                foreach (AtomicCondition atom in this)
                {
                    if (s.Equals(String.Empty))
                        s = atom.ToString();
                    else
                        s = s + " or " + atom;
                }
                return s;
            }
            else
            {
                return this.First().ToString();
            }
        }
    }
}
