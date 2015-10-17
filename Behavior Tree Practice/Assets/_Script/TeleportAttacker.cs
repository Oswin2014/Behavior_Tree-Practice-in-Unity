using UnityEngine;
using System.Collections;



[behaviac.TypeMetaInfo("TeleportAttacker", "TeleportAttacker -> Player")]
public class TeleportAttacker : Player
{

	public bool init()
    {
        base.Init();

        return true;
    }

    [behaviac.MethodMetaInfo()]
    public behaviac.EBTStatus teleport()
    {
        return behaviac.EBTStatus.BT_FAILURE;
    }

    /// <summary>
    /// 敌人是否刚开始攻击(还未击中自己, 此时施放瞬移可以躲过一次攻击)
    /// </summary>
    [behaviac.MethodMetaInfo()]
    public bool isEnemyStartAttackAct()
    {

        return false;
    }

}