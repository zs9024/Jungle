using UnityEngine;
using System.Collections;

public class ResTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //ResourcesManager.Instance.LoadAssetsBundle(AssetConfig.CubeTest, true, (GameObject obj) =>
        //{
        //    GameObjectPool.CreatePool(obj, 10);

        //    GameObjectPool.Spawn(obj);
        //});
        //var pool = GameObjectPool.Instance;
        //GameObjectPool.CreateStartupPools();

        //var go = Resources.Load("Cube", typeof(GameObject)) as GameObject;
        ResourcesManager.Instance.LoadAssetsLocal("Cube", (GameObject obj) =>
        { 
            GameObjectPool.CreatePool(obj, 10);
            GameObjectPool.Spawn(obj);
        });
        //GameObjectPool.CreatePool(go, 10);
	}
	
}
