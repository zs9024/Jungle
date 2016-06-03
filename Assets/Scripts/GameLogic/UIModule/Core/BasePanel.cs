
//using Game.Asset;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 说明：
/// 1.每个页面都认为是一个Panel，大厅为常驻底层Panel，上面叠加各种功能Panel
/// 2.BasePanel类是个非Mono类，相当于MVC中M和C，V是BasePanel的属性，这样可以在界面隐藏状态下，依然能够监听处理事件数据，在页面打开的时候再更新界面信息
/// 3.Panel内的部件prefab上尽量不挂脚本，除非他的功能比较独立，尽量在Panel内集中处理
/// 
/// 创建Panel步骤：
/// 1.新建继承BasePanel的子类，添加构造函数、实现IPanel接口函数框架
/// 2.在构造函数中设置Panel的Prefab路径，并添加预加载资源信息。
/// 3.在Init接口中获取对象引用，以及其他初始化操作
/// 4.在InitEvent接口中注册UI事件或全局事件
/// 5.在OnShow、OnHide、OnDestroy、Update函数中写逻辑
/// 
/// Tips：
/// 1.基类BasePanel提供了m_go和m_tran，方便获取Panel的Prefab实例化后的GameoObject及其RectReansform对象
/// 2.执行ShowPanel后，先立即执行构造函数，等待资源加载完毕后，依次同步执行Init()，InitEvent()，OnShow()，在Panel没销毁之前再打开，就只执行OnShow()了
/// 3.对于像房间列表这种一打开UI就需要服务端数据的，可以在构造函数内向后端请求数据并添加监听，在后端消息返回和OnShow函数分别响应处理数据
/// 4.如果需要在页面没打开之前就能监听消息，可以写在UIManager类的RegedistServerMsg方法中
/// </summary>
public abstract class BasePanel:IPanel
{
    protected GameObject m_go;
    protected RectTransform m_tran;
    protected Canvas m_canvas;
    protected CanvasGroup m_canvasGroup;
    private List<string> m_resList = new List<string>();
    protected bool m_pixelPrefect = false;
    protected object[] m_params;
    protected object[] m_paramsBack;
    private bool m_preloaded;
    private bool m_preloading;
    private bool m_isOpen;
    private bool m_isOpenLogic;
    private bool m_isInited;
    private string m_prefabPath;
    public bool DontDestroyOnLoad { get; set; }

    private List<BasePanel> m_listChildPanel = new List<BasePanel>();
    private BasePanel m_parentPanel = null;

    public LayerType LayerType = LayerType.Content;

    //设置资源的路径
    protected void SetPanelPrefabPath(string path)
    {
        m_prefabPath = path;
    }
    //***************************************************************************************************

    //添加预加载资源的路径
    protected void AddPreLoadRes(string path)
    {
        m_resList.Add(path);
    }
    //***************************************************************************************************

    //获取预加载资源的路径
    public List<string> GetPreloadRes()
    {
        return m_resList;
    }
    //***************************************************************************************************

    //设置传入panel的参数
    public void SetParams(object[] @params)
    {
        m_params = @params;
    }
    //***************************************************************************************************

    //设置返回时的参数
    public void SetBackParams(object[] @params)
    {
        m_paramsBack = @params;
    }
    //***************************************************************************************************

    //添加子面板
    public void AddChildPanel( BasePanel child )
    {
        if( !m_listChildPanel.Contains( child ) )
        {
            m_listChildPanel.Add( child );
        }
    }
    //***************************************************************************************************


    public void AddChild( Transform child,bool worldPosStay )
    {
        child.SetParent(m_tran, worldPosStay);
    }

    public void SetParent( BasePanel parent )
    {
        m_parentPanel = parent;
    }

    public void SetActive(bool isActive)
    {
        m_isOpenLogic = isActive;
        if(m_go != null)
        {
            m_isOpen = isActive;
            m_go.SetActive(isActive);
        }
    }

    public Transform transform
    {
        get { return m_tran; }
    }

    public bool HasParams()
    {
        return m_params != null && m_params.Length > 0;
    }

    public bool HasBackParams()
    {
        return m_paramsBack != null && m_paramsBack.Length > 0;
    }

