
namespace behaviac
{
    public abstract class ConditionBase : BehaviorNode
    {
        public ConditionBase()
        {
        }
        ~ConditionBase()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is ConditionBase))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }
    }

    
    // ============================================================================
    class ConditionBaseTask : LeafTask
    {
        public ConditionBaseTask()
        {
        }

        ~ConditionBaseTask()
        {
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
            return EBTStatus.BT_SUCCESS;
        }

        protected override bool isContinueTicking()
        {
            return false;
        }
    }

}
