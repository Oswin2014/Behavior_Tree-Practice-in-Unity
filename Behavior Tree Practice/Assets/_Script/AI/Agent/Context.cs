
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Globalization;

namespace behaviac
{
    public class Context
    {
        private static Dictionary<int, Context> ms_contexts = new Dictionary<int, Context>();
        private Dictionary<string, Agent> m_namedAgents = new Dictionary<string, Agent>();
        private Dictionary<string, Variables> m_static_variables = new Dictionary<string, Variables>();

        int m_context_id;
        //World m_world;

        Context(int contextId)
        {
            //m_world = null;
            m_context_id = contextId;
        }

        ~Context()
        {
            //this.m_world = null;

            //this.CleanupStaticVariables();
            this.CleanupInstances();

            //ms_eventInfosGlobal.Clear();
        }

        void CleanupInstances()
        {
            //foreach (KeyValuePair<string, Agent> p in m_namedAgents)
            //{
            //    string msg = string.Format("{0}:{1}", p.Key,p.Value.GetName());
            //    behaviac.Debug.Log(msg);
            //}

            //Debug.Check(m_namedAgents.Count == 0, "you need to call DestroyInstance or UnbindInstance");

            m_namedAgents.Clear();
        }

        public static Context GetContext(int contextId)
        {
            Debug.Check(contextId >= 0);
            if (ms_contexts.ContainsKey(contextId))
            {
                Context pContext = ms_contexts[contextId];
                return pContext;
            }

            Context pC = new Context(contextId);
            ms_contexts[contextId] = pC;

            return pC;
        }

        public Agent GetInstance(string agentInstanceName)
        {
            bool bValidName = !string.IsNullOrEmpty(agentInstanceName);
            if (bValidName)
            {
                string className = null;
                GetClassNameString(agentInstanceName, ref className);

                if (m_namedAgents.ContainsKey(className))
                {
                    Agent pA = m_namedAgents[className];

                    return pA;
                }

                return null;
            }

            return null;
        }

        static bool GetClassNameString(string variableName, ref string className)
        {
            Debug.Check(!string.IsNullOrEmpty(variableName));

            int pSep = variableName.LastIndexOf(':');

            if (pSep > 0)
            {
                Debug.Check(variableName[pSep - 1] == ':');
                className = variableName.Substring(0, pSep - 1);

                return true;
            }
            else
            {
                className = variableName;
                return true;
            }

            //return false;
        }

        public static void LogCurrentStates()
        {
        }

        /**
        if staticClassName is no null, it is for static variable
        */
        public void SetStaticVariable<VariableType>(CMemberBase pMember, string variableName, VariableType value, string staticClassName, uint variableId)
        {
            Debug.Check(!string.IsNullOrEmpty(variableName));
            Debug.Check(!string.IsNullOrEmpty(staticClassName));

            if (!m_static_variables.ContainsKey(staticClassName))
            {
                m_static_variables[staticClassName] = new Variables();
            }

            Variables variables = m_static_variables[staticClassName];
            variables.Set(null, pMember, variableName, value, variableId);
        }

    }
}