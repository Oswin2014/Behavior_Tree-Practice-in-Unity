
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class True : ConditionBase
    {
	    public True()
	    {
		}

        ~True()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
			if (!(pTask.GetNode() is True))
			{
				return false;
			}
		
			return base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
        {
			TrueTask pTask = new TrueTask();

			return pTask;
		}


	    // ============================================================================
	    class TrueTask : ConditionBaseTask
	    {
	    	public  TrueTask() : base()
	        {
	        }

	        ~TrueTask()
	        {
	        }

        	protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
        		return EBTStatus.BT_SUCCESS;
    		}
   		}
    }
}