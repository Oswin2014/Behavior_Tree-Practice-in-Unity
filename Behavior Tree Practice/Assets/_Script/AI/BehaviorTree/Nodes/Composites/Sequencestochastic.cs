
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class SequenceStochastic : CompositeStochastic
    {
        public SequenceStochastic()
        {
		}
        ~SequenceStochastic()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is SequenceStochastic))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            SequenceStochasticTask pTask = new SequenceStochasticTask();

            return pTask;
        }

        class SequenceStochasticTask : CompositeStochasticTask
        {
            public SequenceStochasticTask() : base()
            {
			}

            protected override void addChild(BehaviorTask pBehavior)
            {
                base.addChild(pBehavior);
            }

            protected override bool onenter(Agent pAgent)
            {
                base.onenter(pAgent);

                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
                base.onexit(pAgent, s);
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
						int childIndex = this.m_set[this.m_activeChildIndex];
						BehaviorTask pBehavior = this.m_children[childIndex];
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