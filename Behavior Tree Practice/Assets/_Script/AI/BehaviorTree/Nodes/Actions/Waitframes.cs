
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class WaitFrames : BehaviorNode
    {
        public WaitFrames()
        {
		}

        ~WaitFrames()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            foreach (property_t p in properties)
            {
                if (p.name == "Frames")
                {
                    string propertyName = null;

                    int pParenthesis = p.value.IndexOf('(');
                    if (pParenthesis == -1)
                    {
                        string typeName = null;
                        this.m_frames_var = Condition.LoadRight(p.value, propertyName, ref typeName);
                    }
                    else
                    {
                        //method
                        this.m_frames_method = Action.LoadMethod(p.value);
                    }
                }
            }
        }

        protected virtual int GetFrames(Agent pAgent)
        {
            if (this.m_frames_var != null)
            {
                Debug.Check(this.m_frames_var != null);
                int frames = (int)this.m_frames_var.GetValue(pAgent);

                return frames;
            }
            else if (this.m_frames_method != null)
            {
                int frames = (int)this.m_frames_method.Invoke(pAgent);

                return frames;
            }

            return 0;
        }

        protected override BehaviorTask createTask()
        {
            WaitFramesTask pTask = new WaitFramesTask();

            return pTask;
        }

        Property m_frames_var;
        CMethodBase m_frames_method;

        class WaitFramesTask : LeafTask
        {
            public WaitFramesTask()
            {
            }

            ~WaitFramesTask()
            {
            }

            protected override bool onenter(Agent pAgent)
            {
                this.m_start = 0;
                this.m_frames = this.GetFrames(pAgent);

                if (this.m_frames <= 0)
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
                this.m_start += (int)(Workspace.GetDeltaFrames());
                if (this.m_start >= this.m_frames)
                {
                    return EBTStatus.BT_SUCCESS;
                }

                return EBTStatus.BT_RUNNING;
            }

            int GetFrames(Agent pAgent)
            {
                Debug.Check(this.GetNode() is WaitFrames);
                WaitFrames pWaitNode = (WaitFrames)(this.GetNode());

                return pWaitNode != null ? pWaitNode.GetFrames(pAgent) : 0;
            }

            int m_start;
            int m_frames;
        }
    }
}
