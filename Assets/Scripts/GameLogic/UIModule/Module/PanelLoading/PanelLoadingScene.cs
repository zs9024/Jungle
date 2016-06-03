using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelLoadingScene : BasePanel
{
    private Slider slider;
    private Text progressText;

    public PanelLoadingScene()
    {
        SetPanelPrefabPath(ABConfig.PANEL_LOADINGSCENE);
    }

    public override void Init()
    {
        //这里获取组件
        slider = m_tran.Find("Progress/Slider").GetComponent<Slider>();
        progressText = m_tran.Find("Progress/Percent").GetComponent<Text>();
    }

    public override void InitEvent()
    {
        //这里初始化事件
        
    }

    public override void OnShow()
    {
        slider.interactable = false;    //禁用交互

        SceneMain sceneMain = SceneManager.Instance.CreateScene<SceneMain>();
        sceneMain.SetLoadingProgress(setLoadingProgress);
        SceneManager.Instance.PushScene(sceneMain);
    }

    private void setLoadingProgress(int percent)
    {
        progressText.text = percent.ToString() + "%";
        float tmp = (float)((float)percent / 100.0);
        slider.value = tmp;
    }

    public override void OnBack()
    {

    }

    public override void OnHide()
    {
    }

    public override void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }

    public override void Update()
    {

    }
}
