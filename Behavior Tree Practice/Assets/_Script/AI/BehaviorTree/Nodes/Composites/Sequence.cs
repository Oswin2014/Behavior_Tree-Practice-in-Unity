
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class Sequence : BehaviorNode
    {
        public Sequence()
        {
        }
        ~Sequence()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Sequence))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            return new SequenceTask();
        }

        public class SequenceTask : CompositeTask
        {
            public SequenceTask()
            {
            }
            ~SequenceTask()
            {
            }

            protected override bool onenter(Agent pAgent)
            {
                this.m_activeChildIndex = 0;

                return true;
            }
            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(this.m_activeChildIndex < this.m_children.Count);

                bool bFirst = true;

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

                    // If the child fails, or keeps running, do the same.
                    if (s != EBTStatus.BT_SUCCESS)
                    {
                        return s;
                    }

                    // Hit the end of the array, job done!
                    ++this.m_activeChildIndex;
                    if (this.m_activeChildIndex >= this.m_children.Count)
                    {
                        return EBTStatus.BT_SUCCESS;
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
