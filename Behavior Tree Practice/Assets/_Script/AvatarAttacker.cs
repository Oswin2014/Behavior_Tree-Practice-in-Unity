
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[behaviac.TypeMetaInfo("AvatarAttacker", "AvatarAttacker -> Player")]
public class AvatarAttacker : Player
{

    AvatarInfo[] avatarPool = new AvatarInfo[6];

    bool avatarAttacking;

    public float avatarCD;

    float _lastAvatarTime;

    public float avatarTime;

    float _skillAnimTime = 0.8f;

    bool damaging;

    public override bool init()
    {
        base.init();

        _lastAvatarTime = -avatarCD;

        Transform parent = cachedTransform.parent;
        GameObject poolObj = GenericHelper.FindChildByName(parent.gameObject, "Pool", true);
        GameObject avatarRoot = GenericHelper.FindChildByName(poolObj, "Avatar_DK", true);
        Transform trans = avatarRoot.transform;

        Transform t;
        for (int i = 0, max = trans.childCount; i < max; ++i)
        {
            t = trans.GetChild(i);
            avatarPool[i] = new AvatarInfo(t.gameObject);
        }

        return true;
    }

    void Update()
    {
        //var info = _animator.GetCurrentAnimatorStateInfo(0);
        //Debug.Log(info.normalizedTime);
        //Debug.Log(info.IsName("attack"));
    }

    [behaviac.MethodMetaInfo()]
    public behaviac.EBTStatus avatarAttack()
    {
        //behaviac.Config.IsLogging = true;

        if (!avatarAttacking)
        {
            if (Time.time - _lastAvatarTime < avatarCD)
                return behaviac.EBTStatus.BT_FAILURE;

            if (State.Hited != enemy.state)
                return behaviac.EBTStatus.BT_FAILURE;

            _lastAvatarTime = Time.time;
            avatarAttacking = true;
            StopAllCoroutines();

            StartCoroutine(avatarAction());
        }

        return avatarAttacking ? behaviac.EBTStatus.BT_RUNNING : behaviac.EBTStatus.BT_SUCCESS;

    }

    public override float getAttackStartTime()
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);

        if (!info.IsName("attack"))
            return -1;

        float nowTime = info.normalizedTime * info.length;
        return Time.time - nowTime;
    }

    IEnumerator avatarAction()
    {
        changeAction(State.Skill);

        Vector3 originPos = getPosition();
        Vector3 targetPos = enemy.getPosition();

        setPosition(targetPos + (originPos - targetPos).normalized * attackRange);

        yield return new WaitForSeconds(avatarTime);
        StartCoroutine(showAvatar(0));

        yield return new WaitForSeconds(_skillAnimTime - avatarTime);

        for (int i = 2; i < 8; i++)
        {
            Vector3 pos = getPosition();
            //Debug.Log("---pos: x: " + pos.x + " y: " + pos.y + " z: " + pos.z);

            targetPos = enemy.getPosition();
            //Debug.Log(Vector3.Distance(getPosition(), targetPos));
            //Debug.Log("---targetPos: x: " + targetPos.x + " y: " + targetPos.y + " z: " + targetPos.z);

            float angle = -120.0f;
            if (4 == i || 7 == i)
            {
                angle = 60.0f;
            }

            cachedTransform.RotateAround(targetPos, Vector3.up, angle);
            cachedTransform.LookAt(targetPos);

            yield return new WaitForSeconds(avatarTime);
            int index = i - 1;
            if (index > -1 && index < 6)
                StartCoroutine(showAvatar(index));

            yield return new WaitForSeconds(_skillAnimTime - avatarTime);

        }

        hideAllAvatar();
        avatarAttacking = false;
    }

    public override void hited()
    {
        if (avatarAttacking)
            return;

        base.hited();
    }

    protected override bool isSkilling()
    {
        return avatarAttacking;
    }

    IEnumerator showAvatar(int i)
    {
        AvatarInfo avatarInfo = avatarPool[i];

        avatarInfo.setActive(true);
        avatarInfo.setTrans(cachedTransform);

        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        avatarInfo.animator.Play(info.fullPathHash, 0, avatarTime / _skillAnimTime);

        yield return new WaitForEndOfFrame();

        avatarInfo.animator.Stop();

    }

    void hideAllAvatar()
    {
        for (int i = 0, max = avatarPool.Length; i < max; ++i)
        {
            avatarPool[i].setActive(false);
        }
    }

    public override void damage()
    {
        damaging = true;
    }

    public bool checkDamaging()
    {
        bool ret = damaging;
        damaging = false;
        return ret;
    }

}