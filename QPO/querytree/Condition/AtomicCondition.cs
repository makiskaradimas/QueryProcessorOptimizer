using System;
using System.Collections.Generic;

namespace QPO.QueryTree.Condition
{
    class AtomicCondition
    {
        public string ConditionOperator { get; private set; }
        public ConditionElement Attribute1 { get; private set; }
        public ConditionElement Attribute2 { get; private set; }

        public AtomicCondition(String oper, ConditionElement attr1, ConditionElement attr2)
        {
            if (oper == "=" || oper == "!=" || oper == "<" || oper == ">" || oper == ">=" || oper == "<=")
                ConditionOperator = oper;
            else Console.WriteLine("Invalid operator");

            Attribute1 = attr1;
            Attribute2 = attr2;
        }

        public AtomicCondition(string atomicConditionStr)
        {
            string[] conditionOperands;

            if (atomicConditionStr.Split(new string[] { "!=" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
            {
                ConditionOperator = "!=";
                conditionOperands = atomicConditionStr.Split(new string[] { "!=" }, StringSplitOptions.RemoveEmptyEntries);
                Attribute1 = new ConditionElement(conditionOperands[0]);
                Attribute2 = new ConditionElement(conditionOperands[1]);
            }
            else if (atomicConditionStr.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
            {
                ConditionOperator = "=";
                conditionOperands = atomicConditionStr.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                Attribute1 = new ConditionElement(conditionOperands[0]);
                Attribute2 = new ConditionElement(conditionOperands[1]);
            }
            else if (atomicConditionStr.Split(new string[] { ">" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
            {
                ConditionOperator = ">";
                conditionOperands = atomicConditionStr.Split(new string[] { ">" }, StringSplitOptions.RemoveEmptyEntries);
                Attribute1 = new ConditionElement(conditionOperands[0]);
                Attribute2 = new ConditionElement(conditionOperands[1]);
            }
            else if (atomicConditionStr.Split(new string[] { "<" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
            {
                ConditionOperator = "<";
                conditionOperands = atomicConditionStr.Split(new string[] { "<" }, StringSplitOptions.RemoveEmptyEntries);
                Attribute1 = new ConditionElement(conditionOperands[0]);
                Attribute2 = new ConditionElement(conditionOperands[1]);
            }
            else if (atomicConditionStr.Split(new string[] { ">=" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
            {
                ConditionOperator = ">=";
                conditionOperands = atomicConditionStr.Split(new string[] { ">=" }, StringSplitOptions.RemoveEmptyEntries);
                Attribute1 = new ConditionElement(conditionOperands[0]);
                Attribute2 = new ConditionElement(conditionOperands[1]);
            }
            if (atomicConditionStr.Split(new string[] { "<=" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
            {
                ConditionOperator = "<=";
                conditionOperands = atomicConditionStr.Split(new string[] { "<=" }, StringSplitOptions.RemoveEmptyEntries);
                Attribute1 = new ConditionElement(conditionOperands[0]);
                Attribute2 = new ConditionElement(conditionOperands[1]);
            }
        }

        public override string ToString()
        {
            return Attribute1 + ConditionOperator + Attribute2;
        }
    }
}
