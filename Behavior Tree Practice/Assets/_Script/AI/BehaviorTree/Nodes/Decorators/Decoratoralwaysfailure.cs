
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class DecoratorAlwaysFailure : DecoratorNode
    {
        public DecoratorAlwaysFailure()
        {
		}
        ~DecoratorAlwaysFailure()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorAlwaysFailure))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            DecoratorAlwaysFailureTask pTask = new DecoratorAlwaysFailureTask();

            return pTask;
        }

        class DecoratorAlwaysFailureTask : DecoratorTask
        {
            public DecoratorAlwaysFailureTask() : base()
            {
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                return EBTStatus.BT_FAILURE;
            }
        }
    }
}