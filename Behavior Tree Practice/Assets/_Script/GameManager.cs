using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class GameManager : behavior.Agent
{

    protected GameObject mGo;
    protected RectTransform mTrans;

    public GameObject cachedGameObject { get { if (mGo == null) mGo = gameObject; return mGo; } }

    public RectTransform cachedTransform { get { if (mTrans == null) mTrans = transform as RectTransform; return mTrans; } }


    private List<Player> mPlayerList = new List<Player>();

    public static string WorkspaceExportedPath
    {
        get
        {
            string path = "";
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                path = Application.dataPath + "/Resources/BehaviorData/exported";
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = Application.dataPath + "/Resources/BehaviorData/exported";
            }
            else
            {
                path = "Assets/Resources/BehaviorData/exported";
            }

            return path;
        }
    }


    // Use this for initialization
    void Awake()
    {
        string btExportPath = WorkspaceExportedPath;
        behavior.Workspace.EFileFormat btFileFormat = behavior.Workspace.EFileFormat.EFF_xml;
        behavior.Workspace.SetWorkspaceSettings(btExportPath, btFileFormat);            

        //find and add player
        Player[] players = cachedGameObject.GetComponentsInChildren<Player>();
        mPlayerList.AddRange(players);

        for (int i = 0, max = mPlayerList.Count; i < max; i++)
        {
            mPlayerList[i].init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0, max = mPlayerList.Count; i < max; i++)
        {
            mPlayerList[i].tick();
        }
    }
}