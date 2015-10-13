
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class DecoratorNot : DecoratorNode
    {
        public DecoratorNot()
        {
		}
        ~DecoratorNot()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorNot))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            DecoratorNotTask pTask = new DecoratorNotTask();


            return pTask;
        }


        class DecoratorNotTask : DecoratorTask
        {
            public DecoratorNotTask() : base()
            {
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                if (status == EBTStatus.BT_FAILURE)
                {
                    return EBTStatus.BT_SUCCESS;
                }

                if (status == EBTStatus.BT_SUCCESS)
                {
                    return EBTStatus.BT_FAILURE;
                }

                return status;
            }
        }
    }
}