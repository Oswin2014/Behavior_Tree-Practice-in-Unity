
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class False : ConditionBase
    {
    	public False()
    	{
		}

        ~False()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
			if (!(pTask.GetNode() is False))
			{
				return false;
			}
		
			return base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
        {
			FalseTask pTask = new FalseTask();

			return pTask;
		}


	    // ============================================================================
	    class FalseTask : ConditionBaseTask
	    {
	    	public FalseTask() : base()
	        {
	        }

	        ~FalseTask()
	        {
	        }

	        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
	        {
		        return EBTStatus.BT_FAILURE;
	    	}
	    }
    }
}