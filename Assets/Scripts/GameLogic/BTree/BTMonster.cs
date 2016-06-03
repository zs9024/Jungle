using UnityEngine;
using System.Collections;
using BT;
using BT.Ex;

public class BTMonster : BTTree
{
    public Transform target;

    //移动速度
    public float speed = 1f;

    //领地范围，怪物只能在该范围内活动
    public float territoryDis = 6.0f;

    //防御范围，在这个范围内，怪物如果侦测到玩家，怪物会向玩家走近
    public float defendDis = 5.0f;

    //攻击距离，在这个距离怪物可以攻击
    public float tolerance = 1f;

    private Animator animator;
	public override BTNode Init()
    {
        base.Init();
        animator = GetComponent<Animator>();

        BTSelector root = new BTSelector();

        BTActionDeath death = new BTActionDeath(gameObject);
        root.AddChild(death);

        //领地限制的分支，只能在该范围活动
        BTConditionEvaluator evaluatorTerritory = new BTConditionEvaluator(BTLogic.And, false, BT.BTConditionEvaluator.ClearChildOpt.OnNotRunning);

        evaluatorTerritory.AddConditional(new BTCheckWithinDistance(_database.transform, territoryDis, null, BTCheckWithinDistance.DataOpt.FixedPosition), true);
        BTSimpleParallel goHome = new BTSimpleParallel();
        {
            goHome.SetPrimaryChild(new BTActionCharacterMove(_database.transform, speed, 0.1f, null,BTActionMove.DataOpt.FixedPosition,BTActionMove.UsageOpt.Position, BTDataReadOpt.ReadEveryTick));
            goHome.AddChild(new BTActionPlayAnimation(animator, "Walk"));
        }
        evaluatorTerritory.child = goHome;
        root.AddChild(evaluatorTerritory);


        BTConditionEvaluator evaluatorTarget = new BTConditionEvaluator(BTLogic.And, true, BT.BTConditionEvaluator.ClearChildOpt.OnNotRunning);
        evaluatorTarget.AddConditional(new BTCheckExistence(_database.transform, defendDis, "Player", BTCheckExistence.CheckOpt.CheckSphere));
        //加一层组合，来判断进行哪个动作
        BTSelector actionSelector = new BTSelector();
        evaluatorTarget.child = actionSelector;
        //评估是否有碰撞
        BTConditionEvaluator evaluatorCollision = new BTConditionEvaluator(BTLogic.And, true, BT.BTConditionEvaluator.ClearChildOpt.OnNotRunning);
        BTCheckCollision collision = new BTCheckCollision("Player", tolerance, BTCheckCollision.CheckOpt.RayCast);
        
        evaluatorCollision.AddConditional(collision);
        BTSequence attackSubtre = new BTSequence();
        {
            attackSubtre.AddChild(new BTActionPlayAnimation(animator, "Attack"));
        }
        evaluatorCollision.child = attackSubtre;
        actionSelector.AddChild(evaluatorCollision);

        //没有碰撞则移动
        BTSimpleParallel moveTo = new BTSimpleParallel();
        {
            moveTo.SetPrimaryChild(new BTActionCharacterMove(_database.transform, speed, tolerance, target, BTDataReadOpt.ReadEveryTick));
            moveTo.AddChild(new BTActionPlayAnimation(animator, "Walk"));
        }
        actionSelector.AddChild(moveTo);

        root.AddChild(evaluatorTarget);

        //Idle分支
        BTActionPlayAnimation palyIdle = new BTActionPlayAnimation(animator, "Idle");
        root.AddChild(palyIdle);

        return root;
    }


    private System.Action<Collider> TriggerEnter;
    private System.Action<Collider> TriggerExit;
    private System.Action<ControllerColliderHit> ControllerColliderHit;
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        if(TriggerEnter != null)
        {
            TriggerEnter(other);
        }
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay");
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
        if (TriggerExit != null)
        {
            TriggerExit(other);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (ControllerColliderHit != null)
        {
            ControllerColliderHit(hit);
        }
    }
}
