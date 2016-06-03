using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{

    private RectTransform rectTransform;
    private Slider slider;
    public int TotalHP;

    public Transform target;
    public Camera targetCam;
    public Camera uiCam;

    void Start()
    {
        //获取血条的 RectTransform 组件
        rectTransform = GetComponent<RectTransform>();
        slider = GetComponent<Slider>();
        slider.value = 1f;
        _zpos = target.position.z;

        if(targetCam == null)
        {
            targetCam = Hero.thirdCam;
        }
    }

    private float _zpos;
    void LateUpdate()
    {

        if (target == null || rectTransform == null)
        {
            return;
        }

        //将角色的3D世界坐标转换为 屏幕坐标
        Vector3 targetScreenPosition = targetCam.WorldToScreenPoint(target.position);
        //Debug.Log("targetScreenPosition ： " + targetScreenPosition + "target.position" + target.position);

        //定义一个接收转换为 UI  2D 世界坐标的变量
        Vector3 followPosition;

        // 使用下面方法转换
        // RectTransformUtility.ScreenPointToWorldPointInRectangle（）
        // 参数1 血条的 RectTransform 组件；
        // 参数2 角色坐标转换的屏幕坐标
        // 参数3 目标摄像机，Canvas的 Render Mode 参数类型设置为 Screen Space - Camera时需要写摄像机参数
        //        本例 Canvas的 Render Mode 参数类型设置为 Screen Space - Overlay，在此将第三个参数设置为 null
        // 参数4 接收转换后的坐标，需要提前声明一个 Vector3 参数
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, targetScreenPosition, uiCam, out followPosition))
        {
            //将血条的坐标设置为 UI 2D 世界坐标
//             float posz = 2 * target.position.z;// -_zpos;
//             transform.position = new Vector3(followPosition.x, followPosition.y, posz);
//             _zpos = target.position.z;
            //transform.position = followPosition;

            rectTransform.anchoredPosition3D = new Vector3(followPosition.x,followPosition.y,targetScreenPosition.z * 30);
        }
    }

    public void SetSlider(int currentHP)
    {
        slider.value = (float)currentHP / (float)TotalHP;
    }
}