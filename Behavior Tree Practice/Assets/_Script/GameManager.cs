using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[behaviac.TypeMetaInfo("GameManager", "GameManager -> Agent")]
public class GameManager : behaviac.Agent
{
    static GameManager _instance;

    public static GameManager instance
    {
        get { return _instance; }
    }


    protected GameObject mGo;
    protected Transform mTrans;

    public GameObject cachedGameObject { get { if (mGo == null) mGo = gameObject; return mGo; } }

    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform as Transform; return mTrans; } }


    private List<Player> mPlayerList = new List<Player>();

    public List<Player> PlayerList
    {
        get { return mPlayerList; }
    }

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

    public static string WorkspacePath
    {
        get
        {
            string path = "";
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                path = Application.dataPath + "/BTWorkspace";
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = Application.dataPath + "/BTWorkspace";
            }
            else
            {
                behaviac.Debug.LogWarning("only for dev!");
            }

            return path;
        }
    }


    // Use this for initialization
    void Awake()
    {
        _instance = this;

        BehaviacSystem.instance.init();

        //find and add player
        Player[] players = cachedGameObject.GetComponentsInChildren<Player>();
        mPlayerList.AddRange(players);

        for (int i = 0, max = mPlayerList.Count; i < max; i++)
        {
            Player p = mPlayerList[i];
            if(p.enabled)
                p.init();
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