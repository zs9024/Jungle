using UnityEngine;
using System.Collections;

public class HeroCreator : CreatureCreator
{

    private HeroOption heroOpt;
    public HeroCreator(HeroOption ho)
    {
        this.heroOpt = ho;
    }

    public override Creature Create()
    {
        switch(heroOpt)
        {
            case HeroOption.JUGG:
                return new Hero_JUGG();
            default:
                break;
        }

        return null;
    }
	
}

public enum HeroOption
{
    None,
    JUGG,
}