    public void ShowPanel(Transform parent = null, bool addMask = false, Action onShow = null)
    {
        if (m_isOpen)
            return;

        m_isOpenLogic = true;
        if (m_preloaded)
        {
            m_isOpen = true;
            m_go.SetActive(true);
            var tran = m_go.GetComponent<RectTransform>();
            tran.SetUILocation(parent, 0, 0);
            tran.SetAsLastSibling();
            SortingOrderRenderer.RebuildAll();
            OnShow();

            if (onShow != null)
                onShow();
        }
        else if (!m_preloading)
        {
            m_preloading = true;

            ResourceManager.LoadMulti(m_resList.ToArray(), null, () =>
            {
                m_preloading = false;

                if (!m_isOpenLogic)
                {
                     //UIManager.HideMask();
                     return;
                }
                
                 Action callback = () =>
                {
                    m_preloaded = true;
                    m_isOpen = true;

                    Init();
                    m_isInited = true;
                    InitEvent();

                    m_tran.SetUILocation(parent, 0, 0);
                    m_tran.localScale = new Vector3(1f, 1f, 1f);
                    m_tran.offsetMin = new Vector2(0f, 0f);
                    m_tran.offsetMax = new Vector2(0f, 0f);
                    m_tran.anchorMin = new Vector2(0f, 0f);
                    m_tran.anchorMax = new Vector2(1f, 1f);
                    m_tran.SetAsLastSibling();
                    m_go.SetActive(true);
                    m_go.ApplyLayer(GlobalConfig.LAYER_VALUE_UI);

                    //if (addMask && m_go.GetComponent<Image>() == null)
                    //{
                    //    var mask = m_go.AddComponent<Image>();
                    //    mask.sprite = ResourceManager.LoadSprite(AtlasName.Common, "itemblack");
                    //    mask.color = new Color(1f, 1f, 1f, 0.75f);
                    //    mask.type = Image.Type.Sliced;
                    //}

                    m_canvas = m_go.AddMissingComponent<Canvas>();
                    m_canvas.overridePixelPerfect = m_pixelPrefect;
                    m_canvas.pixelPerfect = m_pixelPrefect;

                    m_go.AddMissingComponent<SortingOrderRenderer>();
                    SortingOrderRenderer.RebuildAll();
        
                    OnShow();
                    if (onShow != null)
                        onShow();
                };

                if (m_go == null)
                {
                    ResourceManager.LoadUI(m_prefabPath, (obj) =>
                    {
                        m_go = obj;
                        m_tran = m_go.GetComponent<RectTransform>();
                        callback();
                    });
                }
                else
                {
                    callback();
                }
            });
        }
    }

    public void HidePanel()
    {
        m_isOpenLogic = false;
        if (!m_isOpen || m_go == null)
            return;

        m_isOpen = false;
        m_preloading = false;
        //NetLayer.RemoveHandler(this);
        m_go.SetActive(false);

        OnHide();

        for( int i = 0; i < m_listChildPanel.Count; ++ i )
        {
            m_listChildPanel[i].HidePanel();
        }
    }

    public bool IsOpen()
    {
        return m_isOpen;
    }

    public bool IsInited()
    {
        return m_isInited;
    }

    public void DestroyPanel()
    {
		m_isOpenLogic = false;
        if (m_go == null)
            return;

        m_isOpen = false;
        m_isInited = false;
        m_preloaded = false;
        m_preloading = false;
        //NetLayer.RemoveHandler(this);
       

        for (int i = 0; i < m_listChildPanel.Count; ++i)
        {
            m_listChildPanel[i].DestroyPanel();
        }
        m_listChildPanel.Clear();

        GameObject.Destroy(m_go);

        OnDestroy();
    }

    public float Alpha
    {
        get
        {
            if (m_canvasGroup == null)
            {
                m_canvasGroup = m_go.GetComponent<CanvasGroup>();
                if (m_canvasGroup == null)
                    m_canvasGroup = m_go.AddComponent<CanvasGroup>();
            }
            return m_canvasGroup.alpha;
        }
        set
        {
            if (m_canvasGroup == null)
            {
                m_canvasGroup = m_go.GetComponent<CanvasGroup>();
                if (m_canvasGroup == null)
                    m_canvasGroup = m_go.AddComponent<CanvasGroup>();
            }
            m_canvasGroup.alpha = value;
        }
    }

    public bool EnableMouseEvent
    {
        get
        {
            if (m_canvasGroup == null)
            {
                m_canvasGroup = m_go.GetComponent<CanvasGroup>();
                if (m_canvasGroup == null)
                    m_canvasGroup = m_go.AddComponent<CanvasGroup>();
            }
            return m_canvasGroup.blocksRaycasts;
        }
        set
        {
            if (m_canvasGroup == null)
            {
                m_canvasGroup = m_go.GetComponent<CanvasGroup>();
                if (m_canvasGroup == null)
                    m_canvasGroup = m_go.AddComponent<CanvasGroup>();
            }
            m_canvasGroup.blocksRaycasts = value;
        }
    }

    #region 监听安卓设备的返回键
    private UGUIClickHandler m_backBtnClickHandler;
    /// <summary>
    /// 添加按钮到 设备返回键
    /// </summary>
    public UGUIClickHandler AddBackBtnListen(UGUIClickHandler clickHandler)
    {
        m_backBtnClickHandler = clickHandler;
        return m_backBtnClickHandler;
    }

    public UGUIClickHandler AddBackBtnListen(GameObject goBackBtn)
    {
        m_backBtnClickHandler = UGUIClickHandler.Get(goBackBtn);
        return m_backBtnClickHandler;
    }

    public void RespondBackBtnClick()
    {
        if (m_backBtnClickHandler != null)
        {
            m_backBtnClickHandler.OnPointerClick(null);
        }
    }
    #endregion

    public abstract void Init();
    public abstract void InitEvent();
    public abstract void OnShow();
    public abstract void OnHide();
    public abstract void OnBack();
    public abstract void OnDestroy();
    public abstract void Update();
}
