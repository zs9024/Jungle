using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelTip : BasePanel
{
    private TipPanelParams m_data;
    private Text m_content;
    private Text m_content2;
    private RectTransform m_tranLeft;
    private RectTransform m_tranCancel;
    private RectTransform m_tranRight;
    private RectTransform m_tranCenter;

    private Text m_txtLeft;
    private Text m_txtRight;
    private Text m_txtCenter;

    private string m_strDefaultLeft;
    private string m_strDefaultRight;
    private string m_strDefaultCenter;

    private GameObject warnImg;
    private GameObject errorImg;
    private GameObject okImg;
    private GameObject nebula;
    private GameObject line;

    private Vector3 orinalIconPos;
    private Vector3 orinalTextPos;

    private bool isMaskActive = false;

    private GameObject mask;
    GameObject curIcon;
    public PanelTip()
    {
        SetPanelPrefabPath("");
    }

    public override void Init()
    {
        m_content = m_tran.Find("background/Text").GetComponent<Text>();
        m_content2 = m_tran.Find("background/Text2").GetComponent<Text>();
        m_tranLeft = m_tran.Find("background/btnLeft").GetComponent<RectTransform>();
        m_tranCancel = m_tran.Find("background/btnCancel").GetComponent<RectTransform>();
        m_tranRight = m_tran.Find("background/btnRight").GetComponent<RectTransform>();
        m_tranCenter = m_tran.Find("background/btnCenter").GetComponent<RectTransform>();

        m_txtLeft = m_tran.Find("background/btnLeft/Text").GetComponent<Text>();
        m_txtRight = m_tran.Find("background/btnRight/Text").GetComponent<Text>();
        m_txtCenter = m_tran.Find("background/btnCenter/Text").GetComponent<Text>();
        m_strDefaultLeft = m_txtLeft.text;
        m_strDefaultRight = m_txtRight.text;
        m_strDefaultCenter = m_txtCenter.text;
        warnImg = m_tran.Find("background/warn").gameObject;
        errorImg = m_tran.Find("background/error").gameObject;
        okImg = m_tran.Find("background/ok").gameObject;

        line = m_tran.Find("background/line").gameObject;
        orinalIconPos = warnImg.transform.localPosition;
        orinalTextPos = m_content.transform.localPosition;
        mask = m_tran.Find("Mask").gameObject;
       
        //PlayActionManager.Instance.SetAction(m_go,m_tran.Find("background").gameObject,EActionMode.LeftIn,true);
        //PlayActionManager.Instance.SetAction(m_go, m_tran.Find("Mask").gameObject, EActionMode.FadeIn);
    }

    public override void InitEvent()
    {
        UGUIClickHandler.Get(m_tranLeft).onPointerClick += OnLeftBtnClick;
        UGUIClickHandler.Get(m_tranCancel).onPointerClick += OnCancelClick;
        UGUIClickHandler.Get(m_tranRight).onPointerClick += OnRightBtnClick;
        UGUIClickHandler.Get(m_tranCenter).onPointerClick += OnCenterBtnClick;
        UGUIClickHandler.Get(mask).onPointerClick += OnMaskClick;
        UGUIClickHandler.Get(m_tran.Find("background").gameObject).onPointerClick += OnMaskClick;
    }

    private void OnLeftBtnClick(GameObject target, PointerEventData eventData)
    {
//         PlayActionManager.Instance.CloseAction(m_go, () =>
//         {
//             if (m_data == null)
//                 return;
//             if (m_data.onLeft != null)
//                 m_data.onLeft();
//             UIManager.HideTipPanel();
//         });
       
    }

    private void OnCancelClick(GameObject target, PointerEventData eventdata)
    {
//         PlayActionManager.Instance.CloseAction(m_go, () =>
//         {
//             if (m_data == null)
//                 return;
//             if (m_data.onCancel != null)
//                 m_data.onCancel();
//             UIManager.HideTipPanel();
//         });
       
    }

    private void OnRightBtnClick(GameObject target, PointerEventData eventdata)
    {
//         PlayActionManager.Instance.CloseAction(m_go, () =>
//         {
//            
//             if (m_data == null)
//                 return;
//             if (m_data.onRight != null)
//                 m_data.onRight();
//             UIManager.HideTipPanel();
//         });
    }


    private void OnCenterBtnClick(GameObject target, PointerEventData eventdata)
    {
//         PlayActionManager.Instance.CloseAction(m_go, () =>
//         {
//             if (m_data == null)
//                 return;
//             if (m_data.onCenter != null)
//                 m_data.onCenter();
//             UIManager.HideTipPanel();
//         });
    }


    private void OnMaskClick(GameObject target, PointerEventData eventdata)
    {
        if (isMaskActive)
        {
//             PlayActionManager.Instance.CloseAction(m_go, () =>
//             {
//                 UIManager.HideTipPanel();
//             });
        }
    }

    public override void OnShow()
    {
        if (m_params == null || m_params.Length == 0 || !(m_params[0] is TipPanelParams))
        {
            m_content.text = "";
            return;
        }

        m_data = m_params[0] as TipPanelParams;
        isMaskActive = m_data.isMaskActive;

        curIcon = warnImg;
        SetIconUnActive();
        if (m_data.style == ETipPanelStyle.NONE)
        {
            m_content.gameObject.SetActive(false);
            m_content2.gameObject.SetActive(true);
            m_content2.text = m_data.message;
        }
        #region
        else
        {
            m_content.gameObject.SetActive(true);
            m_content2.gameObject.SetActive(false);
            m_content.text = m_data.message;
            m_tranCancel.gameObject.SetActive(m_data.showCancel);

            m_tranLeft.gameObject.SetActive(m_data.showDoubleBtn);
            m_tranRight.gameObject.SetActive(m_data.showDoubleBtn);

            m_tranCenter.gameObject.SetActive(m_data.showCenter);
            line.SetActive(m_data.showDoubleBtn);
            nebula = m_tran.Find("background/nebula").gameObject;

            m_txtLeft.text = m_data.leftLabel ?? m_strDefaultLeft;
            m_txtRight.text = m_data.rightLabel ?? m_strDefaultRight;
            m_txtCenter.text = m_data.centerLabel ?? m_strDefaultCenter;

            if( m_data.style == ETipPanelStyle.STARBABY && nebula != null)
            {
                nebula.SetActive(true);
                curIcon = nebula;
            }
            else
            {
                if (m_data.style == ETipPanelStyle.WARN && warnImg != null)
                {
                    warnImg.SetActive(true);
                }
                else if (m_data.style == ETipPanelStyle.ERROR && errorImg != null)
                {
                    errorImg.SetActive(true);
                    curIcon = errorImg;
                }
                else if (m_data.style == ETipPanelStyle.SUCCESS && okImg != null)
                {
                    okImg.SetActive(true);
                    curIcon = okImg;
                }

                else if (warnImg != null)
                {
                    warnImg.SetActive(true);
                }

                SetContentPos(m_content.text);
            }
            
        }
        #endregion


        //PlayActionManager.Instance.PlayAction(m_go);
    }


    //根据字符长度，调整图标和字符位置
    void SetContentPos(string msg)
    {
        if (curIcon == null)
            return;

        int offsetCharCount = 0;

        offsetCharCount = (msg.Length <= 11) ? (msg.Length - 4) : 7;
        int offsetPos_X = m_content.fontSize * offsetCharCount;
        curIcon.transform.localPosition = orinalIconPos - new Vector3(offsetPos_X, 0, 0) / 2f;
        m_content.transform.localPosition = orinalTextPos - new Vector3(offsetPos_X, 0, 0) / 2f;
    }

    public override void OnHide()
    {
        if (m_data != null && m_data.hideCallBack != null)
        {
            m_data.hideCallBack();
        }

        m_params = null;
        m_data = null;
        SetIconUnActive();
    }

    void SetIconUnActive()
    {
        if (curIcon != null)
        {
            curIcon.SetActive(false);
        }
    }



    public override void OnBack()
    {
        SetIconUnActive();
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
    }
}

public class TipPanelParams
{
    public string message;
    public Action onLeft;
    public Action onCancel;
    public Action onCenter;
    public Action onRight;
    public Action hideCallBack;
    public bool showCancel;
    public bool showCenter;
    public bool showDoubleBtn;
    public string leftLabel;
    public string rightLabel;
    public string centerLabel;
    public ETipPanelStyle style;
    public bool isMaskActive;
}

