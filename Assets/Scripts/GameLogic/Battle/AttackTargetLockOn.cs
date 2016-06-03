using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackTargetLockOn 
{

    /// <summary>
    /// 单个对象锁定，锁距离最近并在扇形攻击范围内的
    /// </summary>
    /// <param name="attacker">攻击者</param>
    /// <returns></returns>
    public static Creature SingleLockOn(GameObject attacker,float dis,float theta,CharacterController cc = null)
    {
        Creature creature = FindClosestMonster(attacker);
        if(creature == null)
        {
            return null;
        }

        Vector3 pos;
        if(cc != null)
        {
            pos = new Vector3(attacker.transform.position.x, attacker.transform.position.y + cc.center.y, attacker.transform.position.z);            
        }
        else
        {
            pos = attacker.transform.position;
        }
        Vector3 end = pos + attacker.transform.TransformDirection(Vector3.back) * dis;
        Debug.DrawLine(pos, end, Color.red, dis);

        if (RangeDetection.IsPointInCircularSector(pos, end, dis, theta, creature.trans.position))
        {
            return creature;
        }

        return null;
    }

    /// <summary>
    /// 查找距离攻击者最近的生物
    /// </summary>
    /// <param name="attacker"></param>
    /// <returns></returns>
    public static Creature FindClosestMonster(GameObject attacker)
    {
        Dictionary<float, Creature> distances = new Dictionary<float, Creature>();
        foreach (Monster2 monster in CreatureManager.MonsterDic)
        {
            float dis = Vector3.Distance(monster.trans.position, attacker.transform.position);
            if (!distances.ContainsKey(dis))
            {
                distances.Add(dis, monster);
            }
        }

        List<float> disList = new List<float>(distances.Keys);
        disList.Sort();

        if(disList.Count <= 0)
        {
            return null;
        }

        return distances[disList[0]];
    }

    /// <summary>
    /// 锁定一定半径内圆形的所有攻击对象
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static List<Creature> CircularLockOn(GameObject attacker, float radius, CharacterController cc = null)
    {
        List<Creature> creatures = new List<Creature>();
        Vector3 pos;
        if (cc != null)
        {
            pos = new Vector3(attacker.transform.position.x, attacker.transform.position.y + cc.center.y, attacker.transform.position.z);
        }
        else
        {
            pos = attacker.transform.position;
        }

        foreach (Monster2 monster in CreatureManager.MonsterDic)
        {
            if (RangeDetection.IsPointInCircular(monster.trans.position, radius, pos))
            {
                creatures.Add(monster);
            }           
        }

        return creatures;
    }

}
