using UnityEngine;
using System.Collections;
using Common.Event;
using Common.Global;
using Global.Anim;

namespace BT.Ex
{

    public class BTActionAnimTransition : BTActionPlayAnimation
    {
        private int _stateCode;
        private int _loopTime;          //动画循环播放次数，-1表示一直循环
        public BTActionAnimTransition(Animator animator, string stateName, int param,int loopTime = 1, string layerName = "Base Layer")
            : base(animator,stateName,layerName)
        {
            _stateCode = param;
            _loopTime = loopTime;
        }

        protected override void Enter()
        {
            //base.Enter();
            //通过animator controller 控制动画转换
            //_animator.SetTrigger(_paramName);
            _animator.SetInteger("State", _stateCode);
            AnimationConfig.SetAnimNameState(_animName, _stateCode);
        }

        protected override BTResult Execute()
        {
            if (_loopTime == -1)
            {
                return BTResult.Running;
            }

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.nameHash == _stateHash)
            {
                if(animState.normalizedTime < _loopTime - 1)
                {
                    return BTResult.Running;
                }
                float endTime = Mathf.Repeat(animState.normalizedTime, 1.0f);
                if (endTime >= 0.95f)
                {
                    Debug.Log("BTActionAnimTransition Success : " + _animName);
                    //触发动画结束事件
                    EventDispatcher.TriggerEvent(BTreeEventConfig.OnAnimationFinished, animState);
                    _animator.SetInteger("State", GlobalConfig.AC_STATE_Idle);

                    return BTResult.Success;
                }
            }

            return BTResult.Running;
        }

    }

}