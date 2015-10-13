
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class DecoratorLoopUntil : DecoratorCount
    {
        public DecoratorLoopUntil()
        {
            m_until = true;
        }
        ~DecoratorLoopUntil()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            foreach (property_t p in properties)
            {
                if (p.name == "Until")
                {
                    if (p.value == "true")
                    {
                        this.m_until = true;
                    }
                    else if (p.value == "false")
                    {
                        this.m_until = false;
                    }
                }
            }
        }

        protected override BehaviorTask createTask()
        {
            DecoratorLoopUntilTask pTask = new DecoratorLoopUntilTask();

            return pTask;
        }

        protected bool m_until;

        ///Returns EBTStatus.BT_RUNNING until the child returns EBTStatus.BT_SUCCESS. if the child returns EBTStatus.BT_FAILURE, it still returns EBTStatus.BT_RUNNING
        /**
        however, if m_until is false, the checking condition is inverted.
        i.e. it Returns EBTStatus.BT_RUNNING until the child returns EBTStatus.BT_FAILURE. if the child returns EBTStatus.BT_SUCCESS, it still returns EBTStatus.BT_RUNNING
        */
        class DecoratorLoopUntilTask : DecoratorCountTask
        {
            public DecoratorLoopUntilTask() : base()
            {
            }

            public override bool NeedRestart()
            {
                return true;
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                if (this.m_n > 0)
                {
                    this.m_n--;
                }

                if (this.m_n == 0)
                {
                    return EBTStatus.BT_SUCCESS;
                }

                Debug.Check(this.GetNode() is DecoratorLoopUntil);
                DecoratorLoopUntil pDecoratorLoopUntil = (DecoratorLoopUntil)(this.GetNode());

                if (pDecoratorLoopUntil.m_until)
                {
                    if (status == EBTStatus.BT_SUCCESS)
                    {
                        return EBTStatus.BT_SUCCESS;
                    }
                }
                else
                {
                    if (status == EBTStatus.BT_FAILURE)
                    {
                        return EBTStatus.BT_FAILURE;
                    }
                }

                return EBTStatus.BT_RUNNING;
            }
        }
    }
}