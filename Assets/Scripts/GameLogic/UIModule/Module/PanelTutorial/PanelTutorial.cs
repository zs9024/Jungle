using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PanelTutorial : BasePanel
{
    private TutorialPanelParams m_data;
    private Text m_content;
    private Text m_content2;
    private RectTransform m_tranLeft;
    private RectTransform m_tranRight;
    private RectTransform m_tranCenter;
    private RectTransform m_tranCancel;
    private RectTransform m_nebula;

    private Text m_txtLeft;
    private Text m_txtRight;
    private Text m_txtCenter;

    private RectTransform bgGround;
    private RectTransform mask;

    private GameObject line;
    private string m_strDefaultLeft;
    private string m_strDefaultRight;
    private string m_strDefaultCenter;

    private bool isMaskActive = false;

    private Vector3 orianlTextPos;
    public bool SetMaskActive { set{isMaskActive = value;}}

    public PanelTutorial()
    {
        SetPanelPrefabPath("");
    }

    public override void Init()
    {
        m_content = m_tran.Find("background/Text").GetComponent<Text>();
        m_content2 = m_tran.Find("background/Text2").GetComponent<Text>();
        m_tranLeft = m_tran.Find("background/btnLeft").GetComponent<RectTransform>();

        m_tranRight = m_tran.Find("background/btnRight").GetComponent<RectTransform>();
        m_tranCenter = m_tran.Find("background/btnCenter").GetComponent<RectTransform>();
        m_tranCancel = m_tran.Find("background/btnCancel").GetComponent<RectTransform>();
        m_txtLeft = m_tran.Find("background/btnLeft/Text").GetComponent<Text>();
        m_txtRight = m_tran.Find("background/btnRight/Text").GetComponent<Text>();
        m_txtCenter = m_tran.Find("background/btnCenter/Text").GetComponent<Text>();
        mask = m_tran.Find("Mask").gameObject.GetComponent<RectTransform>();
        line = m_tran.Find("background/line").gameObject;
        m_strDefaultLeft = m_txtLeft.text;
        m_strDefaultRight = m_txtRight.text;
        m_strDefaultCenter = m_txtCenter.text;
        m_nebula = m_tran.Find("background/Nebula").GetComponent<RectTransform>();
        bgGround = m_tran.Find("background").GetComponent<RectTransform>();

        orianlTextPos = m_content.transform.localPosition;
        //PlayActionManager.Instance.SetAction(m_go, m_go, EActionMode.LeftIn, true);
    }

    public override void InitEvent()
    {
        UGUIClickHandler.Get(m_tranLeft).onPointerClick += OnLeftClick;
        UGUIClickHandler.Get(m_tranRight).onPointerClick += OnRightClick;
        UGUIClickHandler.Get(m_tranCancel).onPointerClick += OnCancelClick;
        UGUIClickHandler.Get(m_tranCenter).onPointerClick += OnCenterClick;
        UGUIClickHandler.Get(mask).onPointerClick += OnMaskClick;
    }

    private void OnLeftClick(GameObject target, PointerEventData eventData)
    {
//         PlayActionManager.Instance.CloseAction(m_go, () =>
//         {
//             if (m_data == null)
//                 return;
//             if (m_data.onLeft != null)
//                 m_data.onLeft();
//             UIManager.HideTutorialPanel();
//         });

    }


    private void OnRightClick(GameObject target, PointerEventData eventData)
    {
//         PlayActionManager.Instance.CloseAction(m_go, () =>
//         {
//             if (m_data == null)
//                 return;
//             if (m_data.onRight != null)
//                 m_data.onRight();
//             UIManager.HideTutorialPanel();
//         });

    }
    private void OnCenterClick(GameObject target, PointerEventData eventData)
    {
//         PlayActionManager.Instance.CloseAction(m_go, () =>
//         {
//             if (m_data == null)
//                 return;
//             if (m_data.onCenter != null)
//                 m_data.onCenter();
//             UIManager.HideTutorialPanel();
//         });

    }

    private void OnMaskClick(GameObject target, PointerEventData eventData)
    {
        if (isMaskActive)
        {
//             PlayActionManager.Instance.CloseAction(m_go, () =>
//             {
//                 if (m_data == null)
//                     return;
//                 if (m_data.onCancel != null)
//                     m_data.onCancel();
//                 UIManager.HideTutorialPanel();
//             });
        }
    }

    private void OnCancelClick(GameObject target, PointerEventData eventdata)
    {
//         PlayActionManager.Instance.CloseAction(m_go, () =>
//         {
//             if (m_data == null)
//                 return;
//             if (m_data.onCancel != null)
//                 m_data.onCancel();
//             UIManager.HideTutorialPanel();
//         });

    }


    public override void OnShow()
    {
        if (m_params == null || m_params.Length == 0 || !(m_params[0] is TutorialPanelParams))
        {
            m_content.text = "";
            return;
        }
        
        m_data = m_params[0] as TutorialPanelParams;
        m_content.text = m_data.message;
        m_tranLeft.gameObject.SetActive(m_data.showDouble);
        m_tranRight.gameObject.SetActive(m_data.showDouble);
        m_tranCancel.gameObject.SetActive(m_data.showCancel);
        m_tranCenter.gameObject.SetActive(m_data.showCenter);
        m_nebula.gameObject.SetActive(m_data.showNebula);
        m_content2.gameObject.SetActive(m_data.showNebula);
        m_content.gameObject.SetActive(!m_data.showNebula);
        m_content2.text = m_data.message;
      
        line.SetActive(m_data.showDouble);
        bgGround.SetUILocation(bgGround.parent, m_tran.localPosition.x, m_data.yPos);
        m_txtLeft.text = m_data.LeftLabel ?? m_strDefaultLeft;
        m_txtRight.text = m_data.RightLabel ?? m_strDefaultRight;
        m_txtCenter.text = m_data.CenterLabel ?? m_strDefaultCenter;

        isMaskActive = m_data.isMaskActive;
        //PlayActionManager.Instance.PlayAction(m_go);
    }

    public override void OnHide()
    {
        m_params = null;
        m_data = null;
    }

    public override void OnBack()
    {
    }

    public override void OnDestroy()
    {
    }

    public override void Update()
    {
    }
}

public class TutorialPanelParams
{
    public string message;
    public Action onLeft;
    public Action onCancel;
    public Action onRight;
    public Action onCenter;
    public bool showDouble;
    public bool showCancel;
    public bool showCenter;
    public string LeftLabel;
    public string RightLabel;
    public string CenterLabel;
    public float yPos =0;
    public bool isMaskActive = false;
    public bool showNebula;
}