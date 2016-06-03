using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PanelFloatTip : BasePanel
{
    private FloatTipPanelParams m_data;
    private RectTransform m_bgRect;
    private GameObject m_goBg;

    private Text m_content;
    private Text m_content2;
    private GameObject warnImg;
    private GameObject errorImg;
    private GameObject okImg;
    private GameObject curIcon;
    private GameObject nebula;

    private Vector3 orinalIconPos ;
    private Vector3 orinalTextPos;

    private uint timer=0;
    public PanelFloatTip()
    {
        SetPanelPrefabPath("");
    }

    public override void Init()
    {
        m_content = m_tran.Find("background/Text").GetComponent<Text>();
        m_content2 = m_tran.Find("background/Text2").GetComponent<Text>();
        m_bgRect = m_tran.Find("background").GetComponent<RectTransform>();
        m_goBg = m_bgRect.gameObject;
        warnImg = m_tran.Find("background/warn").gameObject;
        errorImg = m_tran.Find("background/error").gameObject;
        okImg = m_tran.Find("background/ok").gameObject;
        nebula = m_tran.Find("background/nebula").gameObject;

        orinalIconPos = warnImg.transform.localPosition;
        orinalTextPos = m_content.transform.localPosition;
        //PlayActionManager.Instance.SetAction(m_go, m_tran.Find("Mask").gameObject, EActionMode.FadeIn);
        //PlayActionManager.Instance.SetAction(m_go, m_goBg, EActionMode.LeftIn,true);
    }

    public override void InitEvent()
    {
    }

    public override void OnShow()
    {
        if (m_params == null || m_params.Length == 0 || !(m_params[0] is FloatTipPanelParams))
        {
            m_content.text = "";
            m_content2.text = "";
            HidePanel();
            return;
        }

        curIcon = warnImg;
        SetIconUnActive();
        m_data = m_params[0] as FloatTipPanelParams;
        if (m_data.style == ETipPanelStyle.NONE)
        {
            m_content.gameObject.SetActive(false);
            m_content2.gameObject.SetActive(true);
            m_content2.fontSize = 48;
            m_content2.text = m_data.message;
        }
        #region
        else
        {

            m_content.gameObject.SetActive(true);
            m_content2.gameObject.SetActive(false);
            m_content.fontSize = 48;
            m_content.text = m_data.message;

            if (m_data.style == ETipPanelStyle.STARBABY && nebula != null)
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

       
        float duration = m_data.duration;
        Vector2 from = m_data.from;
        Vector2 to = m_data.to;
//         if (from == default(Vector2)) from = Vector2.zero;
//         if (to == default(Vector2)) to = new Vector2(0, 200);
//         TweenPosition tweenPos = TweenPosition.Begin(m_goBg, duration, from, to);
//         tweenPos.anchoredPosition = true;
//         tweenPos.Play(true);

            
            //zhushi
//         PlayActionManager.Instance.PlayAction(m_go);
//         if (timer != 0)
//         {
//             GameLoader.Utils.Timer.TimerHeap.DelTimer(timer);
//             timer = 0;
//         }
// 
//         timer = GameLoader.Utils.Timer.TimerHeap.AddTimer((uint)(duration*1000f), 0, () =>
//         {
//             PlayActionManager.Instance.CloseAction(m_go,() =>
//             {
//                 HidePanel();
//             });
//         });
    }



    //根据字符长度，调整图标和字符位置
    void SetContentPos(string msg)
    {
        if (curIcon == null)
            return;

        int offsetCharCount = 0;

        offsetCharCount = (msg.Length <= 11) ? (msg.Length - 4) : 7;
        int offsetPos_X = m_content.fontSize * offsetCharCount;
        curIcon.transform.localPosition =  orinalIconPos - new Vector3(offsetPos_X, 0, 0) / 2f;
        m_content.transform.localPosition = orinalTextPos -  new Vector3(offsetPos_X, 0, 0) / 2f;
    }


    public override void OnHide()
    {
        if (m_data.hidePanelCallback != null)
        {
            m_data.hidePanelCallback();
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

public class FloatTipPanelParams
{
    public string message;
    public float duration;
    public Vector2 from;
    public Vector2 to;
    public ETipPanelStyle style;
    public Action hidePanelCallback;
}


public enum ETipPanelStyle
{
    NONE,
    ERROR,
    SUCCESS,
    WARN,
    STARBABY,
}