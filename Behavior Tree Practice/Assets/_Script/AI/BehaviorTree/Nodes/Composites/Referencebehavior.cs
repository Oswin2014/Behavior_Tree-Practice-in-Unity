
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class ReferencedBehavior : BehaviorNode
    {
        public ReferencedBehavior()
        {
		}

        ~ReferencedBehavior()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            foreach (property_t p in properties)
            {
                if (p.name == "ReferenceFilename")
                {
                    this.m_referencedBehaviorPath = p.value;

                    bool bOk = Workspace.Load(this.m_referencedBehaviorPath);

                    Debug.Check(bOk);
                }
                else
                {
                    //Debug.Check(0, "unrecognised property %s", p.name);
                }
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is ReferencedBehavior))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            ReferencedBehaviorTask pTask = new ReferencedBehaviorTask();

            return pTask;
        }

        protected string m_referencedBehaviorPath;

        class ReferencedBehaviorTask : SingeChildTask
        {
            public ReferencedBehaviorTask()
            {
			}
            ~ReferencedBehaviorTask()
            {
            }

            public override void Init(BehaviorNode node)
            {
                base.Init(node);
            }

            protected override bool isContinueTicking()
            {
                return true;
            }

            protected override bool onenter(Agent pAgent)
            {
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(m_returnStatus == EBTStatus.BT_INVALID);
                ReferencedBehavior pNode = this.GetNode() as ReferencedBehavior;

                if (pNode != null)
                {
                    string pThisTree = pAgent.btgetcurrent().GetName();
                    string msg = string.Format("{0}[{1}] {2}", pThisTree, pNode.GetId(), pNode.m_referencedBehaviorPath);

			        LogManager.Log(pAgent, msg, EActionResult.EAR_none, LogMode.ELM_jump);

                    pAgent.btreferencetree(pNode.m_referencedBehaviorPath);
                }

                return EBTStatus.BT_RUNNING;
            }
        }
    }
}