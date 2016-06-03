using UnityEngine;
using System.Collections;

/// <summary>
/// 动画状态控制类
/// </summary>
public class AnimStateController 
{
    private Animator animator;

    protected string transitionParam = "State";
    protected string blendParam = "Blend";

    public AnimStateController(Animator animator)
    {
        this.animator = animator;
    }

    public void PlayIdle(float blend = .0f)
    {
        SetBlendParam(blend);
        animator.SetInteger(transitionParam, GlobalConfig.AC_STATE_Idle);
    }

    public void PlayRun(float blend = .0f)
    {
        SetBlendParam(blend);
        animator.SetInteger(transitionParam, GlobalConfig.AC_STATE_Run);
    }

    public void PlayAtk(float blend = .0f)
    {
        SetBlendParam(blend);
        animator.SetInteger(transitionParam, GlobalConfig.AC_STATE_Atk);
    }

    public void PlayHit(float blend = .0f)
    {
        SetBlendParam(blend);
        animator.SetInteger(transitionParam, GlobalConfig.AC_STATE_Hit);
    }

    public void PlayDie(float blend = .0f)
    {
        SetBlendParam(blend);
        animator.SetInteger(transitionParam, GlobalConfig.AC_STATE_Die);
    }

    public void SetBlendParam(float blend)
    {
        animator.SetFloat(blendParam, blend);
    }

    public void StopPlay(string stateName, int layer = 0, string layerName = "Base Layer.")
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
        if(stateInfo.IsName(layerName + stateName))
        {
            float endTime = Mathf.Repeat(stateInfo.normalizedTime, 1.0f);
            if (endTime >= 0.9f)
            {
                PlayIdle();
            }
        }
    }
}
