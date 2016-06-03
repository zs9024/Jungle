
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum LayerType
{
    Root,
    Content,
    TV,
    POP,
    Chat,
    Laba,
    Tip,
    Loading
}

/// <summary>
/// 封装界面弹窗逻辑的工具类
/// </summary>
public static class UIManager
{
    public static Camera UICamera;
    public static Vector2 Resolution;
    public static RectTransform RootCanvas;
    private static RectTransform m_rootLayer;
    private static RectTransform m_contentLayer;
    //private static RectTransform m_tvLayer;
    private static RectTransform m_popLayer;
    //private static RectTransform m_chatLayer;
    //private static RectTransform m_labaLayer;
    private static RectTransform m_tipLayer; 
    private static RectTransform m_loadingLayer;
    private static Dictionary<string, BasePanel> m_panelDic = new Dictionary<string, BasePanel>();
    private static List<BasePanel> m_panelLoopList = new List<BasePanel>();                     //遍历用的列表
    private static Stack<string> m_layerNameStack = new Stack<string>();                        //记录弹窗的层次路径
    private static Stack<object[]> m_layerParamsStack = new Stack<object[]>();                  //记录弹窗的参数设置

    private static BasePanel m_preContentPanel;     //上一个显示的主Panel
    private static BasePanel m_contentPanel;        //当前正在显示的主Panel
    //private static BasePanel m_popPanel;          //当前正弹出的popPanel
    //private static BasePanel m_tvPanel;           //当前正在显示的电视Panel
    //private static BasePanel m_labaPanel;         //当前正在显示的喇叭Panel
    private static BasePanel m_tipPanel;            //当前正弹出的tipPanel
    private static BasePanel m_floatTipPanel;       //当前正弹出的floatTipPanel
    private static BasePanel m_maskPanel;           //当前正弹出的maskPanel
    private static BasePanel m_tutorialPanel;       //阶段性引导面板
    private static BasePanel m_venusPanel;          //摘星结果提示面板

    private static PanelLoading m_loadingPanel;     //当前正弹出的loadingPanel

    public static Camera UICamera3D;                //显示3d UI
    public static RectTransform Canvas3D;

    public static RectTransform CanvasOverlay;

