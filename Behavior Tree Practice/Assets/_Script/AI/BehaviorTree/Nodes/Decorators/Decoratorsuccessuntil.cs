
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    //在指定的次数到达前返回成功，指定的次数到达后返回失败。如果指定的次数是-1，则总是返回成功。
    public class DecoratorSuccessUntil : DecoratorCount
    {
        public DecoratorSuccessUntil()
        {
		}
        ~DecoratorSuccessUntil()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorSuccessUntil))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            DecoratorSuccessUntilTask pTask = new DecoratorSuccessUntilTask();


            return pTask;
        }

        ///Returns EBTStatus.BT_SUCCESS for the specified number of iterations, then returns EBTStatus.BT_FAILURE after that
        class DecoratorSuccessUntilTask : DecoratorCountTask
        {
            public DecoratorSuccessUntilTask() : base()
            {
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                if (this.m_n > 0)
                {
                    this.m_n--;

                    if (this.m_n == 0)
                    {
                        return EBTStatus.BT_FAILURE;
                    }

                    return EBTStatus.BT_SUCCESS;
                }

                if (this.m_n == -1)
                {
                    return EBTStatus.BT_SUCCESS;
                }

                Debug.Check(this.m_n == 0);

                return EBTStatus.BT_FAILURE;
            }

            public override bool NeedRestart()
            {
                return true;
            }
        }
    }
}