using System.Collections.Generic;
using UnityEngine;

public static class CreatureManager
{
    //所有生成的生物
    private static Dictionary<string, List<Creature>> creatureDic = new Dictionary<string,List<Creature>>();

    public static List<Monster2> MonsterDic { get { return monsterDic; } }
    private static List<Monster2> monsterDic = new List<Monster2>();

    public static T Spwan<T>(object[] @params = null) where T : Creature, new()
    {
        string creatureName = typeof(T).Name;       //拿到生物类名
        Creature creature = new T();
        creature.SetParams(@params);

        if(!creatureDic.ContainsKey(creatureName))
        {
            creatureDic.Add(creatureName, new List<Creature>());
        }
        creatureDic[creatureName].Add(creature);

        //存怪物對象
        if (typeof(Monster2).IsAssignableFrom(typeof(T)))
        {
            Debug.Log("IsAssignableFrom...");
            Monster2 monster = creature as Monster2;
            if (!monsterDic.Contains(monster))
            {
                monsterDic.Add(monster);
            }
        }

        creature.Spwan(true);

        return creature as T;
    }

    public static void Dead<T>(Creature decedent) where T : Creature
    {
        if(decedent == null)
        {
            return;
        }

        string creatureName = typeof(T).Name; 
        if(creatureDic.ContainsKey(creatureName))
        {
            var creatures = creatureDic[creatureName];
            creatures.Remove(decedent);
        }

        if (typeof(Monster2).IsAssignableFrom(typeof(T)))
        {
            Monster2 monster = decedent as Monster2;
            monsterDic.Remove(monster);
        }

        decedent.Dead();
    }
}

