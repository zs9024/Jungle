using UnityEngine;
using System.Collections;

public class MonsterCreator : CreatureCreator
{

    private MonsterOption monsterOpt;
    public MonsterCreator(MonsterOption mo)
    {
        this.monsterOpt = mo;
    }

    public override Creature Create()
    {
        switch (monsterOpt)
        {
            case MonsterOption.Wolf:
                return new Monster_Wolf();
            default:
                break;
        }

        return null;
    }

}

public enum MonsterOption
{
    None,
    Wolf,
}
