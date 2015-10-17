using UnityEngine;
using System.Collections;



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

    Animator _animator;

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

    bool _fighting;

    
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
        if (newState == state)
            return;
        //Debug.Log(newState);
        switch (newState)
        {
            case State.Run:
                _animator.SetFloat("moveSpeed", speed);
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
                _fighting = true;
                break;
            case State.Hited:
                _animator.SetTrigger("hited");
                if (!_fighting)
                    _animator.SetInteger("attackState", 1);
                _fighting = true;
                break;
        }

        state = newState;
    }

    public Vector3 getPosition()
    {
        return cachedTransform.position;
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
        if (Vector3.Distance(pos, targetPos) < attackRange)
        {
            return behaviac.EBTStatus.BT_SUCCESS;
        }

        changeAction(State.Run);
        cachedTransform.LookAt(targetPos);
        setPosition(Vector3.MoveTowards(pos, targetPos, speed * Time.deltaTime));

        return behaviac.EBTStatus.BT_RUNNING;
    }

    [behaviac.MethodMetaInfo()]
    public float keepCrickTime()
    {
        return (crickTime - (Time.time - hitedTime)) * 1000.0f;
    }

    public void hited()
    {
        changeAction(State.Hited);
        Invoke("resumeFromHited", crickTime);
    }

    void resumeFromHited()
    {
        state = State.Attack;
    }

}