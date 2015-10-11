
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class Selector : BehaviorNode
    {
        public Selector()
        {
		}
        ~Selector()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Selector))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            SelectorTask pTask = new SelectorTask();

            return pTask;
        }

        // ============================================================================
        public class SelectorTask : CompositeTask
        {
            public SelectorTask()
            {
			}
            ~SelectorTask()
            {
            }

            protected override bool onenter(Agent pAgent)
            {
                Debug.Check(this.m_children.Count > 0);
                this.m_activeChildIndex = 0;
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                bool bFirst = true;

                Debug.Check(this.m_activeChildIndex < this.m_children.Count);

                // Keep going until a child behavior says its running.
                for (; ; )
                {
                    EBTStatus s = childStatus;
                    if (!bFirst || s == EBTStatus.BT_RUNNING)
                    {
                        Debug.Check(this.m_status == EBTStatus.BT_INVALID ||
                            this.m_status == EBTStatus.BT_RUNNING);

                        BehaviorTask pBehavior = this.m_children[this.m_activeChildIndex];
                        s = pBehavior.exec(pAgent);
                    }

                    bFirst = false;

                    // If the child succeeds, or keeps running, do the same.
                    if (s != EBTStatus.BT_FAILURE)
                    {
                        return s;
                    }

                    // Hit the end of the array, job done!
                    ++this.m_activeChildIndex;
                    if (this.m_activeChildIndex >= this.m_children.Count)
                    {
                        return EBTStatus.BT_FAILURE;
                    }

					if (!this.CheckPredicates(pAgent))
					{
						return EBTStatus.BT_FAILURE;
					}
                }
            }
        }
    }
}