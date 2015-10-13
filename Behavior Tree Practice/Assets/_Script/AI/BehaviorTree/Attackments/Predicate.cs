
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class Predicate : ConditionBase
    {
        protected Property m_opl;
        protected Property m_opr;
        protected CMethodBase m_opl_m;
        protected VariableComparator m_comparator;
        protected bool m_bAnd;

        public Predicate()
        {
            m_bAnd = false;
        }
        ~Predicate()
        {
            m_opl = null;
            m_opr = null;
            m_opl_m = null;
            m_comparator = null;
        }

        protected override BehaviorTask createTask()
        {
            PredicateTask pTask = new PredicateTask();

            return pTask;
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Predicate))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            string typeName = null;
            string propertyName = null;
            string comparatorName = null;

            foreach (property_t p in properties)
            {
                if (p.name == "Operator")
                {
                    comparatorName = p.value;
                }
                else if (p.name == "Opl")
                {
                    int pParenthesis = p.value.IndexOf('(');
                    if (pParenthesis == -1)
                    {
                        this.m_opl = Condition.LoadLeft(p.value, ref propertyName, null);
                    }
                    else
                    {
                        //method
                        this.m_opl_m = Action.LoadMethod(p.value);
                    }
                }
                else if (p.name == "Opr")
                {
                    //the propertyName set value at before LoadLeft()
                    this.m_opr = Condition.LoadRight(p.value, propertyName, ref typeName);
                }
                else if (p.name == "BinaryOperator")
                {
                    if (p.value == "Or")
                    {
                        this.m_bAnd = false;
                    }
                    else if (p.value == "And")
                    {
                        this.m_bAnd = true;
                    }
                    else
                    {
                        Debug.Check(false);
                    }
                }
                else
                {
                    //Debug.Check(0, "unrecognised property %s", p.name);
                }
            }

            if (!string.IsNullOrEmpty(comparatorName) && (this.m_opl != null || this.m_opl_m != null) && this.m_opr != null)
            {
                this.m_comparator = Condition.Create(typeName, comparatorName, this.m_opl, this.m_opr);
            }
        }

        public bool Evaluate(Agent pAgent)
        {
            if (this.m_comparator != null)
            {
                bool bResult = Condition.DoCompare(pAgent, this.m_comparator, this.m_opl, this.m_opl_m, this.m_opr, null);

                return bResult;
            }
            else
            {
                EBTStatus childStatus = EBTStatus.BT_INVALID;
                EBTStatus result = this.update_impl(pAgent, childStatus);
                return result == EBTStatus.BT_SUCCESS;
            }
        }


        // ============================================================================
        public class PredicateTask : AttachmentTask
        {
            public PredicateTask()
            {
            }
            ~PredicateTask()
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
                EBTStatus result = EBTStatus.BT_FAILURE;

                Predicate pPredicateNode = this.GetNode() as Predicate;

                if (pPredicateNode.m_comparator != null)
                {
                    bool bResult = pPredicateNode.Evaluate(pAgent);

                    if (bResult)
                    {
                        result = EBTStatus.BT_SUCCESS;
                    }
                }
                else
                {
                    result = pPredicateNode.update_impl(pAgent, childStatus);
                }

                return result;
            }

            public override bool NeedRestart()
            {
                return true;
            }

            public bool IsAnd()
            {
                Predicate pPredicateNode = this.GetNode() as Predicate;
                return pPredicateNode.m_bAnd;
            }
        }
    }
}
