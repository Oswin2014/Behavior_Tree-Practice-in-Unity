
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class And : ConditionBase
    {
        public And()
        {
		}
        ~And()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }
        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is And))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            AndTask pTask = new AndTask();

            return pTask;
        }
    }


    // ============================================================================
    class AndTask : Sequence.SequenceTask
    {
        public AndTask() : base()
        {
        }

        ~AndTask()
        {
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            //Debug.Check(this.m_children.Count == 2);

			for(int i = 0; i < this.m_children.Count; ++i)
            {
				BehaviorTask pBehavior = this.m_children[i];
                EBTStatus s = pBehavior.exec(pAgent);

                // If the child fails, fails
                if (s == EBTStatus.BT_FAILURE)
                {
                    return s;           	
                }

                Debug.Check(s == EBTStatus.BT_SUCCESS);
            }

            return EBTStatus.BT_SUCCESS;
        }
    }
}