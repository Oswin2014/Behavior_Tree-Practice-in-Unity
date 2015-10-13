
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class Condition : ConditionBase
    {

        protected Property m_opl;
        Property m_opr;
        CMethodBase m_opl_m;
        VariableComparator m_comparator;

        public Condition()
        {
        }

        ~Condition()
        {
            m_opl = null;
            m_opr = null;
            m_opl_m = null;
            m_comparator = null;
        }

        public static VariableComparator Create(string typeName, string comparionOperator, Property lhs, Property rhs)
        {
            E_VariableComparisonType comparisonType = VariableComparator.ParseComparisonType(comparionOperator);

            //bool bAgentPtr = false;
            //it might be par or the right value of condition/assignment
            if (Agent.IsAgentClassName(typeName))
            {
                //bAgentPtr = true;
                typeName = "void*";
            }

            VariableComparator pComparator = VariableComparator.Create(typeName, lhs, rhs);
            pComparator.SetComparisonType(comparisonType);

            return pComparator;
        }

        protected override BehaviorTask createTask()
        {
            ConditionTask pTask = new ConditionTask();

            return pTask;
        }

        public static bool DoCompare(Agent pAgent, VariableComparator comparator, Property opl, CMethodBase opl_m, Property opr, CMethodBase opr_m)
        {
            bool bResult = false;
            if (opl != null)
            {
                Agent agent_left = opl.GetParentAgent(pAgent);
                Agent agent_right = opr != null ? opr.GetParentAgent(pAgent) : opr_m.GetParentAgent(pAgent);

                bResult = comparator.Execute(agent_left, agent_right);
            }
            else if (opl_m != null)
            {
                Agent agent_left = opl_m.GetParentAgent(pAgent);

                object returnValue = opl_m.Invoke(agent_left, pAgent);

                Agent agent_right = opr != null ? opr.GetParentAgent(pAgent) : opr_m.GetParentAgent(pAgent);

                bResult = comparator.Execute(returnValue, agent_left, agent_right);
            }

            return bResult;
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

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Condition))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        public static Property LoadProperty(string value)
        {
            return null;
        }

        public static Property LoadLeft(string value, ref string propertyName, string constValue)
        {
            Property opl = null;

            if (!string.IsNullOrEmpty(value))
            {
                string[] tokens = value.Split(' ');

                if (tokens != null && tokens.Length == 2)
                {
                    //int AgentTest::Property1
                    string typeName = tokens[0].Replace("::", ".");
                    propertyName = tokens[1];
                    opl = Property.Create(typeName, tokens[1], constValue, false, false);
                }
                else
                {
                    //static int AgentTest::Property6
                    Debug.Check(tokens[0] == "static");
                    string typeName = tokens[1].Replace("::", ".");
                    propertyName = tokens[2];
                    opl = Property.Create(typeName, tokens[2], constValue, true, false);
                }
            }

            return opl;
        }

        public static Property LoadRight(string value, string propertyName, ref string typeName)
        {
            Property opr = null;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.StartsWith("const"))
                {
                    //const Int32 0
                    const int kConstLength = 5;
                    string strRemaining = value.Substring(kConstLength + 1);
                    int p = StringUtils.FirstToken(strRemaining, ' ', ref typeName);

                    typeName = typeName.Replace("::", ".");

                    string strVale = strRemaining.Substring(p + 1);
                    opr = Property.Create(typeName, propertyName, strVale, false, true);
                }
                else
                {
                    string[] tokens = value.Split(' ');

                    if (tokens[0] == "static")
                    {
                        //static int Property1
                        Debug.Check(tokens.Length == 3);
                        typeName = tokens[1].Replace("::", ".");
                        opr = Property.Create(typeName, tokens[2], null, true, false);
                    }
                    else
                    {
                        //int Property1
                        Debug.Check(tokens.Length == 2);
                        typeName = tokens[0].Replace("::", ".");
                        opr = Property.Create(typeName, tokens[1], null, false, false);
                    }
                }
            }

            return opr;
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
                        this.m_opl = LoadLeft(p.value, ref propertyName, null);
                    }
                    else
                    {
                        //method
                        this.m_opl_m = Action.LoadMethod(p.value);
                    }
                }
                else if (p.name == "Opr")
                {
                    this.m_opr = Condition.LoadRight(p.value, propertyName, ref typeName);
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

        public static Property ParseProperty(string value, ref string typeName)
        {
            Property opr = null;
            return opr;
        }

        class ConditionTask : ConditionBaseTask
        {
            public ConditionTask()
            {
            }

            ~ConditionTask()
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
                Debug.Check(childStatus == EBTStatus.BT_RUNNING);

                Debug.Check(this.GetNode() is Condition);
                Condition pConditionNode = (Condition)(this.GetNode());

                bool ret = pConditionNode.Evaluate(pAgent);

                return ret ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
            }

        }

    }
}