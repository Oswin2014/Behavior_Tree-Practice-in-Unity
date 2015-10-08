
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class Condition : ConditionBase
    {

        protected override BehaviorTask createTask()
        {
            ConditionTask pTask = new ConditionTask();

            return pTask;
        }

        public static Property LoadProperty(string value)
        {
            return null;
        }

        public static Property ParseProperty(string value, ref string typeName)
        {
            Property opr = null;
            return opr;
        }

        class ConditionTask : ConditionBaseTask
        {
            public ConditionTask()
            {
            }

            ~ConditionTask()
            {
            }
        }

    }
}