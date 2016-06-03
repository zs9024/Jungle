using UnityEngine;
using System.Collections;
using BT;
using BT.Ex;

public class BTCharacator : BTTree
{

    public float speed;
    private Animator animator;

    //攻击距离，在这个距离可以攻击
    public float tolerance = 2f;

    private static CharacterController characterController;
    public static CharacterController CharacController { get { return characterController; } }
    public override BTNode Init()
    {
        _useAnimEvent = true;

        base.Init();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        BTSelector root = new BTSelector();

        //键盘控制的分支
        BTConditionEvaluator evaluatorKey = new BTConditionEvaluator(BTLogic.And, true, BT.BTConditionEvaluator.ClearChildOpt.OnNotRunning);
        evaluatorKey.AddConditional(new BTCheckKeyOrTouch(BTCheckKeyOrTouch.ActionExecution.None));

        BTSimpleParallel translation = new BTSimpleParallel();
        {
            translation.SetPrimaryChild(new BTActionTranslate(_database.transform, speed, BTDataReadOpt.ReadEveryTick));
            //translation.AddChild(new BTActionPlayAnimation(animator, "run"));
            translation.AddChild(new BTActionAnimTransition(animator,"run",GlobalConfig.AC_STATE_Run,-1));
        }
        evaluatorKey.child = translation;
        root.AddChild(evaluatorKey);

        //easyJoystick控制的分支
        BTConditionEvaluator evaluatorJoystick = new BTConditionEvaluator(BTLogic.And, true, BT.BTConditionEvaluator.ClearChildOpt.OnNotRunning);
        evaluatorJoystick.AddConditional(new BTCheckEasyJoystick());
        evaluatorJoystick.child = translation;
        root.AddChild(evaluatorJoystick);

        //easytouch控制的分支
        BTConditionEvaluator evaluatorTouch = new BTConditionEvaluator(BTLogic.And, true, BT.BTConditionEvaluator.ClearChildOpt.OnNotRunning);
        evaluatorTouch.AddConditional(new BTCheckEasyTouch());
        evaluatorTouch.AddConditional(new BTCheckWithinDistance(_database.transform, 1f, "TargetPositon", BTCheckWithinDistance.DataOpt.TouchPosition), true);

        BTSimpleParallel pathFind = new BTSimpleParallel();
        {

            pathFind.SetPrimaryChild(new BTActionPathFind());
            pathFind.AddChild(new BTActionAnimTransition(animator, "run", GlobalConfig.AC_STATE_Run, -1));
        }
        evaluatorTouch.child = pathFind;

        root.AddChild(evaluatorTouch);


        //普通攻击的分支
        BTConditionEvaluator evaluatorAttack = new BTConditionEvaluator(BTLogic.And, true, BT.BTConditionEvaluator.ClearChildOpt.OnNotRunning);
        evaluatorAttack.AddConditional(new BTCheckAttack(BTCheckAttackOpt.Common));
        BTSimpleParallel catchSubtree = new BTSimpleParallel();
        {
            catchSubtree.SetPrimaryChild(new BTActionAnimTransition(animator, "attack", GlobalConfig.AC_STATE_Atk));
            var actionAttack = new BTActionAttack(gameObject, animator, "attack", new TrailEvent(0.35f, 0.51f));
            actionAttack.SetAnimFramekEvent(0.5f, AttackMode.Single, 100);
            catchSubtree.AddChild(actionAttack);
        }
        evaluatorAttack.child = catchSubtree;
        root.AddChild(evaluatorAttack);

        //技能攻击的分支
        BTConditionEvaluator evaluatorSkill = new BTConditionEvaluator(BTLogic.And, true, BT.BTConditionEvaluator.ClearChildOpt.OnNotRunning);
        evaluatorSkill.AddConditional(new BTCheckAttack(BTCheckAttackOpt.Skill_1));
        BTSimpleParallel skillSubtree = new BTSimpleParallel();
        {
            skillSubtree.SetPrimaryChild(new BTActionAnimTransition(animator, "attack_showoff", GlobalConfig.AC_STATE_JUGG_SKILL_Showoff));
            var skillAttack = new BTActionAttack(gameObject, animator, "attack_showoff", new TrailEvent(0.35f, 1.3f));
            skillAttack.SetAnimFramekEvent(0.6f, AttackMode.Circular, 150);
            skillAttack.SetAnimFramekEvent(1.0f, AttackMode.Circular, 150);
            skillSubtree.AddChild(skillAttack);
        }
        evaluatorSkill.child = skillSubtree;
        root.AddChild(evaluatorSkill);

        BTConditionEvaluator evaluatorSkill_2 = new BTConditionEvaluator(BTLogic.And, true, BT.BTConditionEvaluator.ClearChildOpt.OnNotRunning);
        evaluatorSkill_2.AddConditional(new BTCheckAttack(BTCheckAttackOpt.Skill_2));
        BTSimpleParallel skill_2Tree = new BTSimpleParallel();
        {
            skill_2Tree.SetPrimaryChild(new BTActionAnimTransition(animator, "attack_spin_odachi", GlobalConfig.AC_STATE_JUGG_SKILL_Spin,3));
            var skillAttack = new BTActionAttack(gameObject, animator, "attack_spin_odachi", new TrailEvent(0.31f, 1.2f,3));
            skillAttack.SetAnimFramekEvent(0.6f, AttackMode.Circular, 150);
            skillAttack.SetAnimFramekEvent(1.0f, AttackMode.Circular, 150);
            skill_2Tree.AddChild(skillAttack);
        }
        evaluatorSkill_2.child = skill_2Tree;
        root.AddChild(evaluatorSkill_2);



        //idle分支
        BTActionPlayAnimation palyIdle = new BTActionAnimTransition(animator, "idle", GlobalConfig.AC_STATE_Idle,-1);
        root.AddChild(palyIdle);

        return root;

    }

}
