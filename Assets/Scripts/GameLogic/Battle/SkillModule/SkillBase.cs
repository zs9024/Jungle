using UnityEngine;
public abstract class SkillBase
{
    //技能状态
    public SkillState State { get; protected set; }
    public int SkillID { get { return dataVo == null ? 0 : dataVo.ID; } }
    public SkillDataVo dataVo { get; protected set; }

    //特效
    protected GameObject effect;
    protected bool ifInitEffect = false; //是否已经播放了特效
    protected float effectBeginTime;
    protected float effectTime = 5f;     //特效时长

    //伤害
    protected bool damaged = false;     //是否已经计算了伤害


    //动画
    protected float curAnimalLength;//当前动画时长
    protected string animalName;    //动画名称
    protected float CurCd;          //当前cd时长
    protected float timer;


    //攻击目标、角度范围、距离
    protected bool InForward(Transform owner, Transform trans, float _angle, float _dis)    {        if (null == trans)        {            return false;        }           // 获得从当前Obj到目标Obj的方向，然后求和当前Obj的朝向夹角。        Vector2 tarPos = new Vector2(trans.position.x, trans.position.z);        Vector2 myPos = new Vector2(owner.transform.position.x, owner.transform.position.z);        Vector2 mydir = new Vector2(owner.transform.forward.x, owner.transform.forward.z);        Vector2 dir = tarPos - myPos;        float angle = Vector2.Angle(mydir.normalized, dir.normalized);         if (angle < _angle / 2f)        {             float toPosDis = Vector2.Distance(tarPos, myPos);            if (toPosDis <= _dis)            {                return true;            }            else            {                return false;            }        }        return false;    }


    public SkillBase(int skillID)
    {
        State = SkillState.None;
        this.CurCd = 0f;

        //dataVo = ConfigDataManager.GetSkillDataByID(skillID);
        dataVo = new SkillDataVo();
        //拿到该动画时长
        curAnimalLength = 1f;

        animalName = dataVo.animName;

        //加载特效
        //GameObject effectPre = LoadCache.LoadEffect(dataVo.effectName);
        //if (effectPre != null)
        //{
        //    effect = GameObject.Instantiate(effectPre) as GameObject;
        //    effect.SetActive(false);
        //}

    }

    //  Interrupts
    virtual public void SetInterrupts() { State = SkillState.Interrupts; }


    virtual public void ProcessBuffer(int index) { }


    virtual public bool IsCanInterrupts()
    {
        if (State == SkillState.None)
            return true;

        if (dataVo.minInterrupt != -1 && timer <= dataVo.minInterrupt)
            return true;

        if (dataVo.maxInterrupt != -1 && timer >= dataVo.maxInterrupt)
            return true;

        return false;
    }

    virtual public void Execute()
    {
        switch (State)
        {
            case SkillState.Start:
                break;
            case SkillState.Execution:
                break;
            case SkillState.Interrupts:
                break;
            case SkillState.Finish:
                break;
        }

    }

    virtual public void Begin()
    {
        State = SkillState.Start;
    }

    //这是一个模
    virtual public void DoUpdate()
    {
        if (State == SkillState.None)
        {
            return;
        }

        //  if (owner.mAIState == AIState.Dead)
        //   return;

        Execute();
        CalcCd();
    }


    //CD计时
    public void CalcCd()
    {
        CurCd += Time.deltaTime;
        if (CurCd >= dataVo.CD)
        {
            CurCd = 0f;
        }
    }
}



public enum SkillState
{
    None,
    Start,
    Execution,
    Interrupts,
    Finish
}

/// <summary>
/// 技能数据,配置中读取
/// </summary>
public class SkillDataVo
{
    public int ID;
    public string animName;
    public float CD;
    //技能打断，可配置该技能在XXX秒--XXX秒之间可被打断
    public float minInterrupt;
    public float maxInterrupt;

    public string effectName;
}