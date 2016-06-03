using UnityEngine;
using System.Collections;

public class CopyTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private string text = "好风景就和五河无尽";
    void CopyText()
    {
        System.Type T = typeof(GUIUtility);
        System.Reflection.PropertyInfo systemCopyBufferProperty = T.GetProperty("systemCopyBuffer", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        systemCopyBufferProperty.SetValue(null, text, null);

//         TextEditor te = new TextEditor();
//         te.content = new GUIContent(text);
//         te.SelectAll();
//         te.Copy();
    }

    void OnGUI()
    {
        if(GUI.Button(new Rect(50,50,100,100),"Copy"))
        {
            CopyText();
        }


    }
}
