// using Common.Event;
// using Common.Global;
// using System.Collections.Generic;
// using UnityEngine;
// 
// namespace BT.Ex
// {
//     public class BTActionSkillAttack : BTActionAttack
//     {
//         //private List<AnimFramekEvent> _keyAttacks;
// 
//         public BTActionSkillAttack(GameObject gameObject, Animator animator, string animName,TrailEvent trailEvent)
//             : base(gameObject, animator, animName, trailEvent)
//         {
// 
//         }
// 
//         public override void Activate(BTDatabase database)
//         {
//             base.Activate(database);
// 
//             _keyAttacks = new List<AnimFramekEvent>();
//             Attack atk = new Attack()
//             {
//                 attackMode = AttackMode.Circular,
//                 attackValue = 150
//             };
// 
//             _keyAttacks.Add(new AnimFramekEvent()
//                 {
//                     keyFrame = 0.6f,
//                     attack = atk
//                 });
//             _keyAttacks.Add(new AnimFramekEvent()
//             {
//                 keyFrame = 1.0f,
//                 attack = atk
//             });
//         }
// 
//         protected override void AddAnimEvent(UnityEngine.AnimatorStateInfo currentState, UnityEngine.AnimationClip clip)
//         {
//             //加刀光
//             _animEvent.AddAnimEvent(clip, 0.35f, _trail.TrailStart);
//             _animEvent.AddAnimEvent(clip, 1.3f, _trail.TrailStop);
//             //加攻击事件
//             foreach (var attackEv in _keyAttacks)
//             {
//                 _animEvent.AddAnimEvent(attackEv.attack, clip, attackEv.keyFrame, attackEvent2);
//             }
//         }
// 
//         
// 
// 
//     }
// 
// 
// }
