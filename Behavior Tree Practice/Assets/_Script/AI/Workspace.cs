
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace behaviac
{

    #region Config
    public static class Config
    {
        readonly private static bool ms_bIsDesktopPlayer = (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer);
        readonly private static bool ms_bIsDesktopEditor = (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXPlayer);

        static bool ms_bDebugging = false;
        public static bool IsDebugging
        {
            get
            {
                return ms_bDebugging;
            }
            set
            {
#if !BEHAVIAC_RELEASE
                ms_bDebugging = value;
#else
				if (ms_bDebugging)
				{
					behaviac.Debug.LogWarning("Debugging can't be enabled on Release! please don't define BEHAVIAC_RELEASE to enable it!\n");
				}
#endif
            }
        }

        static bool ms_bProfiling = false;
        public static bool IsProfiling
        {
            get
            {
                return ms_bProfiling;
            }
            set
            {
#if !BEHAVIAC_RELEASE
                ms_bProfiling = value;
#else
				if (ms_bProfiling)
				{
					behaviac.Debug.LogWarning("Profiling can't be enabled on Release! please don't define BEHAVIAC_RELEASE to enable it!\n");
				}
#endif
            }
        }

        public static bool IsDesktopPlayer
        {
            get
            {
                return ms_bIsDesktopPlayer;
            }
        }

        public static bool IsDesktopEditor
        {
            get
            {
                return ms_bIsDesktopEditor;
            }
        }

        public static bool IsDesktop
        {
            get
            {
                return ms_bIsDesktopPlayer || ms_bIsDesktopEditor;
            }
        }

        public static bool IsLoggingOrSocketing
        {
            get
            {
                return IsLogging || IsSocketing;
            }
        }

#if !BEHAVIAC_RELEASE
        private static bool ms_bIsLogging = false;
#else
		private static bool ms_bIsLogging = false;
#endif
        ///it is disable on pc by default
        public static bool IsLogging
        {
            get
            {
                //logging is only enabled on pc platform, it is disabled on android, ios, etc.
                return IsDesktop && ms_bIsLogging;
            }
            set
            {
#if !BEHAVIAC_RELEASE
                ms_bIsLogging = value;
#else
				if (ms_bIsLogging)
				{
					behaviac.Debug.LogWarning("Logging can't be enabled on Release! please don't define BEHAVIAC_RELEASE to enable it!\n");
				}
#endif
            }
        }

#if !BEHAVIAC_RELEASE
        private static bool ms_bIsSocketing = IsDesktop;
#else
		private static bool ms_bIsSocketing = false;
#endif
        //it is enabled on pc by default
        public static bool IsSocketing
        {
            get
            {
                return ms_bIsSocketing;
            }
            set
            {
#if !BEHAVIAC_RELEASE
                ms_bIsSocketing = value;
#else
				if (ms_bIsLogging)
				{
					behaviac.Debug.LogWarning("Socketing can't be enabled on Release! please don't define BEHAVIAC_RELEASE to enable it!\n");
				}
#endif
            }
        }

        static bool ms_bIsSuppressingNonPublicWarning;
        /// <summary>
        /// Gets or sets a value indicating is supressing non public warning.
        /// </summary>
        /// <value><c>true</c> if is supressing non public warning; otherwise, <c>false</c>.</value>
        public static bool IsSuppressingNonPublicWarning
        {
            get
            {
                return ms_bIsSuppressingNonPublicWarning;
            }
            set
            {
                ms_bIsSuppressingNonPublicWarning = value;
            }
        }

    }
    #endregion

    public static class Workspace
    {

        class BTItem_t
        {
            public List<BehaviorTreeTask> bts = new List<BehaviorTreeTask>();
            public List<Agent> agents = new List<Agent>();
        };

        [Flags]
        public enum EFileFormat
        {
            EFF_xml = 1,		                    //specify to use xml
            EFF_bson = 2,		                    //specify to use bson
            EFF_cs = 4,                            //specify to use cs
            EFF_default = EFF_xml | EFF_bson | EFF_cs,  //use the format specified by SetWorkspaceSettings
        };

        private static EFileFormat fileFormat_ = EFileFormat.EFF_xml;
        public static EFileFormat FileFormat
        {
            get
            {
                return fileFormat_;
            }
            set
            {
                fileFormat_ = value;
            }
        }


        //read from 'WorkspaceFile', prepending with 'WorkspacePath', relative to the exe's path
        private static string ms_workspaceExportPath;
        public static string WorkspaceExportPath
        {
            get
            {
                Debug.Check(!string.IsNullOrEmpty(ms_workspaceExportPath));

                return ms_workspaceExportPath;
            }
        }
        static Dictionary<string, BTItem_t> ms_allBehaviorTreeTasks = new Dictionary<string, BTItem_t>();


        private static Dictionary<string, BehaviorTree> ms_behaviortrees;
        private static Dictionary<string, BehaviorTree> BehaviorTrees
        {
            get
            {
                if (ms_behaviortrees == null)
                {
                    ms_behaviortrees = new Dictionary<string, BehaviorTree>();
                }

                return ms_behaviortrees;
            }
        }

        private static Dictionary<string, Type> ms_behaviorNodeTypes;
        
		static Dictionary<CStringID, int> m_actions_count = new Dictionary<CStringID, int>();



        /**
        uses the behavior tree in the cache, if not loaded yet, it loads the behavior tree first
        */
        public static BehaviorTreeTask CreateBehaviorTreeTask(string relativePath)
        {
            BehaviorTree bt = null;
            if (BehaviorTrees.ContainsKey(relativePath))
            {
                bt = BehaviorTrees[relativePath];
            }
            else
            {
                bool bOk = Workspace.Load(relativePath);

                if (bOk)
                {
                    bt = BehaviorTrees[relativePath];
                }
            }

            if (bt != null)
            {
                BehaviorTask task = bt.CreateAndInitTask();
                Debug.Check(task is BehaviorTreeTask);
                BehaviorTreeTask behaviorTreeTask = task as BehaviorTreeTask;

                if (!ms_allBehaviorTreeTasks.ContainsKey(relativePath))
                {
                    ms_allBehaviorTreeTasks[relativePath] = new BTItem_t();
                }

                BTItem_t btItem = ms_allBehaviorTreeTasks[relativePath];
                if (!btItem.bts.Contains(behaviorTreeTask))
                {
                    btItem.bts.Add(behaviorTreeTask);
                }

                return behaviorTreeTask;
            }

            return null;
        }

        public static BehaviorNode CreateBehaviorNode(string className)
        {
            if (ms_behaviorNodeTypes.ContainsKey(className))
            {
                Type type = ms_behaviorNodeTypes[className];
                object p = Activator.CreateInstance(type);
                return p as BehaviorNode;
            }

            return null;
        }

        public static bool IsValidPath(string relativePath)
        {
            Debug.Check(!string.IsNullOrEmpty(relativePath));

            if (relativePath[0] == '.' && (relativePath[1] == '/' || relativePath[1] == '\\'))
            {
                // ./dummy_bt
                return false;
            }
            else if (relativePath[0] == '/' || relativePath[0] == '\\')
            {
                // /dummy_bt
                return false;
            }

            return true;
        }

        public static bool Load(string relativePath)
        {
            return Load(relativePath, false);
        }

        public static bool Load(string relativePath, bool bForce)
        {
            Debug.Check(string.IsNullOrEmpty(StringUtils.FindExtension(relativePath)), "no extention to specify");
            Debug.Check(Workspace.IsValidPath(relativePath));

            BehaviorTree pBT = null;
            if (BehaviorTrees.ContainsKey(relativePath))
            {
                if (!bForce)
                {
                    return true;
                }

                pBT = BehaviorTrees[relativePath];
            }

            string fullPath = Workspace.WorkspaceExportPath;
            fullPath += relativePath;
            
            EFileFormat f = Workspace.FileFormat;

            bool bLoadResult = false;

            bool bCleared = false;
            bool bNewly = false;

            if (pBT == null)
            {
                bNewly = true;
                pBT = new BehaviorTree();

                //in case of circular referencebehavior
                BehaviorTrees[relativePath] = pBT;
            }

            if(EFileFormat.EFF_xml == f)
            {
                byte[] pBuffer = ReadFileToBuffer(fullPath, ".xml");
                if (pBuffer != null)
                {
                    //if forced to reload
                    if (!bNewly)
                    {
                        bCleared = true;
                        pBT.Clear();
                    }

                    bLoadResult = pBT.load_xml(pBuffer);
                }
            }
            else
            {
                Debug.Check(false);
            }

            if (bLoadResult)
            {
                Debug.Check(pBT.GetName() == relativePath);
                if (!bNewly)
                {
                    Debug.Check(BehaviorTrees[pBT.GetName()] == pBT);
                }
            }
            else
            {
                if (bNewly)
                {
                    bool removed = BehaviorTrees.Remove(relativePath);
                    Debug.Check(removed);
                }
                else if (bCleared)
                {
                    //it has been cleared but failed to load, to remove it
                    BehaviorTrees.Remove(relativePath);
                }

                behaviac.Debug.LogWarning(string.Format("BehaviorTree {0} not loaded!", fullPath));
            }

            return true;
        }

        public static void RecordBTAgentMapping(string relativePath, Agent agent)
        {
            if (ms_allBehaviorTreeTasks == null)
            {
                ms_allBehaviorTreeTasks = new Dictionary<string, BTItem_t>();
            }

            if (!ms_allBehaviorTreeTasks.ContainsKey(relativePath))
            {
                ms_allBehaviorTreeTasks[relativePath] = new BTItem_t();
            }

            BTItem_t btItems = ms_allBehaviorTreeTasks[relativePath];
            //bool bFound = false;

            if (btItems.agents.IndexOf(agent) == -1)
            {
                btItems.agents.Add(agent);
            }
        }

        static byte[] ReadFileToBuffer(string file, string ext)
        {
            byte[] pBuffer = FileManager.Instance.FileOpen(file, ext);

            return pBuffer;
        }

        public static void RegisterBehaviorNode()
        {
            //Assembly a = Assembly.GetExecutingAssembly();
            Assembly a = Assembly.GetCallingAssembly();

            RegisterBehaviorNode(a);
        }

        public static void RegisterBehaviorNode(Assembly a)
        {
            //Debug.Check(ms_behaviorNodeTypes != null);
            if (ms_behaviorNodeTypes == null)
            {
                ms_behaviorNodeTypes = new Dictionary<string, Type>();
            }

            //List<Type> agentTypes = new List<Type>();
            Type[] types = a.GetTypes();
            foreach (Type type in types)
            {
                if (type.IsSubclassOf(typeof(behaviac.BehaviorNode)) && !type.IsAbstract)
                {
                    //Attribute[] attributes = (Attribute[])type.GetCustomAttributes(typeof(Behaviac.Design.BehaviorNodeDescAttribute), false);
                    //if (attributes.Length > 0)
                    {
                        ms_behaviorNodeTypes[type.Name] = type;
                    }
                }
            }
        }


        public static bool SetWorkspaceSettings(string workspaceExportPath, EFileFormat format)
        {
            Debug.Check(!workspaceExportPath.EndsWith("\\"), "use '/' instead of '\\'");

            bool bFirstTime = string.IsNullOrEmpty(ms_workspaceExportPath);

            ms_workspaceExportPath = workspaceExportPath;

            if (!ms_workspaceExportPath.EndsWith("/"))
            {
                ms_workspaceExportPath += '/';
            }

            fileFormat_ = format;

            //////////////////////////////////////////////////////////
            //only register metas and others at the 1st time
            if (bFirstTime)
            {
                //behaviac.Details.RegisterCompareValue();
                //behaviac.Details.RegisterComputeValue();
                behaviac.Workspace.RegisterBehaviorNode();
                //behaviac.Workspace.RegisterMetas();
            }

            return true;
        }
        
		public static int UpdateActionCount(string actionString)
		{
			lock (m_actions_count)
			{
				int count = 1;
				CStringID actionId = new CStringID(actionString);
				if (!m_actions_count.ContainsKey(actionId))
				{
					m_actions_count[actionId] = count;
				}
				else
				{
					count = m_actions_count[actionId];
					count++;
					m_actions_count[actionId] = count;
				}
				
				return count;
			}
		}
		
		public static int GetActionCount(string actionString)
		{
			lock (m_actions_count)
			{
				int count = 0;
				CStringID actionId = new CStringID(actionString);
				if (m_actions_count.ContainsKey(actionId))
				{
					count = m_actions_count[actionId];
				}
				
				return count;
			}
		}

        static string m_workspaceFileAbs = "";
        public static string GetWorkspaceAbsolutePath()
        {
            return m_workspaceFileAbs;
        }


    }


}
