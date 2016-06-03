using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CorTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        StartCoroutine(countDown2(5)); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Text timeText;
    private IEnumerator countDown(int times)
    {

        timeText.text = times.ToString();

        for (; times >= 0;times-- )
        {
            yield return new WaitForSeconds(1f);
            timeText.text = times.ToString();
        }                    
    }

    private IEnumerator countDown2(int times)
    {

        timeText.text = times.ToString();

        while (times > 0)
        {
//             for (float timer = 0; timer < 1; timer += Time.deltaTime)
//                 yield return 0;
            yield return new WaitForSeconds(1f);

            --times;
            timeText.text = times.ToString();
        }
    }
}
