using UnityEngine;
using System.Collections;

public class UITest : MonoBehaviour {

    Rect rect = new Rect(0, 0, 200, 200);
    private GameObject btn;
    private RectTransform rectTrans;

    public Camera uicam;
	// Use this for initialization
	void Start () {
        btn = transform.Find("Button").gameObject;
        rectTrans = btn.GetRectTransform();

        UGUIClickHandler.Get(btn).onPointerClick += delegate { Debug.Log("on button..."); };

       
//         float rate = (float)Screen.width / (float)Screen.currentResolution.width;
//         Vector2 screenPos = rectTrans.anchoredPosition * rate;
//         Vector2 size = rectTrans.sizeDelta * rate * 1.2f;
// 
// 
//         Vector3 pos = rectTrans.position;
//         pos = uicam.WorldToScreenPoint(pos);
// 
//         rect = new Rect(pos.x - size.x / 2, pos.y - size.y / 2  , size.x , size.y );
// 
//         //EasyTouch.AddReservedGuiArea(rect);
//         EasyTouch.AddReservedArea(rect);
//         Debug.Log(rect + "dpi ： " + pos);
//         Debug.Log("width: " + Screen.width + "height: " + Screen.height + "currentResolution : " + Screen.currentResolution.height	
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if(GUI.Button(rect,"Test"))
        {
            Debug.Log("Button...");
        }
    }
}
