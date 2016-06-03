using UnityEngine;
using System.Collections;

public class PanelStart : BasePanel {

    private GameObject startBtn;

	public PanelStart()
    {
        SetPanelPrefabPath(ABConfig.PANEL_START);
    }

    public override void Init()
    {
        //这里获取组件
        startBtn = m_tran.Find("Button").gameObject;
    }

    public override void InitEvent()
    {
        //这里初始化事件
        UGUIClickHandler.Get(startBtn).onPointerClick += delegate { onClickStartBtn(); };
    }

    public override void OnShow()
    {

    }

    public override void OnBack()
    {
        
    }

    public override void OnHide()
    {
    }

    public override void OnDestroy()
    {
        UGUIClickHandler.Get(startBtn).RemoveAllHandler();
        Resources.UnloadUnusedAssets();
    }

    public override void Update()
    {

    }

    private void onClickStartBtn()
    {
        Debug.Log("PanelStart onClickStartBtn...");
        GlobalDelegate.Instance.View.Attach(() => { SceneManager.Instance.PushScene(SceneManager.Instance.CreateScene<SceneInterim>()); });
        UIManager.DestroyPanel<PanelStart>();
    }
}
