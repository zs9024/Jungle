// using System.Collections.Generic;
// using UnityEngine;
// public class Skill_Default : SkillBase
// {
// 
//     private bool isMove;
// 
//     SkillEzMoveData moveData;
// 
//     Vector3 moveDir;
// 
//     public Skill_Default(SpriteController obj, int skillID)
//         : base(obj, skillID)
//     {
//     }
// 
//     //遍历场景所有怪物
//     void ForALLEnmy()
//     {
//         Dictionary<long, SpriteController> dic = BattlerDataManager.Instance.DicMonsters;
//         //List<long> tmpList = new List<long>();
//         foreach (SpriteController controller in dic.Values)
//         {
//             if (controller.mAIState == AIState.Dead)
// 
//                 continue;
// 
//             //技能的伤害判断和处理
//             bool inforward = InForward(controller.transform, dataVo.attackFanAngle, dataVo.attackFanRange);
//             if (inforward)
//             {
//                 int damageValue = dataVo.harm;
//                 owner.AttackTo(controller, damageValue);
//                 Util.CallMethod("FightingPanel", "UpdateHeroHpMp", BattlerDataManager.Instance.SelfPlayer.SpiritVO.CurHp, BattlerDataManager.Instance.SelfPlayer.SpiritVO.CurMp);   //刷新角色血条
// 
//                 //            if (BattlerDataManager.Instance.SelfPlayer.mAIState  == AIState.Dead)
//                 //            {
//                 //                //玩家死亡
//                 //
//                 //            }
//                 if (BattlerDataManager.Instance.SelfPlayer.SpiritVO.CurHp <= 0)
//                 {
//                     //玩家死亡
//                     // Util.CallMethod("FightingPanel", "Lose");
//                 }
//             }
//         }
//     }
// 
// 
// 
//     public override void Execute()
//     {
// 
//         switch (State)
//         {
//             case SkillState.Start:
//                 moveDir = owner.MoveToPostion;
//                 timer = 0f;
//                 effectBeginTime = 0;
//                 owner.MoveMng.StopMove();
//                 isMove = false;
//                 // IfUseActionReturn = false;
//                 damaged = false;
//                 ifInitEffect = false;
//                 //执行
//                 owner.mAIState = AIState.Attack;
//                 owner.ActionMng.PlayAnimation(animalName);
//                 State = SkillState.Execution;
// 
//                 Debug.Log("skill ID " + dataVo.ID);
// 
//                 break;
//             case SkillState.Interrupts:
//                 {
//                     Debug.Log("被打断");
//                     State = SkillState.None;
//                 }
//                 break;
//             case SkillState.Execution:
//                 {
//                     timer += Time.deltaTime;
// 
//                     //触发伤害
//                     if (!damaged && timer > dataVo.damageTime)
//                     {
//                         damaged = true;
//                         //伤害结算
//                         ForALLEnmy();
//                     }
// 
//                     //触发特效
//                     if (!ifInitEffect && timer > dataVo.fxStartTime)
//                     {
//                         if (effect != null)
//                         {
//                             effect.transform.position = owner.transform.position + new Vector3(0, 0.5f, 0) + owner.transform.forward * 2;
//                             effect.transform.rotation = owner.transform.rotation;
//                             effect.SetActive(false);
//                             effect.SetActive(true);
//                         }
// 
//                         ifInitEffect = true;
//                     }
// 
//                     //动画播放结束，并且触发了特效，触发了伤害
//                     if (timer >= curAnimalLength && damaged && ifInitEffect)
//                     {
//                         State = SkillState.Finish;
//                     }
// 
//                 }
//                 break;
// 
//             case SkillState.Finish:
//                 {
//                     Debug.Log("Finish");
//                     if (effect != null)
//                         effect.SetActive(false);
//                     owner.mAIState = AIState.Idle;
//                     State = SkillState.None;
// 
//                 }
//                 break;
//         }
// 
//     }
// }