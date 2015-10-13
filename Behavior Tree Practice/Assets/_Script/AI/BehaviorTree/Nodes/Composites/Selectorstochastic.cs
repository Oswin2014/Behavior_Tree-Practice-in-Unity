
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    /**
    the Selector runs the children from the first sequentially until the child which returns success.
    for SelectorStochastic, the children are not sequentially selected, instead it is selected stochasticly.

    for example: the children might be [0, 1, 2, 3, 4]
    Selector always select the child by the order of 0, 1, 2, 3, 4
    while SelectorStochastic, sometime, it is [4, 2, 0, 1, 3], sometime, it is [2, 3, 0, 4, 1], etc.
    */
    public class SelectorStochastic : CompositeStochastic
    {
        public SelectorStochastic()
        {
		}
        ~SelectorStochastic()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is SelectorStochastic))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            SelectorStochasticTask pTask = new SelectorStochasticTask();

            return pTask;
        }

        // ============================================================================
        class SelectorStochasticTask : CompositeStochasticTask
        {
            public SelectorStochasticTask() : base()
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
                bool bFirst = true;

                Debug.Check(this.m_activeChildIndex < this.m_children.Count);

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
                }
            }
        }
    }
}