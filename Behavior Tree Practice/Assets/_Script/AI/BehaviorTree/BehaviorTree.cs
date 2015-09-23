using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace behavior
{
    public abstract class BehaviorNode
    {

        public EBTStatus m_status;
        protected List<BehaviorNode> m_attachments;
        protected BehaviorNode m_parent;
        protected List<BehaviorNode> m_children;

        protected BehaviorNode()
        {
        }

        ~BehaviorNode()
        {
            this.Clear();
        }

        public virtual void AddChild(BehaviorNode pChild)
        {
            pChild.m_parent = this;

            if (this.m_children == null)
            {
                this.m_children = new List<BehaviorNode>();
            }

            this.m_children.Add(pChild);
        }

        public void Attach(BehaviorNode pAttachment)
        {
            if (this.m_attachments == null)
            {
                this.m_attachments = new List<BehaviorNode>();
            }

            this.m_attachments.Add(pAttachment);
        }

        public void Clear()
        {
            if (this.m_attachments != null)
            {
                this.m_attachments.Clear();
                this.m_attachments = null;
            }

            if (this.m_children != null)
            {
                this.m_children.Clear();
                this.m_children = null;
            }
        }

        protected abstract BehaviorTask createTask();

        public BehaviorTask CreateAndInitTask()
        {
            BehaviorTask pTask = this.createTask();
            pTask.Init(this);

            return pTask;
        }

        public virtual bool enteraction_impl(Agent pAgent) { return false; }

        public virtual bool exitaction_impl(Agent pAgent) { return false; }

        public int GetChildrenCount()
        {
            if (this.m_children != null)
            {
                return this.m_children.Count;
            }

            return 0;
        }

        public BehaviorNode GetChild(int index)
        {
            if (this.m_children != null && index < this.m_children.Count)
            {
                return this.m_children[index];
            }

            return null;
        }

        public int GetAttachmentsCount()
        {
            if (this.m_attachments != null)
            {
                return this.m_attachments.Count;
            }

            return 0;
        }

        public BehaviorNode GetAttachment(int index)
        {
            if (this.m_attachments != null && index < this.m_attachments.Count)
            {
                return this.m_attachments[index];
            }

            return null;
        }

    }

    // ============================================================================

    public abstract class DecoratorNode : BehaviorNode
    {
        public DecoratorNode()
        {
            m_bDecorateWhenChildEnds = false;
        }

        ~DecoratorNode()
        { }


        public bool m_bDecorateWhenChildEnds;
    }

    // ============================================================================

    public class BehaviorTree : BehaviorNode
    {

        protected override BehaviorTask createTask()
        {
            BehaviorTreeTask pTask = new BehaviorTreeTask();
            return pTask;
        }

    }

}

