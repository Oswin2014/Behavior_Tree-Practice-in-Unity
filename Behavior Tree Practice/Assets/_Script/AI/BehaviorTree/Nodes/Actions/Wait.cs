
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace behaviac
{
    public class Wait : BehaviorNode
    {
		protected bool m_ignoreTimeScale;
        protected Property m_time_var;

        public Wait()
        {
			this.m_ignoreTimeScale = false;
			this.m_time_var = null;
        }

        ~Wait()
        {
            this.m_time_var = null;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            foreach (property_t p in properties)
            {
				if (p.name == "IgnoreTimeScale")
				{
					this.m_ignoreTimeScale = (p.value == "true");
				}
                else if (p.name == "Time")
                {
                    string typeName = null;
                    string propertyName = null;
                    this.m_time_var = Condition.LoadRight(p.value, propertyName, ref typeName);
                }
            }
        }

        protected virtual float GetTime(Agent pAgent)
        {
            if (this.m_time_var != null)
            {
                Debug.Check(this.m_time_var != null);
				object timeObj = this.m_time_var.GetValue(pAgent);
				return Convert.ToSingle(timeObj);
            }

            return 0;
        }

        protected override BehaviorTask createTask()
        {
            WaitTask pTask = new WaitTask();

            return pTask;
        }


        class WaitTask : LeafTask
        {
			private float m_start;
			private float m_time;

            public WaitTask()
            {
                m_start = 0;
                m_time = 0;
            }

			private bool GetIgnoreTimeScale()
			{
				Wait pWaitNode = this.GetNode() as Wait;
				
				return pWaitNode != null ? pWaitNode.m_ignoreTimeScale : false;
			}

			private float GetTime(Agent pAgent)
            {
                Wait pWaitNode = this.GetNode() as Wait;

                return pWaitNode != null ? pWaitNode.GetTime(pAgent) : 0;
            }

            protected override bool onenter(Agent pAgent)
            {
				if (this.GetIgnoreTimeScale())
				{
					this.m_start = Time.realtimeSinceStartup * 1000.0f;
				}
				else
				{
					this.m_start = 0;
				}

                this.m_time = this.GetTime(pAgent);

                if (this.m_time <= 0)
                {
                    return false;
                }

                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
				if (this.GetIgnoreTimeScale())
				{
					if (Time.realtimeSinceStartup * 1000.0f - this.m_start >= this.m_time)
					{
						return EBTStatus.BT_SUCCESS;
					}
				}
				else
				{
					this.m_start += Time.deltaTime * 1000.0f;
					if (this.m_start >= this.m_time)
					{
						return EBTStatus.BT_SUCCESS;
					}
				}

                return EBTStatus.BT_RUNNING;
            }
        }
    }
}