using BT;
using Common.Event;
using Common.Global;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : Singleton<DamageManager>,IInit
{
    public void Init()
    {
        EventDispatcher.AddEventListener<Damage>(AnimEventConfig.OnDamage, onDamaged);
    }

    private void onDamaged(Damage damage)
    {
        Debug.Log("DamageManager onDamaged...");

        foreach(var sufferer in damage.Sufferers)
        {
            sufferer.CreatureProperty.HP -= damage.DamageValue;
            sufferer.lifeBar.GetComponent<LifeBar>().SetSlider(sufferer.CreatureProperty.HP);
            if (sufferer.CreatureProperty.HP <= 0)
            {
                Action onDeath = () =>
                {
                    deathFinal(sufferer);
                };

                Death death = new Death()
                {
                    decedent = sufferer,
                    callback = onDeath
                };

                //触发行为树中的死亡事件，播放死亡动画
                EventDispatcher.TriggerEvent<Death>(BTreeEventConfig.OnDeath, death);
            }
        }
        

    }

    /// <summary>
    /// 死亡后行为，回收或着销毁物体
    /// 如果是怪物，随机掉落物品
    /// </summary>
    /// <param name="death"></param>
    private void deathFinal(Creature death)
    {
        CreatureManager.Dead<Monster_Wolf>(death);

        //掉落 TODO...
    }
}

public class Damage
{
    public List<Creature> Sufferers;
    public GameObject Attacker;
    public int DamageValue;
}

/// <summary>
/// 动画帧事件
/// </summary>
public class AnimFramekEvent
{
    public float keyFrame;          //时间

    public Attack attack;
}

public class Attack
{
    public AttackMode attackMode;       //攻击类型
    public int attackValue;             //攻击值

    public List<Creature> targets;
    public GameObject attacker;
}

/// <summary>
/// 攻击类型
/// </summary>
public enum AttackMode
{
    None,
    Single,     //单体攻击
    Circular,   //圆形范围攻击
    Rectangle   //矩形范围攻击
}

public class Death
{
    public Creature decedent;
    public Action callback;
}

