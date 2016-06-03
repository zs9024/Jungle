using UnityEngine;
using System.Collections;

public class CloudFlow : MonoBehaviour {

    public float m_SpeedU = 0.1f;


	// Update is called once per frame
	void Update () {
        float newOffsetU = Time.time * m_SpeedU;


        if (this.GetComponent<Renderer>())
        {
            GetComponent<Renderer>().material.mainTextureOffset = new Vector2(newOffsetU, 0);
        }
	}
}