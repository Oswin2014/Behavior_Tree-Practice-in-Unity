
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    // ============================================================================
    public class Action : BehaviorNode
    {


        protected override BehaviorTask createTask()
        {
            ActionTask pTask = new ActionTask();

            return pTask;
        }

        static string ParseInstanceName(string fullName, ref string instanceName)
        {
            //Self.AgentActionTest::Action2(0)
            int pClassBegin = fullName.IndexOf('.');
            Debug.Check(pClassBegin != -1);

            instanceName = fullName.Substring(0, pClassBegin);

            string propertyName = fullName.Substring(pClassBegin + 1);
            return propertyName;
        }

        static int ParseMethodNames(string fullName, ref string agentIntanceName, ref string agentClassName, ref string methodName)
        {
            //Self.test_ns::AgentActionTest::Action2(0)
            int pClassBegin = fullName.IndexOf('.');
            Debug.Check(pClassBegin != -1);

            agentIntanceName = fullName.Substring(0, pClassBegin);

            int pBeginAgentClass = pClassBegin + 1;

            int pBeginP = fullName.IndexOf('(', pBeginAgentClass);
            Debug.Check(pBeginP != -1);

            //test_ns::AgentActionTest::Action2(0)
            int pBeginMethod = fullName.LastIndexOf(':', pBeginP);
            Debug.Check(pBeginMethod != -1);
            //skip '::'
            Debug.Check(fullName[pBeginMethod] == ':' && fullName[pBeginMethod - 1] == ':');
            pBeginMethod += 1;

            int pos1 = pBeginP - pBeginMethod;

            methodName = fullName.Substring(pBeginMethod, pos1);

            int pos = pBeginMethod - 2 - pBeginAgentClass;

            agentClassName = fullName.Substring(pBeginAgentClass, pos).Replace("::", ".");

            return pBeginP;
        }

        //suppose params are seprated by ','
        static List<string> ParseForParams(string tsrc)
        {
            int tsrcLen = tsrc.Length;
            int startIndex = 0;
            int index = 0;
            int quoteDepth = 0;

            List<string> params_ = new List<string>();

            for (; index < tsrcLen; ++index)
            {
                if (tsrc[index] == '"')
                {
                    quoteDepth++;

                    //if (quoteDepth == 1)
                    //{
                    //	startIndex = index;
                    //}

                    if ((quoteDepth & 0x1) == 0)
                    {
                        //closing quote
                        quoteDepth -= 2;
                        Debug.Check(quoteDepth >= 0);
                    }
                }
                else if (quoteDepth == 0 && tsrc[index] == ',')
                {
                    //skip ',' inside quotes, like "count, count"
                    int lengthTemp = index - startIndex;
                    string strTemp = tsrc.Substring(startIndex, lengthTemp);
                    params_.Add(strTemp);
                    startIndex = index + 1;
                }
            }//end for

            // the last param
            int lengthTemp0 = index - startIndex;
            if (lengthTemp0 > 0)
            {
                string strTemp = tsrc.Substring(startIndex, lengthTemp0);
                params_.Add(strTemp);

                //params_.Add(strTemp);
            }

            return params_;
        }

        public static CMethodBase LoadMethod(string value_)
        {
            //Self.test_ns::AgentActionTest::Action2(0)
            string agentIntanceName = null;
            string agentClassName = null;
            string methodName = null;
            int pBeginP = ParseMethodNames(value_, ref agentIntanceName, ref agentClassName, ref methodName);

            //propertyName = FormatString("%s::%s", agentClassName, methodName);
            CStringID agentClassId = new CStringID(agentClassName);
            CStringID methodId = new CStringID(methodName);

            CMethodBase method = Agent.CreateMethod(agentClassId, methodId);

            if (method == null)
            {
				behaviac.Debug.LogWarning(string.Format("No Method {0}::{1} registered\n", agentClassName, methodName));
				Debug.Check(false, string.Format("No Method {0}::{1} registered\n", agentClassName, methodName));
            }
            else
            {
                if (Agent.IsNameRegistered(agentIntanceName))
                {
                    method.InstanceName = agentIntanceName; 
                }
                else
                {
                    //Debug.Check(agentIntanceName == "Self");
                }

                Debug.Check(method != null, string.Format("No Method {0}::{1} registered", agentClassName, methodName));
                string params_ = value_.Substring(pBeginP);

                Debug.Check(params_[0] == '(');

                List<string> paramsTokens = null;

                {
                    int len = params_.Length;

                    Debug.Check(params_[len - 1] == ')');

                    string text = params_.Substring(1, len - 2);
                    //StringUtils::SplitIntoArray(text, ",", tokens);
                    paramsTokens = ParseForParams(text);
                }

                if (paramsTokens != null)
                {
                    method.Load(paramsTokens);
                }
            }

            return method;
        }



        class ActionTask : LeafTask
        {
            public ActionTask()
            {

            }
            ~ActionTask()
            {
            }
        }
    }
}