    #region UI系统的初始化
    public static void Init()
    {
        m_tipLayer = null;
        m_loadingLayer = null;

        Resolution = new Vector2(1280, 720);

        //创建EventSystem
        EventSystem eventSystem = UnityEngine.Object.FindObjectOfType(typeof(EventSystem)) as EventSystem;
        if(eventSystem == null)
        {
            var eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.AddComponent<StandaloneInputModule>();
            eventSystemGo.AddComponent<TouchInputModule>();
            eventSystem = eventSystemGo.GetComponent<EventSystem>();
            eventSystem.sendNavigationEvents = false;
            eventSystem.pixelDragThreshold = 8;
            GameObject.DontDestroyOnLoad(eventSystemGo);
        }

        //创建UI摄像机
        UICamera = new GameObject("UICamera").AddComponent<Camera>();
        UICamera.clearFlags = CameraClearFlags.Depth;
        UICamera.backgroundColor = Color.black;
        UICamera.cullingMask = GlobalConfig.LAYER_MASK_UI;
        UICamera.orthographic = true;
        UICamera.orthographicSize = 10;
        UICamera.nearClipPlane = -100;
        UICamera.farClipPlane = 100;
        UICamera.depth = 10;
        GameObject.DontDestroyOnLoad(UICamera.gameObject);

        //创建UI的Canvas，模式ScreenSpaceCamera
        var rootCanvasGo = new GameObject("UGUIRootCanvas");
        rootCanvasGo.layer = LayerMask.NameToLayer("UI");
        var canvas = rootCanvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = UICamera;
        canvas.pixelPerfect = false;
        canvas.planeDistance = 0;
        var scaler = rootCanvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = Resolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1;
        rootCanvasGo.AddComponent<GraphicRaycaster>();
        GameObject.DontDestroyOnLoad(rootCanvasGo);
        RootCanvas = rootCanvasGo.GetRectTransform(); 

        //创建渲染3dUI的摄像机
        UICamera3D = new GameObject("UICamera3D").AddComponent<Camera>();
        UICamera3D.clearFlags = CameraClearFlags.Depth;
        UICamera3D.cullingMask = GlobalConfig.LAYER_MASK_3DUI;
        UICamera3D.orthographic = false;
        UICamera3D.fieldOfView = 40;
        UICamera3D.depth = 11;
        UICamera3D.enabled = true;
        UICamera3D.transform.position = new Vector3(0, 0, -100);
        GameObject.DontDestroyOnLoad(UICamera3D.gameObject);

        //创建3dUI的Canvas
        var canvas3DGo = new GameObject("Canvas3D");
        canvas3DGo.layer = LayerMask.NameToLayer("3DUI");
        var canvas3D = canvas3DGo.AddComponent<Canvas>();
        canvas3D.renderMode = RenderMode.WorldSpace; 
        canvas3D.worldCamera = UICamera3D;
        var scaler3D = canvas3DGo.AddComponent<CanvasScaler>();
        canvas3DGo.AddComponent<GraphicRaycaster>();
        Canvas3D = canvas3DGo.GetRectTransform();
        GameObject.DontDestroyOnLoad(canvas3DGo);

        //创建最上层画布
        var canvasOverlayGo = new GameObject("CanvasOverlay");
        var canvasOverlay = canvasOverlayGo.AddComponent<Canvas>();
        canvasOverlay.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasOverlay.planeDistance = 100;
        var scalerOverlay = canvasOverlayGo.AddComponent<CanvasScaler>();
        scalerOverlay.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scalerOverlay.referenceResolution = Resolution;
        scalerOverlay.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scalerOverlay.matchWidthOrHeight = 1;
        canvasOverlayGo.AddComponent<GraphicRaycaster>();
        CanvasOverlay = canvasOverlayGo.GetRectTransform();
        GameObject.DontDestroyOnLoad(canvasOverlayGo);

        //添加层级，不要轻易改动顺序
        m_rootLayer = AddLayer("rootLayer");
        m_contentLayer = AddLayer("contentLayer");
        m_popLayer = AddLayer("popLayer");
        //m_tvLayer = AddLayer("tvLayer");
        //m_chatLayer = AddLayer("chatLayer");
        //m_labaLayer = AddLayer("labaLayer");
        //m_tipLayer = AddLayer("tipLayer");
        //m_loadingLayer = AddLayer("loadingLayer");
        //UISystem.Init();
    }
    #endregion

    #region 层操作
    static public RectTransform GetLayer(LayerType value)
    {
        RectTransform result = null;
        switch (value)
        {
            case LayerType.Root:
                result = RootCanvas;
                break;
            case LayerType.Content:
                result = m_contentLayer;
                break;
            case LayerType.POP:
                result = m_popLayer;
                break;
            //case LayerType.TV:
            //    result = m_tvLayer;
            //    break;
            //case LayerType.Chat:
            //    result = m_chatLayer;
            //    break;
            //case LayerType.Laba:
            //    result = m_labaLayer;
            //    break;
            case LayerType.Tip:
                result = m_tipLayer;
                break;
            case LayerType.Loading:
                result = m_loadingLayer;
                break;
        }
        return result;
    }

    private static RectTransform AddLayer(string name)
    {
        var go = new GameObject(name);
        go.AddComponent<CanvasRenderer>();
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.SetUILocation(RootCanvas);
        return rect;
    }
    #endregion

    #region Panel相关操作方法
    /// <summary>
    /// 切换显示主要面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="params">参数数组</param>
    /// <returns></returns>
    public static T ShowPanel<T>(object[] @params = null) where T : BasePanel, new()
    {
        var panelName = typeof (T).Name;
        if (!m_panelDic.ContainsKey(panelName))
        {
            m_panelDic.Add(panelName, new T());
            m_panelLoopList.Add(m_panelDic[panelName]);
        }

        if (m_panelDic[panelName].IsOpen())
            return m_panelDic[panelName] as T;

        if (m_contentPanel != null)
        {
            m_preContentPanel = m_contentPanel;
            m_contentPanel.HidePanel();
        }

        m_contentPanel = m_panelDic[panelName];
        PushStack(panelName, @params);
     
        var panel = m_panelDic[panelName];
        panel.SetParams(@params);
        panel.ShowPanel(m_contentLayer);
        panel.LayerType = LayerType.Content;
       
        return panel as T;
    }

