using UnityEngine;
using System.Collections;
using Common.Event;
using Common.Global;
using System.Collections.Generic;
using Global.Anim;

namespace BT.Ex
{
    /// <summary>
    /// 攻擊行為
    /// </summary>
    public class BTActionAttack : BTAction
    {


        private GameObject  _go;
        private Animator    _animator;
        private string      _animName;
        private int         _stateHash;


        private AnimEvent   _animEvent;
        private TrailWeapon _trail;

        private TrailEvent  _tarilEvent;
        private List<AnimFramekEvent> _keyAttacks;

        private float       _breakTime;
        private bool        _hasSetAnimEvent = false;
        private bool        _trailNeedStopExtra = false;

        public BTActionAttack(GameObject go, Animator animator, string animName, TrailEvent trailEvent)
        {
            _go             = go;
            _animator       = animator;
            _animName       = animName;
            _tarilEvent     = trailEvent;
            _stateHash      = Animator.StringToHash("Base Layer." + _animName);
        }

        public BTActionAttack(GameObject go, Animator animator, string animName, TrailEvent trailEvent, List<AnimFramekEvent> frameEvents)
            : this(go, animator, animName, trailEvent)
        {
            _keyAttacks = frameEvents;
        }

        public override void Activate(BTDatabase database)
        {
            base.Activate(database);

            _animEvent = _go.GetComponent<AnimEvent>();
            if (_animEvent == null)
            {
                _animEvent = _go.AddComponent<AnimEvent>();
            }

            _trail = _go.GetComponent<TrailWeapon>();
            if (_trail == null)
            {
                _trail = _go.AddComponent<TrailWeapon>();
                _trail.TrailObj = _go.transform.Find2("weapon_trail", false).GetComponent<WeaponTrail>();
            }

            //监测攻击完成事件
            EventDispatcher.AddEventListener<AnimatorStateInfo>(BTreeEventConfig.OnAnimationFinished, onAttackFinished);

            
        }

        protected override void Enter()
        {
            base.Enter();
        }

        protected override BTResult Execute()
        {
            addCurrentAnimAttackTrail();

            return BTResult.Running;
        }


        /// <summary>
        /// 给当前的攻击动画增加事件，实现WeaponTrail
        /// </summary>
        private void addCurrentAnimAttackTrail()
        {
            if(!_hasSetAnimEvent)
            {
                AnimatorStateInfo currentState = _animator.GetCurrentAnimatorStateInfo(0);
                if (currentState.nameHash != _stateHash)
                {
                    return;
                }

                AnimatorClipInfo[] info = _animator.GetCurrentAnimatorClipInfo(0);
                foreach (AnimatorClipInfo i in info)
                {
                    var clip = i.clip;
                    if (clip.name.Contains(_animName))
                    {
                        AddAnimEvent(currentState, clip);

                        _hasSetAnimEvent = true;
                    }
                }             
            }
        }

        protected virtual void AddAnimEvent(AnimatorStateInfo currentState, AnimationClip clip)
        {

            GlobalDelegate.Instance.View.StartCoroutine(playBreakpoint());
            _breakTime = Mathf.Repeat(currentState.normalizedTime, 1.0f);
            //Debug.Log("normalizedTime: " + _breakTime + " length: " + currentState.length);

            //加刀光
            _animEvent.AddAnimEvent(clip, _tarilEvent.StartTime, _trail.TrailStart);
            if (_tarilEvent.Times <=  1)
            {
                _animEvent.AddAnimEvent(clip, _tarilEvent.StopTime, _trail.TrailStop);
            }
            else
            {
                //一个动画循环播放多次，在最后停止
                _trailNeedStopExtra = true;
            }

            //加攻击事件
            foreach (var attackEv in _keyAttacks)
            {
                _animEvent.AddAnimEvent(attackEv.attack, clip, attackEv.keyFrame, attackEvent);
            }
        }

        private void attackEvent(object param)
        {
            Attack atk = param as Attack;
            if(atk == null)
            {
                return;
            }

            var cc = _go.GetComponent<CharacterController>();
            Damage damage = null;
            switch (atk.attackMode)
            {
                case AttackMode.Single:
                    var sufferer = AttackTargetLockOn.SingleLockOn(_go, 3f, 60, cc);
                    if (sufferer != null)
                    {
                        damage = GetDamage(new List<Creature> { sufferer }, atk.attackValue);
                    }
                    break;
                case AttackMode.Circular:
                    var creatures = AttackTargetLockOn.CircularLockOn(_go, 3f, cc);
                    if (creatures != null && creatures.Count > 0)
                    {
                        //构建伤害对象
                        damage = GetDamage(creatures, atk.attackValue);                       
                    }
                    break;
                case AttackMode.Rectangle:
                    break;
                default:
                    break;
            }

            if (damage != null)
            {
                //触发事件
                EventDispatcher.TriggerEvent<Damage>(AnimEventConfig.OnDamage, damage);
            }
        }


        public void SetAnimFramekEvent(float time, AttackMode mode, int value)
        {
            if (_keyAttacks == null)
            {
                _keyAttacks = new List<AnimFramekEvent>();
            }

            Attack atk = new Attack()
            {
                attackMode = mode,
                attackValue = value
            };

            _keyAttacks.Add(new AnimFramekEvent()
            {
                keyFrame = time,
                attack = atk
            });
        }

        public Damage GetDamage(List<Creature> targets, int value)
        {
            Damage damage = new Damage();
            damage.Sufferers = targets;
            damage.Attacker = _go;
            damage.DamageValue = value;

            return damage;
        }


        //坑爹，动态加动画事件的时候，播放到事件帧时，会跳过该transition
        //这里让它接着上次跳过的事件帧断点播放一次
        private IEnumerator playBreakpoint()
        {
            yield return null;
            _animator.CrossFade(_stateHash, 0.01f, 0, _breakTime - 2 * Time.deltaTime);           
        }

        private void onAttackFinished(AnimatorStateInfo animState)
        {
            if (!animState.IsName(_animName))
            {
                return;
            }

            if(_trailNeedStopExtra)
            {
                _trail.TrailStop();
            }

            var callback = _database.GetData<System.Action>("AtkCallback");
            if(callback != null)
            {
                callback();
            }
        }
    }
}

