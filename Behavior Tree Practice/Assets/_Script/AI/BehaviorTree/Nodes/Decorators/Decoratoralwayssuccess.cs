
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class DecoratorAlwaysSuccess : DecoratorNode
    {
    	public DecoratorAlwaysSuccess()
    	{
		}
        ~DecoratorAlwaysSuccess()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
			if (!(pTask.GetNode() is DecoratorAlwaysSuccess))
			{
				return false;
			}
		
			return base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
        {
			DecoratorAlwaysSuccessTask pTask = new DecoratorAlwaysSuccessTask();
			
			return pTask;
		}

	    class DecoratorAlwaysSuccessTask : DecoratorTask
	    {
	    	public DecoratorAlwaysSuccessTask() : base()
	        {
	        }

	        protected override EBTStatus decorate(EBTStatus status)
	        {
	            return EBTStatus.BT_SUCCESS;
	        }
	    }
    }
}