    private static void ShowPanel(BasePanel panel, object[] @params = null)
    {
        if (panel.IsOpen())
            return;

        if (m_contentPanel != null)
        {
            m_preContentPanel = m_contentPanel;
            m_contentPanel.HidePanel();
        }
        
        m_contentPanel = panel;

        m_contentPanel.SetParams(@params);
        m_contentPanel.ShowPanel(m_contentLayer);
        m_contentPanel.LayerType = LayerType.Content;
    }

    private static void PushStack(string panelName, object[] @params = null)
    {
        m_layerNameStack.Push(panelName);
        m_layerParamsStack.Push(@params);
    }

    public static void PopStack()
    {
        m_layerNameStack.Pop();
        m_layerParamsStack.Pop();
    }
    
    /// <summary>
    /// 显示根面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void PopRootPanel<T>() where T : BasePanel, new()
    {
        var panelName = typeof(T).Name;
        if (!m_panelDic.ContainsKey(panelName))
        {
            m_panelDic.Add(panelName, new T());
            m_panelLoopList.Add(m_panelDic[panelName]);
        }

        if (m_panelDic[panelName].IsOpen())
            return;

        m_panelDic[panelName].ShowPanel(m_rootLayer);
        m_panelDic[panelName].LayerType = LayerType.Root;
    }

    /// <summary>
    /// 在pop层弹出窗口并置顶，该层需要自己维护显示隐藏
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="params"></param>
    /// <param name="addMask"></param>
    /// <param name="parentPanel"></param>
    public static T PopPanel<T>(object[] @params = null, bool addMask = false, BasePanel parentPanel = null, LayerType layer = LayerType.POP) where T : BasePanel, new()
    {
        var panelName = typeof(T).Name;
        if (!m_panelDic.ContainsKey(panelName))
        {
            m_panelDic.Add(panelName, new T());
            m_panelLoopList.Add(m_panelDic[panelName]);
        }

        var popPanel = m_panelDic[panelName];
        popPanel.SetParams(@params);
        popPanel.ShowPanel(m_popLayer, addMask);
        popPanel.LayerType = LayerType.POP;
        if( parentPanel != null )
        {
            parentPanel.AddChildPanel(popPanel);
            popPanel.SetParent( parentPanel );
        }

        return popPanel as T;
    }

    /// <summary>
    /// 在chat层弹出窗口并置顶，该层需要自己维护显示隐藏
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="params"></param>
    /// <param name="addMask"></param>
    /// <param name="parentPanel"></param>
    //public static T PopChatPanel<T>(object[] @params = null, bool addMask = false, BasePanel parentPanel = null) where T : BasePanel, new()
    //{
    //    var panelName = typeof(T).Name;
    //    if (!m_panelDic.ContainsKey(panelName))
    //    {
    //        m_panelDic.Add(panelName, new T());
    //        m_panelLoopList.Add(m_panelDic[panelName]);
    //    }

    //    var popPanel = m_panelDic[panelName];
    //    popPanel.SetParams(@params);
    //    popPanel.ShowPanel(m_chatLayer, addMask);
    //    if (parentPanel != null)
    //    {
    //        parentPanel.AddChildPanel(popPanel);
    //        popPanel.SetParent(parentPanel);
    //    }

    //    return popPanel as T;
    //}


    public static void ShowTipPanel(string message, bool isMaskActive = true,Action closePanelCallback =null, ETipPanelStyle imgStyle = ETipPanelStyle.NONE)
    {
        ShowTipPanel3(message, showCenter: false, isMaskActive: isMaskActive,hideCallBack:closePanelCallback,imgStyle:imgStyle);
    }


    public static void ShowTipPanel1(string message, Action onLeft, Action onRight, string leftLabel, string rightLabel, bool showCancel, bool isMaskActive, ETipPanelStyle imgStyle = ETipPanelStyle.WARN)
    {
        ShowTipPanel3(message, onLeft: onLeft, onRight: onRight, leftLabel: leftLabel, rightLabel: rightLabel, showCenter: false, showDoubleBtn: true, showCancel: showCancel, isMaskActive: isMaskActive, imgStyle: imgStyle);
    }



