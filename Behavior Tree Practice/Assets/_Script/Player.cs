using UnityEngine;
using System.Collections;


public class AvatarInfo
{
    public GameObject obj;
    public Transform trans;
    public Animator animator;

    public AvatarInfo(GameObject go)
    {
        obj = go;
        trans = go.transform;
        animator = go.GetComponent<Animator>();
    }

    public void setActive(bool value)
    {
        obj.SetActive(value);
    }

    public void setTrans(Transform src)
    {
        trans.position = src.position;
        trans.rotation = src.rotation;
    }

}


[behaviac.TypeMetaInfo("Player", "Player -> GameActor")]
public class Player : GameActor
{

    public enum State
    {
        Idle,
        Run,
        Attack,
        Hited,
        Skill,
    }

    protected GameObject mGo;
    protected Transform mTrans;

    public GameObject cachedGameObject { get { if (mGo == null) mGo = gameObject; return mGo; } }

    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform as Transform; return mTrans; } }

    protected Animator _animator;

    public Player enemy;

    public string behaviorTree = "";
    protected bool btloadResult = false;

    [behaviac.MemberMetaInfo()]
    public State state = State.Idle;

    //攻击范围
    public float attackRange;

    //受击状态时间(秒)
    [behaviac.MemberMetaInfo()]
    public float crickTime;

    //受到攻击的时间
    [behaviac.MemberMetaInfo()]
    public float hitedTime;

    public float speed;

    float moveSpeed;

    protected float _lastHitedTime;

    /// <summary>
    /// 弧度
    /// </summary>
    public float rotateRadianSpeed;

    bool _fighting;

    bool _rotating;

    
	public virtual bool init()
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

        var playerList = GameManager.instance.PlayerList;

        for (int i = 0, max = playerList.Count; i < max; i++)
        {
            Player p = playerList[i];
            if (p == this)
                continue;

            enemy = p;
            break;
        }

        initObj();

        return true;
    }

    void initObj()
    {
        _animator = cachedGameObject.GetComponentInChildren<Animator>();

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

    [behaviac.MethodMetaInfo()]
    public behaviac.EBTStatus attack()
    {
        changeAction(State.Attack);

        return behaviac.EBTStatus.BT_SUCCESS;
    }

    public void changeAction(State newState)
    {
        //if (newState == state)
        //    return;

        //if (this is AvatarAttacker)
        //    Debug.Log(newState);

        switch (newState)
        {
            case State.Idle:
                moveSpeed = 0;
                _animator.SetFloat("moveSpeed", 0);
                if (_fighting)
                    _animator.SetInteger("attackState", 0);
                _fighting = false;
                break;
            case State.Run:
                moveSpeed = speed;
                _animator.SetFloat("moveSpeed", moveSpeed);
                if (_fighting)
                    _animator.SetInteger("attackState", 0);
                _fighting = false;
                break;
            case State.Attack:
                _animator.SetInteger("attackState", 1);
                _fighting = true;
                break;
            case State.Skill:
                _animator.SetInteger("attackState", 2);
                moveSpeed = 0;
                _fighting = true;
                break;
            case State.Hited:
                _animator.SetTrigger("hited");
                //if (!_fighting)
                //    _animator.SetInteger("attackState", 1);
                //_fighting = true;
                break;
        }

        state = newState;
    }

    public Vector3 getPosition()
    {
        return cachedTransform.position;
    }

    public float getTargetDis()
    {
        return Vector3.Distance(getPosition(), enemy.getPosition());
    }

    public void setPosition(Vector3 pos)
    {
        cachedTransform.position = pos;
    }

    [behaviac.MethodMetaInfo()]
    public bool isEnemyInRange()
    {
        float dis = Vector3.Distance(getPosition(), enemy.getPosition());

        return dis < attackRange;
    }

    [behaviac.MethodMetaInfo()]
    public behaviac.EBTStatus moveToTarget()
    {
        Vector3 pos = getPosition();
        Vector3 targetPos = enemy.getPosition();
        if (getTargetDis() < attackRange)
        {
            return behaviac.EBTStatus.BT_SUCCESS;
        }

        changeAction(State.Run);
        cachedTransform.LookAt(targetPos);
        setPosition(Vector3.MoveTowards(pos, targetPos, moveSpeed * Time.deltaTime));

        return behaviac.EBTStatus.BT_SUCCESS;
    }

    [behaviac.MethodMetaInfo()]
    public float keepCrickTime()
    {
        return (crickTime - (Time.time - hitedTime)) * 1000.0f;
    }

    public virtual void hited()
    {
        _lastHitedTime = Time.time;
        changeAction(State.Hited);
        Invoke("resumeFromHited", crickTime);
    }

    public virtual void damage()
    {

    }

    void resumeFromHited()
    {
        state = State.Attack;
    }

    public virtual float getAttackStartTime()
    {
        return -1;
    }

    [behaviac.MethodMetaInfo()]
    public behaviac.EBTStatus rotateToTarget()
    {
        if (isSkilling())
            return behaviac.EBTStatus.BT_SUCCESS;

        Vector3 pos = getPosition();
        Vector3 targetPos = enemy.getPosition();
        Vector3 targetDir = targetPos - pos;

        if (!_rotating)
        {
            float angle = Vector3.Angle(targetDir, cachedTransform.forward);

            if (angle < 10.0f)
            {
                cachedTransform.LookAt(targetPos);
                return behaviac.EBTStatus.BT_SUCCESS;
            }
            _rotating = true;
            changeAction(State.Idle);
        }

        if (Vector3.Angle(targetDir, cachedTransform.forward) < 3.0f)
        {
            cachedTransform.LookAt(targetPos);
            _rotating = false;
            return behaviac.EBTStatus.BT_SUCCESS;
        }

        if (!isIdleAnimationState())
            return behaviac.EBTStatus.BT_RUNNING;

        //if (this is AvatarAttacker)
        //    Debug.Log(Time.frameCount);

        Vector3 newDir = Vector3.RotateTowards(cachedTransform.forward, targetDir, rotateRadianSpeed * Time.deltaTime, 0.0f);
        cachedTransform.rotation = Quaternion.LookRotation(newDir);

        return behaviac.EBTStatus.BT_RUNNING;
    }

    public bool isIdleAnimationState()
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);

        return info.IsName("idle");
    }

    protected virtual bool isSkilling()
    {
        return false;
    }

}