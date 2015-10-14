using UnityEngine;
using System.Collections;



[behaviac.TypeMetaInfo("Player", "Player -> GameActor")]
public class Player : GameActor
{

    public string behaviorTree = "";
    protected bool btloadResult = false;
    
	public bool init()
    {
        base.Init();
        this.SetIdFlag(1);

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

    [behaviac.MethodMetaInfo()]
    public behaviac.EBTStatus testBT()
    {
        Debug.Log("------testBT: " + name);
        return behaviac.EBTStatus.BT_SUCCESS;
    }

}