    public static void ShowTipPanel2(string message, bool showCenter, string centerLabel, Action onCenter, bool showCancel, bool isMaskActive, ETipPanelStyle imgStyle = ETipPanelStyle.WARN)
    {
        ShowTipPanel3(message, onCenter: onCenter, showCenter: showCenter, showCancel: showCancel, isMaskActive: isMaskActive, imgStyle: imgStyle);
    }



    /// <summary>
    /// 使用默认的信息提示窗
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="onOk">按钮回调</param>
    /// <param name="onCancel">按钮回调</param>
    /// <param name="showCloseBtn">是否显示关闭按钮</param>
    /// <param name="showCancel">是否显示取消按钮</param>
    /// <param name="okLabel">确定按钮的文字</param>
    /// <param name="cancelLabel">取消按钮的文字</param>
    public static void ShowTipPanel3(string message, Action onLeft = null, Action onCancel = null, Action onCenter = null,Action onRight =null,
        Action hideCallBack = null,bool showCancel = false,bool showDoubleBtn = false, bool showCenter = true,string leftLabel = null, string rightLabel = null,
        string centerLabel = null, ETipPanelStyle imgStyle = ETipPanelStyle.NONE, bool isMaskActive = false)
    {
        if (string.IsNullOrEmpty(message))
            return;

        var panelName = typeof(PanelTip).Name;
        if (!m_panelDic.ContainsKey(panelName))
        {
            m_panelDic.Add(panelName, new PanelTip());
            m_panelLoopList.Add(m_panelDic[panelName]);
        }

        if (m_tipPanel != null)
            m_tipPanel.HidePanel();

        var param = new TipPanelParams();
        param.message = message;
        param.onLeft = onLeft;
        param.onCancel = onCancel;
        param.onRight = onRight;
        param.hideCallBack = hideCallBack;
        param.showCancel = showCancel;
        param.showCenter = showCenter;
        param.onCenter = onCenter;
        param.leftLabel = leftLabel;
        param.rightLabel = rightLabel;
        param.showDoubleBtn = showDoubleBtn;
        param.centerLabel = centerLabel;
        param.style = imgStyle;
        param.isMaskActive = isMaskActive;

        m_tipPanel = m_panelDic[panelName];
        m_tipPanel.SetParams(new object[] { param });
        m_tipPanel.ShowPanel(CanvasOverlay, true);
        m_tipPanel.LayerType = LayerType.Tip;
    }
    /// <summary>
    /// 使用默认的信息提示窗
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="onOk">按钮回调</param>
    /// <param name="onCancel">按钮回调</param>
    /// <param name="showCloseBtn">是否显示关闭按钮</param>
    /// <param name="showCancel">是否显示取消按钮</param>
    /// <param name="okLabel">确定按钮的文字</param>
    /// <param name="cancelLabel">取消按钮的文字</param>
    //public static void ShowTipMulTextPanel(string message, Action onOk = null, Action onCancel = null, bool showCloseBtn = true, bool showCancel = false, string okLabel = null, string cancelLabel = null)
    //{
    //    var panelName = typeof(PanelTipMulText).Name;
    //    if (!m_panelDic.ContainsKey(panelName))
    //    {
    //        m_panelDic.Add(panelName, new PanelTipMulText());
    //        m_panelLoopList.Add(m_panelDic[panelName]);
    //    }

    //    if (m_tipPanel != null)
    //        m_tipPanel.HidePanel();

    //    var param = new TipMulTextPanelParams();
    //    param.message = message;
    //    param.onOk = onOk;
    //    param.onCancel = onCancel;
    //    param.showCancel = showCancel;
    //    param.showCloseBtn = showCloseBtn;
    //    param.okLabel = okLabel;
    //    param.cancelLabel = cancelLabel;

    //    m_tipPanel = m_panelDic[panelName];
    //    m_tipPanel.SetParams(new object[] { param });
    //    m_tipPanel.ShowPanel(m_tipLayer, true);
    //}



