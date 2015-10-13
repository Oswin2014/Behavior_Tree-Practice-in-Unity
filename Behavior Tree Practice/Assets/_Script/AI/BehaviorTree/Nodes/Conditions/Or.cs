
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class Or : ConditionBase
    {
        public Or()
        {
		}
        ~Or()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Or))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            OrTask pTask = new OrTask();

            return pTask;
        }


        // ============================================================================
        class OrTask : Selector.SelectorTask
        {
            public OrTask()
                : base()
            {
            }

            ~OrTask()
            {
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                //Debug.Check(this.m_children.Count == 2);
                for(int i = 0; i < this.m_children.Count; ++i)
                {
                    BehaviorTask pBehavior = this.m_children[i];
                    EBTStatus s = pBehavior.exec(pAgent);

                    // If the child succeeds, succeeds
                    if (s == EBTStatus.BT_SUCCESS)
                    {
                        return s;
                    }

                    Debug.Check(s == EBTStatus.BT_FAILURE);
                }

                return EBTStatus.BT_FAILURE;
            }
        }
    }
}