using UnityEngine;
using System.Collections;

/// <summary>
/// 生物类，游戏中所有英雄，怪物的基类
/// </summary>
public abstract class Creature
{
    #region 生物属性
    public CreatureProp CreatureProperty { get { return creatureProp; } }
    protected CreatureProp creatureProp;
    #endregion

    #region 场景属性
    protected string assetName;

    public Transform trans;

    public GameObject gObj;

    protected string assetPath;

    protected object[] param;

    //血条
    public GameObject lifeBar;

    protected BT.BTTree bTree;
    #endregion
    public Creature()
    {
        Debug.Log("Creature actor...");
        //this.Create();
    }
    //protected abstract bool Create();

    protected void SetAssetPath(string path)
    {
        assetPath = path;
    }

    public void SetParams(object[] @params)
    {
        param = @params;
    }

    public void SetTransform(Vector3 pos,Vector3 scale, Quaternion rotation)
    {
        if(trans != null)
        {
            trans.position = pos;
            trans.localScale = scale;
            trans.rotation = rotation;
        }      
    }

    protected virtual void Init()
    {
        creatureProp = new CreatureProp();
    }

 

    public void Spwan(bool poolIn,System.Action onSpwan = null)
    {
        Init();

        Vector3 position = creatureProp.position.String2Vector3();
        //先从对象池取
        gObj = GameObjectPool.Spawn(assetName, null, position, Quaternion.identity);
        if (gObj == null)
        {
            ResourcesManager.Instance.LoadAssetsLocal(assetPath, (GameObject go) =>
            {
                if (poolIn)
                {
                    GameObjectPool.CreatePool(go, 3);
                }

                gObj = GameObjectPool.Spawn(go, null, position);
                
            });
        }

        GlobalDelegate.Instance.View.StartCoroutine(setProperties(gObj, onSpwan));

    }

    private IEnumerator setProperties(GameObject gObj, System.Action onSpwan = null)
    {
        yield return gObj;

        trans = gObj.transform;
        Transform life = trans.Find(creatureProp.lifeBar);
        lifeBar = UIManager.ShowLifebar(life, creatureProp.HP);

        if (onSpwan != null)
        {
            onSpwan();
        }

        OnShow();

        yield return null;
    }

    public void Dead(bool recycle = true)
    {
        if (recycle)
        {
            GameObjectPool.Recycle(gObj);
            GameObjectPool.Recycle(lifeBar);
        }
        else
        {

        }

        OnDead();
    }

    protected virtual void OnShow()
    {

    }


    protected virtual void OnDead()
    {

    }
}