    /// <summary>
    /// 使用阶段性引导专用提示窗
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="onOk">按钮回调</param>
    /// <param name="onCancel">按钮回调</param>
    /// <param name="showCloseBtn">是否显示关闭按钮</param>
    /// <param name="showCancel">是否显示取消按钮</param>
    /// <param name="okLabel">确定按钮的文字</param>
    /// <param name="cancelLabel">取消按钮的文字</param>
    public static void ShowTutorialPanel(string message, Action onLeft = null, Action onCancel = null,Action onRight =null,Action onCenter = null,
        bool showCancel = false, bool showDouble = false, bool showCenter = true, string leftLabel = null, string rightLabel = null, string centerLabel =null,
        bool isMaskActive = false, float yPos = 0,bool showNebula = false)
    {
        if (string.IsNullOrEmpty(message))
            return;


        var panelName = typeof(PanelTutorial).Name;
        if (!m_panelDic.ContainsKey(panelName))
        {
            m_panelDic.Add(panelName, new PanelTutorial());
            m_panelLoopList.Add(m_panelDic[panelName]);
        }

        if (m_tutorialPanel != null)
            m_tutorialPanel.HidePanel();

        var param = new TutorialPanelParams();
        param.message = message;
        param.onLeft = onLeft;
        param.onCancel = onCancel;
        param.onRight = onRight;
        param.onCenter = onCenter;

        param.showCancel = showCancel;
        param.showDouble = showDouble;
        param.showCenter = showCenter;


        param.LeftLabel = leftLabel;
        param.RightLabel = rightLabel;
        param.CenterLabel = centerLabel;
        param.yPos = yPos;
        param.isMaskActive = isMaskActive;
        param.showNebula = showNebula;

        m_tutorialPanel = m_panelDic[panelName];
        m_tutorialPanel.SetParams(new object[] { param });
        m_tutorialPanel.ShowPanel(CanvasOverlay, true);
    }



//     public static void ShowVenusPanel1(string content, string leftBtnText = null, string rightBtnText = null,
//         Action leftBtnCallback = null, Action rightBtnCallback = null, string imgURLPath = null)
//     {
//         ShowVenusPanel(EVenusPanelState.GETAWARD,content,leftBtnText,rightBtnText,leftBtnCallback,rightBtnCallback,2,true,imgURLPath);
//     }
// 
// 
//     /// <summary>
//     /// 显示摘星结果提示面板,只有一个按钮时用左按钮和左回调，没有按钮时点击遮罩可关闭面板，回调则用右回调
//     /// </summary>
//     /// <param name="type"></param>
//     /// <param name="param"></param>
//     public static void ShowVenusPanel(EVenusPanelState type,string content, string leftBtnText=null,string rightBtnText=null,
//         Action leftBtnCallback =null,Action rightBtnCallback=null,int btnCount =2,bool showMaks = true,string imgURLPath = null)
//     {
//         var panelName = typeof(PanelVenus).Name;
//         if (!m_panelDic.ContainsKey(panelName))
//         {
//             m_panelDic.Add(panelName, new PanelVenus());
//             m_panelLoopList.Add(m_panelDic[panelName]);
//         }
// 
//         m_venusPanel = m_panelDic[panelName];
//         if (m_venusPanel  != null)
//             m_venusPanel.HidePanel();
// 
//         VenusPanelParams venusPanelParams = new VenusPanelParams();
//         venusPanelParams.type                   = type;
//         venusPanelParams.content               = content;
//         venusPanelParams.leftBtnCallback        = leftBtnCallback;
//         venusPanelParams.rightBtnCallback       = rightBtnCallback;
//         venusPanelParams.leftBtnText        = leftBtnText;
//         venusPanelParams.rightBtnText       = rightBtnText;
//         venusPanelParams.btnCount = btnCount;
//         venusPanelParams.showmask = showMaks;
//         venusPanelParams.imgURLPaht = imgURLPath;
// 
//         m_venusPanel.SetParams(new object[]{venusPanelParams});
//         m_venusPanel.ShowPanel(m_popLayer, true);
//     }
// 
// 
// 
//     public static void HideVenusPanel()
//     {
//         if (m_venusPanel != null)
//         {
//             (m_venusPanel as PanelVenus).ClosePanelByAnimaton();
//         }
//     }

