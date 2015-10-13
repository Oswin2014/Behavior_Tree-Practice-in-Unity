
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class DecoratorLoop : DecoratorCount
    {
        public DecoratorLoop()
        {
		}
        ~DecoratorLoop()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorLoop))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            DecoratorLoopTask pTask = new DecoratorLoopTask();

            return pTask;
        }

        ///Returns EBTStatus.BT_FAILURE for the specified number of iterations, then returns EBTStatus.BT_SUCCESS after that
        class DecoratorLoopTask : DecoratorCountTask
        {
            public DecoratorLoopTask()
            {
            }

            ~DecoratorLoopTask()
            {
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                if (this.m_n > 0)
                {
                    this.m_n--;

                    if (this.m_n == 0)
                    {
                        return EBTStatus.BT_SUCCESS;
                    }

                    return EBTStatus.BT_RUNNING;
                }

                if (this.m_n == -1)
                {
                    return EBTStatus.BT_RUNNING;
                }

                Debug.Check(this.m_n == 0);

                return EBTStatus.BT_SUCCESS;
            }
        }
    }
}