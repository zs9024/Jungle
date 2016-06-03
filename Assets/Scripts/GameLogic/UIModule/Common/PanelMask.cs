using UnityEngine;
using UnityEngine.UI;
public class PanelMask : BasePanel
{
    private Text m_content;
    private Transform m_tranLoading;

    public PanelMask()
    {
        SetPanelPrefabPath("");
    }

    public override void Init()
    {
        m_content = m_tran.Find("Text").GetComponent<Text>();
        m_tranLoading = m_tran.Find("imgLoading");
    }

    public override void InitEvent()
    {
    }

    public override void OnShow()
    {
        if (!HasParams())
            m_content.text = "";
        else
            m_content.text = m_params[0].ToString();
    }

    public override void OnHide()
    {
        m_params = null;
    }

    public override void OnBack()
    {
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
        if (IsOpen())
        {
            m_tranLoading.Rotate(0, 0, -360*Time.deltaTime*2, Space.Self);
        }
    }
}