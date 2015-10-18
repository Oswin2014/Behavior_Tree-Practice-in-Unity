using UnityEngine;
using System.Collections;



[behaviac.TypeMetaInfo("TeleportAttacker", "TeleportAttacker -> Player")]
public class TeleportAttacker : Player
{
    AvatarInfo avatarInfo;

    public float teleportDis;

    //瞬移施放时间
    public float teleportCastTime;

    public float teleportCD;

    //下次瞬移的时间点
    float mTeleportTime;

    //将要瞬移
    bool willTeleport;

	public override bool init()
    {
        base.init();

        mTeleportTime = -teleportCD + 1.5f;

        Transform parent = cachedTransform.parent;
        GameObject poolObj = GenericHelper.FindChildByName(parent.gameObject, "Pool", true);
        GameObject avatarObj = GenericHelper.FindChildByName(poolObj, "Avatar_Alice", true);

        avatarInfo = new AvatarInfo(avatarObj);

        return true;
    }

    [behaviac.MethodMetaInfo()]
    public behaviac.EBTStatus teleport()
    {
        if (0 == teleportDis)
            return behaviac.EBTStatus.BT_FAILURE;

        if (!willTeleport)
        {
            if (Time.time - mTeleportTime < teleportCD)
                return behaviac.EBTStatus.BT_FAILURE;

            changeAction(State.Skill);
            mTeleportTime = Time.time + teleportCastTime;
            willTeleport = true;

            return behaviac.EBTStatus.BT_RUNNING;
        }

        if (mTeleportTime < Time.time)
        {
            StartCoroutine(showAvatar());

            Vector3 pos = getPosition();
            Vector3 targetPos = enemy.getPosition();
            float disToTarget = getTargetDis();

            if (disToTarget > teleportDis)
            {
                setPosition(pos + (targetPos - pos).normalized * teleportDis);
            }
            else
            {
                Vector3 teleportPos = targetPos - enemy.cachedTransform.right * attackRange;
                setPosition(teleportPos);
                cachedTransform.LookAt(targetPos);
            }

            willTeleport = false;
            return behaviac.EBTStatus.BT_SUCCESS;
        }


        return behaviac.EBTStatus.BT_RUNNING;
    }

    /// <summary>
    /// 敌人是否刚开始攻击(还未击中自己, 此时施放瞬移可以躲过一次攻击)
    /// </summary>
    [behaviac.MethodMetaInfo()]
    public bool isEnemyStartAttackAct()
    {
        float startTime = enemy.getAttackStartTime();

        return hitedTime < startTime;
    }

    protected override bool isSkilling()
    {
        return willTeleport;
    }

    IEnumerator showAvatar()
    {
        avatarInfo.setActive(true);
        avatarInfo.setTrans(cachedTransform);

        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        avatarInfo.animator.Play(info.fullPathHash, 0, info.normalizedTime);

        yield return new WaitForEndOfFrame();

        avatarInfo.animator.Stop();

        yield return new WaitForSeconds(1.0f);

        avatarInfo.setActive(false);
    }
}