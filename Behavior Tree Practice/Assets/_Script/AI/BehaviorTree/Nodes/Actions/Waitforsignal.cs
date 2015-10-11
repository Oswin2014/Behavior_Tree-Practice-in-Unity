
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class WaitforSignal : BehaviorNode
    {
        public WaitforSignal()
        {
		}

        ~WaitforSignal()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is WaitforSignal))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            WaitforSignalTask pTask = new WaitforSignalTask();

            return pTask;
        }
    }

    // ============================================================================
    class WaitforSignalTask : SingeChildTask
    {
        public WaitforSignalTask()
            : base()
        {
            m_bTriggered = false;
        }
        ~WaitforSignalTask()
        {
        }

        public override void Init(BehaviorNode node)
        {
            base.Init(node);
        }

		public override bool CheckPredicates(Agent pAgent)
		{
			//when there are no predicates, not triggered
			bool bTriggered = false;
			if (this.m_attachments != null)
			{
				bTriggered = base.CheckPredicates(pAgent);
			}

			return bTriggered;
		}

        protected override bool onenter(Agent pAgent)
        {
			this.m_bTriggered = false;

            return true;
        }

        protected override void onexit(Agent pAgent, EBTStatus s)
        {
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            if (childStatus != EBTStatus.BT_RUNNING)
            {
                return childStatus;
            }

            if (!this.m_bTriggered)
            {
                this.m_bTriggered = this.CheckPredicates(pAgent);
            }

            if (this.m_bTriggered)
            {
                if (this.m_root == null)
                {
                    return EBTStatus.BT_SUCCESS;
                }

                EBTStatus status = base.update(pAgent, childStatus);

                return status;
            }

            return EBTStatus.BT_RUNNING;
        }

        protected override bool isContinueTicking()
        {
            //return !this.m_bTriggered;
            return true;
        }

        bool m_bTriggered;
    }
}