    /// <summary>
    /// 隐藏tipPanel
    /// </summary>
    public static void HideTipPanel()
    {
        if (m_tipPanel != null)
            m_tipPanel.HidePanel();
    }

    /// <summary>
    /// 隐藏loadingPanel
    /// </summary>
    public static void HideLoadingPanel() 
    {
        if (m_loadingPanel != null) 
        {
            m_loadingPanel.HidePanel();
        }
    }



    /// <summary>
    /// 隐藏Tutorialpanel
    /// </summary>
    public static void HideTutorialPanel()
    {
        if (m_tutorialPanel != null)
        {
            m_tutorialPanel.HidePanel();
        }
    }




    /// <summary>
    /// 文字浮动提示
    /// </summary>
    /// <param name="message">提示内容</param>
    /// <param name="duration">持续时间</param>
    /// <param name="from">起始位置</param>
    /// <param name="to">目标位置</param>
    public static void ShowFloatTipPanel(string message, float duration = 0.8f, Vector2 from = default(Vector2), Vector2 to = default(Vector2),ETipPanelStyle imgStyle = ETipPanelStyle.NONE,Action hidePanelCallback =null)
    {
        if (string.IsNullOrEmpty(message))
            return;

        var panelName = typeof(PanelFloatTip).Name;
        if (!m_panelDic.ContainsKey(panelName))
        {
            m_panelDic.Add(panelName, new PanelFloatTip());
            m_panelLoopList.Add(m_panelDic[panelName]);
        }

        if (m_floatTipPanel != null)
            m_floatTipPanel.HidePanel();

        var param = new FloatTipPanelParams();
        param.message = message;
        param.duration = duration;
        param.from = from;
        param.to = to;
        param.style = imgStyle;
        param.hidePanelCallback = hidePanelCallback;

        m_floatTipPanel = m_panelDic[panelName];
        m_floatTipPanel.SetParams(new object[] { param });
        m_floatTipPanel.ShowPanel(CanvasOverlay);
        m_floatTipPanel.LayerType = LayerType.Tip;
    }

    public static void HideFloatTipPanel()
    {
        if (m_floatTipPanel != null)
            m_floatTipPanel.HidePanel();
    }

    //private static float clearMaxTime = 8;
    private static float clearWatchDog;
    //private static int m_count;
    public static void ShowMask(string message, float clearTime = 8)
    {
        var panelName = typeof(PanelMask).Name;
        if (!m_panelDic.ContainsKey(panelName))
        {
            m_panelDic.Add(panelName, new PanelMask());
            m_panelLoopList.Add(m_panelDic[panelName]);
        }

        if (m_maskPanel != null)
            m_maskPanel.HidePanel();

        //m_count++;
        clearWatchDog = clearTime;
        m_maskPanel = m_panelDic[panelName];
        m_maskPanel.SetParams(new object[] { message });
        m_maskPanel.ShowPanel(CanvasOverlay, true);        
    }

    public static void HideMask()
    {
        clearWatchDog = 8;
        //m_count = Mathf.Max(0, --m_count);
        if (m_maskPanel != null)
            m_maskPanel.HidePanel();
    }

    //public static void ShowLoading(List<ResourceItem> resources, Action callBack = null)
    //{
    //    var panelName = typeof(PanelLoading).Name;
    //    if (!m_panelDic.ContainsKey(panelName))
    //    {
    //        m_panelDic.Add(panelName, new PanelLoading());
    //        m_panelLoopList.Add(m_panelDic[panelName]);
    //    }

    //    if (m_loadingPanel != null)
    //        m_loadingPanel.HidePanel();

    //    m_loadingPanel = m_panelDic[panelName] as PanelLoading;
    //    m_loadingPanel.DontDestroyOnLoad = true;
    //    m_loadingPanel.SetParams(resources.ToArray());
    //    m_loadingPanel.OnLoadComplete += callBack;
    //    m_loadingPanel.ShowPanel(m_loadingLayer);
    //}

