using UnityEngine;
using System.Collections;

public class ResTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ResourcesManager.Instance.LoadAssetsBundle(AssetConfig.CubeTest, true, (GameObject obj) =>
        {
            GameObjectPool.CreatePool(obj, 10);

            GameObjectPool.Spawn(obj);
        });
	}
	
}
