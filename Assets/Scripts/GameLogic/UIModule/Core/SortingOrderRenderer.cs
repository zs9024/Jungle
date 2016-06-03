using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortingOrderRenderer : MonoBehaviour
{
    public bool ForceRebuildAll;    //给编辑器模式下用的
    private static bool m_rebuildAllTag;
    private bool m_noGraphicRaycaster;
    private bool m_isUI;
    public int m_order;
    private List<int> m_depthLinkedList = new List<int>();
    private Canvas m_canvas;
    private Renderer[] m_renderers;
    private static List<SortingOrderRenderer> m_allRendererList = new List<SortingOrderRenderer>();

    private void Awake()
    {
        m_isUI = GetComponent<CanvasRenderer>() != null;
        m_allRendererList.Add(this);
    }

    private void Update()
    {
        if (ForceRebuildAll)
        {
            ForceRebuildAll = false;
            RebuildAllCore(true);
        }
    }

    private void LateUpdate()
    {
        if (m_rebuildAllTag)
        {
            m_rebuildAllTag = false;
            RebuildAllCore(false);
        }
    }

    private void OnDestroy()
    {
        m_canvas = null;
        m_renderers = null;
        m_depthLinkedList = null;
        m_allRendererList.Remove(this);
    }

    private void Rebuild(bool force = false)
    {
        if (m_isUI)
        {
            if (!m_canvas)
            {
                m_canvas = GetComponent<Canvas>();
                if (!m_canvas)
                    m_canvas = gameObject.AddComponent<Canvas>();
                m_canvas.overrideSorting = true;

                if (!m_noGraphicRaycaster)
                {
                    var r = GetComponent<GraphicRaycaster>();
                    if (!r)
                    {
                        gameObject.AddComponent<GraphicRaycaster>();
                        var tran = gameObject.transform;
                        //强制重新激活使GraphicRaycaster生效
                        var index = tran.GetSiblingIndex();
                        tran.SetParent(tran.parent, false);
                        tran.SetSiblingIndex(index);
                    }
                }
            }
            if (!m_canvas.overrideSorting)
                m_canvas.overrideSorting = true;
            m_canvas.sortingOrder = m_order;
        }
        else
        {
            if (m_renderers == null || force)
                m_renderers = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < m_renderers.Length; i++)
            {
                m_renderers[i].sortingOrder = m_order;
            }
        }
    }

    private void RecalcDepth()
    {
        if (!enabled)
            return;

        if (m_depthLinkedList == null)
            m_depthLinkedList = new List<int>();
        else
            m_depthLinkedList.Clear();
        
        var tran = transform;
        while (tran != null)
        {
            m_depthLinkedList.Insert(0, tran.GetSiblingIndex());
            tran = tran.parent;
        }
    }

    public static void RebuildAll()
    {
        m_rebuildAllTag = true;
//        RebuildAllCore(false);
    }

    private static void RebuildAllCore(bool force)
    {
        for (int i = 0; i < m_allRendererList.Count; i++)
        {
            m_allRendererList[i].RecalcDepth();
        }
        m_allRendererList.Sort(CompareDepthLinkedList);
        for (int i = 0; i < m_allRendererList.Count; i++)
        {
            m_allRendererList[i].m_order = i + 1;
            m_allRendererList[i].Rebuild(force);
        }
    }

    private static int CompareDepthLinkedList(SortingOrderRenderer x, SortingOrderRenderer y)
    {
        var aList = x.m_depthLinkedList;
        var bList = y.m_depthLinkedList;
        var length = Mathf.Max(aList.Count, bList.Count);

        for (int i = 0; i < length; i++)
        {
            if (i >= aList.Count && i >= bList.Count)
                return 0;
            else if (i >= aList.Count)
                return -1;
            else if (i >= bList.Count)
                return 1;
            else if (aList[i] < bList[i])
                return -1;
            else if (aList[i] > bList[i])
                return 1;
        }
        return 0;
    }

    /// <summary>
    /// 不需要接收鼠标事件
    /// </summary>
    public void RemoveGraphicRaycaster()
    {
        m_noGraphicRaycaster = true;
        var componet = GetComponent<GraphicRaycaster>();
        if (componet != null)
            Destroy(componet);
    }
}