    public static void ShowLoading() 
    {
        var panelName = typeof(PanelLoading).Name;
        if (!m_panelDic.ContainsKey(panelName))
        {
            m_panelDic.Add(panelName, new PanelLoading());
            m_panelLoopList.Add(m_panelDic[panelName]);
        }

        if (m_loadingPanel != null)
            m_loadingPanel.HidePanel();

        m_loadingPanel = m_panelDic[panelName] as PanelLoading;
        m_loadingPanel.DontDestroyOnLoad = true;
        m_loadingPanel.ShowPanel(CanvasOverlay);
        m_loadingPanel.LayerType = LayerType.Loading;
    }

    
    /// <summary>
    /// 上电视
    /// </summary>
    /// <param name="message"></param>
    //public static void ShowTv(string message)
    //{
    //    PanelTv.AddMsg(message);

    //    var panelName = typeof(PanelTv).Name;
    //    if (!m_panelDic.ContainsKey(panelName))
    //    {
    //        m_panelDic.Add(panelName, new PanelTv());
    //        m_panelLoopList.Add(m_panelDic[panelName]);
    //    }

    //    if (m_panelDic[panelName].IsOpen())
    //        return;

    //    if (m_tvPanel != null)
    //        m_tvPanel.HidePanel();
    //    m_tvPanel = m_panelDic[panelName];

    //    m_tvPanel.ShowPanel(m_tvLayer);
    //}

    /// <summary>
    /// 上喇叭
    /// </summary>
    /// <param name="record"></param>
    //public static void ShowLaba(ChatRecord record)
    //{
    //    PanelLaba.AddMsg(record);

    //    var panelName = typeof(PanelLaba).Name;
    //    if (!m_panelDic.ContainsKey(panelName))
    //    {
    //        m_panelDic.Add(panelName, new PanelLaba());
    //        m_panelLoopList.Add(m_panelDic[panelName]);
    //    }

    //    if (m_panelDic[panelName].IsOpen())
    //        return;

    //    if (m_labaPanel != null)
    //        m_labaPanel.HidePanel();
    //    m_labaPanel = m_panelDic[panelName];

    //    m_labaPanel.ShowPanel(m_labaLayer);
    //}

    /// <summary>
    /// 执行返回
    /// </summary>
    public static void BackPanel(object[] newParams = null)
    {
        if (m_contentPanel == null)
            return;

        if (m_layerNameStack.Count <= 0 || m_layerNameStack.Peek() != m_contentPanel.GetType().Name)
            return;

        PopStack();

        m_contentPanel.HidePanel();

        string prePanel = m_layerNameStack.Peek();

        object[] @params = newParams == null ? m_layerParamsStack.Peek() : newParams;

        if (!m_panelDic.ContainsKey(prePanel))
        {
            Debug.LogError("can not find panel: "+prePanel);
            return;
        }

        m_contentPanel = m_panelDic[prePanel];
        m_contentPanel.SetActive(true);
        m_contentPanel.SetBackParams(@params);
        m_contentPanel.OnBack();
    }

    /// <summary>
    /// 判断是否存在指定UI，但不一定是打开状态（可用IsOpen方法判断）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool HasPanel<T>() where T : BasePanel, new()
    {
        var panelName = typeof(T).Name;
        return m_panelDic.ContainsKey(panelName);
    }

    public static bool IsOpen<T>() where T : BasePanel, new()
    {
        var panelName = typeof(T).Name;
        return m_panelDic.ContainsKey(panelName) && m_panelDic[panelName].IsOpen();
    }

    public static T GetPanel<T>() where T : BasePanel, new()
    {
        var panelName = typeof(T).Name;
        if (m_panelDic.ContainsKey(panelName))
            return m_panelDic[panelName] as T;
        return null;
    }


    public static void DestroyPanel<T>() where T : BasePanel, new()
    {
        var panelName = typeof(T).Name;
        if (m_panelDic.ContainsKey(panelName))
        {
            var panel = m_panelDic[panelName];
            m_panelLoopList.Remove(panel);
            m_panelDic.Remove(panelName);
            panel.DestroyPanel();
        }
    }


