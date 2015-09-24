using UnityEngine;
using System.Collections;



public class Player : GameActor
{

    public string behaviorTree = "";
    protected bool btloadResult = false;
    
	public bool init()
    {
        if (behaviorTree.Length > 0)
        {
            btloadResult = btload(behaviorTree, true);
            if (btloadResult)
                btsetcurrent(behaviorTree);
            else
                Debug.LogError("Behavior tree data load failed! " + behaviorTree);
        }

        return true;
    }

	public void tick()
    {

        if (btloadResult && aiEnabled)
            btexec();
    }

}