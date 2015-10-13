
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class DecoratorFrames : DecoratorNode
    {
        public DecoratorFrames()
        {
		}
        ~DecoratorFrames()
        {
            this.m_frames_var = null;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            foreach (property_t p in properties)
            {
                if (p.name == "Time")
                {
                    string typeName = null;
                    string propertyName = null;
                    this.m_frames_var = Condition.LoadRight(p.value, propertyName, ref typeName);
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

            return 0;
        }

        protected override BehaviorTask createTask()
        {
            DecoratorFramesTask pTask = new DecoratorFramesTask();

            return pTask;
        }

        Property m_frames_var;

        class DecoratorFramesTask : DecoratorTask
        {
            public DecoratorFramesTask()
            {
            }

            ~DecoratorFramesTask()
            {
            }

            protected override bool onenter(Agent pAgent)
            {
                base.onenter(pAgent);

                this.m_start = 0;
                this.m_frames = this.GetFrames(pAgent);

                return (this.m_frames > 0);
            }

            protected override EBTStatus decorate(EBTStatus status)
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
                Debug.Check(this.GetNode() is DecoratorFrames);
                DecoratorFrames pNode = (DecoratorFrames)(this.GetNode());

                return pNode != null ? pNode.GetFrames(pAgent) : 0;
            }

            int m_start;
            int m_frames = 0;
        }
    }
}