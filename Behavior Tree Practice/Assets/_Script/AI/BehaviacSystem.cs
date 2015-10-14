
using UnityEngine;
using System.Collections;
using System;


public class BehaviacSystem
{
    static BehaviacSystem _instance;

    public static BehaviacSystem instance
    {
        get {
            if (null == _instance)
                _instance = new BehaviacSystem();

            return _instance; 
        }
    }

		private bool m_bIsInited = false;

		public bool init ()
		{		
			if (!m_bIsInited) 
			{
				m_bIsInited = true;

            	//behaviac.Workspace.RegisterBehaviorNode ();

                string btExportPath = GameManager.WorkspaceExportedPath;
                behaviac.Workspace.EFileFormat btFileFormat = behaviac.Workspace.EFileFormat.EFF_xml;
                behaviac.Workspace.SetWorkspaceSettings(btExportPath, btFileFormat);

                //register agents in ExportMetas(), no need at here.
				//behaviac.Workspace.RegisterMetas ();				
         
				//register names
                behaviac.Agent.RegisterName<GameManager>("GameManager");

                string metaExportPath = GameManager.WorkspacePath + "/xmlmeta/BTPracticeMeta.xml";
				behaviac.Workspace.ExportMetas (metaExportPath);
				behaviac.Debug.Log ("Behaviac meta data export over.");

				//behaviac.Workspace.RespondToBreakHandler += RespondToBreak;

				//< write log file
				//behaviac.Config.IsLogging = false;
				//behaviac.Config.IsSocketing = false;				
				
				bool isBlockSocket = false;
				behaviac.SocketUtils.SetupConnection (isBlockSocket);		
				behaviac.Agent.SetIdMask (0xffffffff);
			}

			return true;
		}

}
