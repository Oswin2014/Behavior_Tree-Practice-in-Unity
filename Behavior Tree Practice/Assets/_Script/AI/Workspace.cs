
using System;
using System.Collections.Generic;

namespace behavior
{
    
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

                behavior.Debug.LogWarning(string.Format("BehaviorTree {0} not loaded!", fullPath));
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

            return true;
        }


    }


}
