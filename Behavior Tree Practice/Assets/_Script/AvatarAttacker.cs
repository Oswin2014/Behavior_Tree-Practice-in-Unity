using UnityEngine;
using System.Collections;



[behaviac.TypeMetaInfo("AvatarAttacker", "AvatarAttacker -> Player")]
public class AvatarAttacker : Player
{

	public bool init()
    {
        base.Init();


        return true;
    }

    [behaviac.MethodMetaInfo()]
    public behaviac.EBTStatus avatarAttack()
    {
        return behaviac.EBTStatus.BT_FAILURE;
    }


}