using UnityEngine;
using System.Collections;

public class Fps : MonoBehaviour
{

    /// <summary>
    /// 单位时间内更新的帧数
    /// </summary>
    int fpscount = 0;

    /// <summary>
    /// 单位时间的帧率
    /// </summary>
    float FpsSpeed = 0;

    /// <summary>
    /// 单位时间
    /// </summary>
    float ontime = 1f;


    /// <summary>
    /// 更新单位时间的帧数,计算帧率
    /// </summary>
    void Update()
    {
        fpscount++;
        ontime -= Time.deltaTime;

        if (ontime <= 0)
        {
            FpsSpeed = fpscount / 1f;
            fpscount = 0;
            ontime = 1f;
        }
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 20;

//         if (Logger.LOG_LEVEL >= LogLevel.DEBUG)
//         {
//             GUI.Label(new Rect(10, Screen.height - 50, 200, 100), "Fps=" + FpsSpeed.ToString());
//         }
        GUI.Label(new Rect(10, Screen.height - 30, 200, 100), "Fps=" + FpsSpeed.ToString());
    }
}

