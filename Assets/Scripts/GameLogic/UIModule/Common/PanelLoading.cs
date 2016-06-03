//*************************************************************************
//	创建日期:	2015/12/8 21:09:04
//	文件名称:	PanelLoading 
//  创建作者:	Potter 	
//	版权所有:	CIFPAY.CN
//	相关说明:	
//*************************************************************************

using UnityEngine;
using System.Collections.Generic;
public class PanelLoading : BasePanel
{
    private GameObject m_goCancelBtn;
    private float m_fLoadingTime = 0;
    private readonly float DELAY_TIME = 0.5f;
    private bool m_isShow = false;

    //六边形透明
    private List<CanvasGroup> m_Six = new List<CanvasGroup>();                //当前六边形
    private List<int> m_speet = new List<int>();                            //当前速度
    private List<float> m_alpha = new List<float>();                            //当前透明

    public PanelLoading()
    {
        SetPanelPrefabPath("");
    }

    public override void Init()
    {
        m_tran.Find("BackBtn", ref m_goCancelBtn);
        AddSixFunc();
        
        //PlayActionManager.Instance.SetAction(m_go, m_tran.Find("BG").gameObject, EActionMode.FadeIn);
        //PlayActionManager.Instance.SetAction(m_go, m_tran.Find("Contain").gameObject, EActionMode.FadeIn);        
    }

    public override void InitEvent()
    {
        if (m_goCancelBtn != null)
        {
            UGUIClickHandler.Get(m_goCancelBtn).onPointerClick += OnCancelBtn;
        }
    }

    private void OnCancelBtn(GameObject target, UnityEngine.EventSystems.PointerEventData eventData)
    {   
        //停止网络请求
        //CommunicatorManager.Instance.Stop();
    }

    public override void OnShow()
    {
        base.Alpha = 0;
        m_fLoadingTime = 0;
        m_isShow = true;
    }

    public override void OnHide()
    {
        m_isShow = false;
        //PlayActionManager.Instance.CloseAction(m_go);
    }

    public override void OnBack()
    {
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
        if(m_isShow)
        {
            m_fLoadingTime += 0.02f;
            base.Alpha = Mathf.Clamp(-0.3f + m_fLoadingTime * 2, 0 , 1);
            if(m_fLoadingTime >= DELAY_TIME)
            {                
                m_isShow = false;
                //PlayActionManager.Instance.PlayAction(m_go);
                base.Alpha = 1;
            }
        }

        //改六边形透明度
        ChangeAlpha();
    }

    private void PlayAnimation()
    {

    }

    //添加六边形方法
    private void AddSixFunc()
    {
        if (m_Six.Count == 0)
        {
            GameObject go1 = m_tran.Find("Contain/Icon1").gameObject;
            GameObject go2 = m_tran.Find("Contain/Icon2").gameObject;
            GameObject go3 = m_tran.Find("Contain/Icon3").gameObject;
            GameObject go4 = m_tran.Find("Contain/Icon4").gameObject;
            GameObject go5 = m_tran.Find("Contain/Icon5").gameObject;

            CanvasGroup Mc1 = go1.GetComponent<CanvasGroup>();
            CanvasGroup Mc2 = go2.GetComponent<CanvasGroup>();
            CanvasGroup Mc3 = go3.GetComponent<CanvasGroup>();
            CanvasGroup Mc4 = go4.GetComponent<CanvasGroup>();
            CanvasGroup Mc5 = go5.GetComponent<CanvasGroup>();

            m_speet.Add(1);
            m_alpha.Add(0.8f);
            m_Six.Add(Mc1);

            m_speet.Add(1);
            m_alpha.Add(0.7f);
            m_Six.Add(Mc2);

            m_speet.Add(1);
            m_alpha.Add(0.6f);
            m_Six.Add(Mc3);

            m_speet.Add(1);
            m_alpha.Add(0.5f);
            m_Six.Add(Mc4);

            m_speet.Add(1);
            m_alpha.Add(0.4f);
            m_Six.Add(Mc5);
        }
    }

    private void ChangeAlpha()
    {
        for (int i = 0; i < m_alpha.Count; i++)
        {
            m_alpha[i] += (0.04f * m_speet[i]);
            m_Six[i].alpha = m_alpha[i];
            if (m_alpha[i] < 0.3f)
            {
                m_alpha[i] = 0.3f;
                m_speet[i] *= -1;
            }
            else if (m_alpha[i] > 1)
            {
                m_alpha[i] = 1f;
                m_speet[i] *= -1;
            }
        }
    }
}
