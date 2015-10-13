
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class DecoratorAlwaysRunning : DecoratorNode
    {
        public DecoratorAlwaysRunning()
        {
		}
        ~DecoratorAlwaysRunning()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorAlwaysRunning))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            DecoratorAlwaysRunningTask pTask = new DecoratorAlwaysRunningTask();

            return pTask;
        }

        class DecoratorAlwaysRunningTask : DecoratorTask
        {
            public DecoratorAlwaysRunningTask() : base()
            {
            }

            protected override void addChild(BehaviorTask pBehavior)
            {
                base.addChild(pBehavior);
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                return EBTStatus.BT_RUNNING;
            }
        }
    }
}