    public static void DestroyAllPanel()
    {
        var dontDestroyPanelList = new List<KeyValue<string, BasePanel>>();
        foreach (var kv in m_panelDic)
        {
            if (!kv.Value.DontDestroyOnLoad)
                kv.Value.DestroyPanel();
            else
                dontDestroyPanelList.Add(new KeyValue<string, BasePanel>(kv.Key, kv.Value));
        }
        m_panelDic.Clear();
        m_panelLoopList.Clear();

        for (int i = 0; i < dontDestroyPanelList.Count; i++)
        {
            var panel = dontDestroyPanelList[i];
            m_panelDic.Add(panel.key, panel.value);
            m_panelLoopList.Add(panel.value);
        }

        m_contentPanel = null;
        dontDestroyPanelList.Clear();
        dontDestroyPanelList = null;
//        m_tvPanel = null;
        //m_tipPanel全局存在，不应该清理,否则容易造成控制权丢失，无法关掉弹出的窗口
        //m_tipPanel = null;

        //AudioManager.Clear();
        //if (Util.IsSmallMem())
        //    ResourceManager.Clear(ResTag.Forever, true, true);
        
    }

    /// <summary>
    /// 隐藏打开的面板
    /// </summary>
    public static void HideAllPanel()
    {
        m_panelDic.ForeachTo(p =>
            {
                if(p.Value.IsOpen())
                {
                    p.Value.HidePanel();
                }
            });
    }

    /// <summary>
    /// 关闭没有打开的界面
    /// </summary>
    public static void DestroyUnusedPanel ()
    {
        var list = new List<string>();
        foreach (var kv in m_panelDic)
        {
            if (kv.Value.IsOpen() || kv.Value.DontDestroyOnLoad)
                continue;
            list.Add(kv.Key);
            kv.Value.DestroyPanel();
        }
        for (int i = 0; i < list.Count; i++)
        {
            var panel = m_panelDic[list[i]];
            m_panelLoopList.Remove(panel);
            m_panelDic.Remove(list[i]);
        }
        list.Clear();
        list = null;
    }

    public static void Update()
    {
        if (clearWatchDog > 0)
        {
            clearWatchDog -= Time.deltaTime;
            if (clearWatchDog <= 0)
            {
                clearWatchDog = 0;
                HideMask();
            }
        }
        for (int i = 0; i < m_panelLoopList.Count; i++)
        {
            if (m_panelLoopList[i].IsInited())
                m_panelLoopList[i].Update();
        }
    }

    public static BasePanel CurContentPanel { get { return m_contentPanel; } }
    public static BasePanel PreContentPanel { get { return m_preContentPanel; } }

    /// <summary>
    /// 判断当前是否打开主界面,打开则可以点击流星，否则不能点击
    /// </summary>
    //public static bool isCurMainPanel { get { return IsOpen<PanelNavigation>(); } }

    public static void RespondPanelClick()
    {
        bool isCanClick = true;
        List<string> keys = new List<string>(m_panelDic.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            var panel = m_panelDic[keys[i]];
            if (panel.LayerType == LayerType.Tip)
            {
                if(panel.IsOpen())
                {
                    isCanClick = false;
                    break;
                }
            }
            else if(panel.LayerType == LayerType.POP)
            {
                if (panel.IsOpen())
                {
                    isCanClick = false;
                    panel.RespondBackBtnClick();
                    break;
                }
            }
            else if(panel.LayerType == LayerType.Loading)
            {
                if (panel.IsOpen())
                {
                    isCanClick = false;
                    break;
                }
            }
        }

        if(isCanClick)
        {
            m_contentPanel.RespondBackBtnClick();
        }
    }

    #endregion

    public static GameObject ShowLifebar(Transform target,int totalHP)
    {
        var lb = GameObjectPool.Spawn("LifeBar", null, Vector3.zero, Quaternion.identity);
        lb.ApplyLayer(GlobalConfig.LAYER_VALUE_3DUI);
        lb.GetComponent<RectTransform>().SetUILocation(Canvas3D);
        var lifeBar = lb.AddComponent<LifeBar>();
        lifeBar.TotalHP = totalHP;
        lifeBar.target = target;
        lifeBar.uiCam = UICamera3D;

        lifeBar.targetCam = Hero.thirdCam;

        return lb;
    }
}