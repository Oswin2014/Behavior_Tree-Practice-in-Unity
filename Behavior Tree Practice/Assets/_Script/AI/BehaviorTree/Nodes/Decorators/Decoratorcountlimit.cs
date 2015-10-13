
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{

    //CountLimit 节点在指定的循环次数到达前返回子节点返回的状态，无论成功失败还是 Running
    public class DecoratorCountLimit : DecoratorCount
    {
        public DecoratorCountLimit()
        {
		}
        ~DecoratorCountLimit()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorCountLimit))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            DecoratorCountLimitTask pTask = new DecoratorCountLimitTask();

            return pTask;
        }

        ///enter and tick the child for the specified number of iterations, then it will not enter and tick the child after that
        class DecoratorCountLimitTask : DecoratorCountTask
        {
            public DecoratorCountLimitTask()
            {
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
				if (this.CheckPredicates(pAgent))
                {
                    this.m_bInited = false;
                }

                if (!this.m_bInited)
                {
                    this.m_bInited = true;

                    int count = this.GetCount(pAgent);

                    this.m_n = count;
                }

                //if this.m_n is -1, it is endless
                if (this.m_n > 0)
                {
                    this.m_n--;
                    return true;
                }
                else if (this.m_n == 0)
                {
                    return false;
                }
                else if (this.m_n == -1)
                {
                    return true;
                }

                Debug.Check(false);

                return false;
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                Debug.Check(this.m_n >= 0 || this.m_n == -1);

                return status;
            }

            bool m_bInited;
        }
    }
}