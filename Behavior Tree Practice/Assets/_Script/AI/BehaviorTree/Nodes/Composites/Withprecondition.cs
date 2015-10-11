
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class WithPrecondition : BehaviorNode
    {
        public WithPrecondition()
        {
		}
        ~WithPrecondition()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is WithPrecondition))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            WithPreconditionTask pTask = new WithPreconditionTask();


            return pTask;
        }
    }

    class WithPreconditionTask : Sequence.SequenceTask
    {
        public WithPreconditionTask() : base()
        {
		}

        protected override void addChild(BehaviorTask pBehavior)
        {
            base.addChild(pBehavior);
        }

        protected override bool onenter(Agent pAgent)
        {
            BehaviorTask pParent = this.GetParent();

            //when as child of SelctorLoop, it is not ticked normally
            Debug.Check(pParent is SelectorLoop.SelectorLoopTask);

            return true;
        }

        protected override void onexit(Agent pAgent, EBTStatus s)
        {
            BehaviorTask pParent = this.GetParent();

            Debug.Check(pParent is SelectorLoop.SelectorLoopTask);
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            BehaviorTask pParent = this.GetParent();
            Debug.Check(pParent is SelectorLoop.SelectorLoopTask);

            return EBTStatus.BT_RUNNING;
        }

        public BehaviorTask PreconditionNode()
        {
            Debug.Check(this.m_children.Count == 2);

            return (this.m_children)[0];
        }

        public BehaviorTask Action()
        {
            Debug.Check(this.m_children.Count == 2);

            return (this.m_children)[1];
        }
    }
}