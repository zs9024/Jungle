using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;


public static class ExtendFuncHelper
{
    /// <summary>
    /// 【扩展】设置UI组件的父对象、坐标
    /// </summary>
    /// <param name="source"></param>
    /// <param name="parent"></param>
    /// <param name="x">如果为float.NaN，则不更改</param>
    /// <param name="y">如果为float.NaN，则不更改</param>
    public static void SetUILocation(this RectTransform source, Transform parent, float x = float.NaN, float y = float.NaN)
    {
        source.SetParent(parent, false);
        var pos = source.anchoredPosition;
        if (!float.IsNaN(x))
            pos.x = x;
        if (!float.IsNaN(y))
            pos.y = y;
        source.anchoredPosition = pos;
    }

    /// <summary>
    /// 【扩展】设置Tranform的父对象、本地坐标
    /// </summary>
    /// <param name="source"></param>
    /// <param name="parent"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void SetLocation(this Transform source, Transform parent, float x = float.NaN, float y = float.NaN, float z = float.NaN)
    {
        source.SetParent(parent, false);
        var pos = source.localPosition;
        if (!float.IsNaN(x))
            pos.x = x;
        if (!float.IsNaN(y))
            pos.y = y;
        if (!float.IsNaN(z))
            pos.z = z;
        source.localPosition = pos;
    }

    public static RectTransform GetRectTransform(this GameObject source)
    {
        return source.GetComponent<RectTransform>();
    }

    public static bool SetToggleOn(this ToggleGroup group, int index)
    {
        if (group == null || group.gameObject == null || index < 0) return false;
        Toggle[] toggleArray = group.gameObject.GetComponentsInChildren<Toggle>(true);
        if (toggleArray == null) return false;
        int length = toggleArray.Length;
        if (length <= index) return false;
        for (int i = 0; i < length; i++)
        {
            if(toggleArray[i] != null)
            {
                toggleArray[i].isOn = i == index;
            }
        }
        return true;
    }

    static public T AddMissingComponent<T>(this GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
            comp = go.AddComponent<T>();
        return comp;
    }

    static public void DestroyComponent<T>(this GameObject go) where T : Component
    {
        if (go == null)
            return;

        T comp = go.GetComponent<T>();
        if (comp != null)
            UnityEngine.Object.Destroy(comp);
    }

    static public void ResetLocal(this Transform trans)
    {
        if(trans == null)
        {
            Debug.LogError("ResetLocal trans is null");
            return;
        }

        trans.localPosition = Vector3.zero;
        trans.localEulerAngles = Vector3.zero;
        trans.localScale = Vector3.one;
    }

    /// <summary>
    /// 递归搜索
    /// </summary>
    static public Transform Find2(this Transform trans, string name, bool ignoreCase=true)
    {
        if (trans == null)
            return null;

        Transform child = null;
        Transform ret = null;
        for(int i = 0; i<trans.childCount; i++)
        {
            child = trans.GetChild(i);
            if (ignoreCase && child.name.ToLower() == name.ToLower() || !ignoreCase && child.name == name)
            {
                ret = child;
                break;
            }

            ret = child.Find2(name, ignoreCase);
            if (ret != null)
                break;
        }

        return ret;
    }

    /// <summary>
    /// 查找失败 返回空类型
    /// </summary>   
    static public void Find<T>(this Transform trans, string path, ref T t) where T : Component
    {
        t = trans.Find<T>(path);
    }
    static public T Find<T>(this Transform trans, string path) where T : Component
    {
        var target = trans.Find(path);
        if(target != null)
        {
            var component = target.GetComponent<T>();
            if(component != null)
            {
                return component;
            }
            Debug.LogError(trans.name + "/" + path + " is no find");
            return null;
        }
        Debug.LogError(trans.name + "/" + path + " is no find");
        return null;
    }
    static public void Find(this Transform trans, string path, ref GameObject go)
    {
        var target = trans.Find(path);
        if (target != null)
        {
            go = target.gameObject;
        }
        else
        {
            Debug.LogError(trans.name + "/" + path + " is no find");
            go = null;
        }
    }

    /// <summary>
    /// foreach循环
    /// </summary>    
    static public void ForeachTo<T>(this IEnumerable<T> collect, Action<T> action)
    {
        foreach(T t in collect)
        {
            action(t);
        }
    }

    /// <summary>
    /// 等比缩放
    /// </summary>
    static public void Scaling(this GameObject go, int width = 1080, int height = 1920)
    {
        float ratio = width * Screen.height * 1f / height / Screen.width;
        RectTransform rt = go.GetComponent<RectTransform>();
        if(rt != null)
        {
            rt.sizeDelta = rt.sizeDelta * ratio;
        }
    }

    /// <summary>
    /// 等比缩放
    /// </summary>
    static public void Scaling(this Transform trans, int width = 1080, int height = 1920)
    {
        float ratio = width * Screen.height * 1f / height / Screen.width;
        RectTransform rt = trans.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta = ratio * rt.sizeDelta;
        }
    }

    //static public void ScalingX(this GameObject go)
    //{
    //    RectTransform rt = go.GetComponent<RectTransform>();
    //    if (rt != null)
    //    {

    //    }
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rt"></param>
    /// <param name="trans"></param>
    static public void CopyFrom(this RectTransform rt, RectTransform target_rt)
    {
        if (target_rt == null)
        {
            return;
        }
        
        rt.localScale = target_rt.localScale;
        rt.anchoredPosition = Vector2.zero;
        //LayoutElement le = target_rt.GetComponent<LayoutElement>();
        
        //var vlg = target_rt.parent.GetComponent<VerticalLayoutGroup>();
        rt.sizeDelta = target_rt.sizeDelta;
        //TODO:补丁
        if(target_rt.parent != null)
        {
            rt.sizeDelta = new Vector2(target_rt.parent.gameObject.GetRectTransform().rect.width, rt.sizeDelta.y);            
        }              
    }

    static public void ApplyLayer(this GameObject goParent, int newLayer, int[] excludeLayers = null, string[] excludeName = null, bool traverse = true)
    {
        if (goParent == null)
            return;

        if (excludeLayers != null)
        {
            for (int i = 0; i < excludeLayers.Length; i++)
            {
                if (excludeLayers[i] == goParent.layer)
                    return;
            }
        }

        string name = goParent.name;
        if (excludeName != null)
        {
            for (int i = 0; i < excludeName.Length; i++)
            {
                if (excludeName[i] == name)
                    return;
            }
        }

        goParent.layer = newLayer;

        if (!traverse)
            return;

        Transform transParent = goParent.transform;
        Transform transChild;
        int iChildCnt = transParent.transform.childCount;

        for (int i = 0; i < iChildCnt; i++)
        {
            transChild = transParent.transform.GetChild(i);
            transChild.gameObject.ApplyLayer(newLayer, excludeLayers, excludeName, traverse);
        }
    }

    /// <summary>
    /// 只设置所有子节点的层，不设置本层
    /// </summary>
    static public void ApplyChildrenLayer(this GameObject goParent, int newLayer, int[] excludeLayers = null, string[] excludeName = null)
    {
        if (goParent == null)
            return;

        Transform transParent = goParent.transform;
        Transform transChild;
        int iChildCnt = transParent.transform.childCount;

        for (int i = 0; i < iChildCnt; i++)
        {
            transChild = transParent.transform.GetChild(i);
            transChild.gameObject.ApplyLayer(newLayer, excludeLayers, excludeName, true);
        }
    }

    /// <summary>
    /// 递归访问
    /// </summary>
    static public void VisitChild(this Transform transform, Action<GameObject> funCallBack, bool includeSelf)
    {
        if (includeSelf)
            funCallBack(transform.gameObject);

        transform.VisitChild(funCallBack);
    }

    static private void VisitChild(this Transform transform, Action<GameObject> funCallBack)
    {
        if (transform == null)
            return;

        Transform transChild = null;

        int iChildCnt = transform.transform.childCount;
        for (int i = 0; i < iChildCnt; i++)
        {
            transChild = transform.transform.GetChild(i);
            funCallBack(transChild.gameObject);
            transChild.VisitChild(funCallBack);
        }
    }

    public delegate bool CallBackBool<T>(T obj);

    static public void VisitChild(this Transform transform, CallBackBool<GameObject> funCallBack, bool includeSelf)
    {
        if (includeSelf)
            funCallBack(transform.gameObject);

        transform.VisitChild(funCallBack);
    }

    static private void VisitChild(this Transform transform, CallBackBool<GameObject> funCallBack)
    {
        if (transform == null)
            return;

        Transform transChild = null;

        int iChildCnt = transform.transform.childCount;
        for (int i = 0; i < iChildCnt; i++)
        {
            transChild = transform.transform.GetChild(i);
            if(funCallBack(transChild.gameObject) == true)
                transChild.VisitChild(funCallBack);
        }
    }

    static public bool IsInLayerMask(this GameObject go, LayerMask layerMask)
    {
        int goLayerMask = (1 << go.layer);
        return (goLayerMask & layerMask.value) > 0;
    }

    static public void ReplaceShader(this GameObject go, string oldShaderName, List<string> excludeShader, Shader newShader, bool useLightProbe, bool newMaterial = false)
    {
        if (go == null)
            return;

#if UNITY_EDITOR
        //if (newMaterial == true)
        //    GameObjectHelper.RefreshMaterial(go.transform);
#endif

        go.transform.VisitChild((child)=>
        {
            Renderer render = child.GetComponent<Renderer>();
            if(render != null)
            {
                render.useLightProbes = useLightProbe;
                Material sharedMat = render.sharedMaterial;

                if (sharedMat != null)
                {
                    if (oldShaderName.IsNullOrEmpty() || sharedMat.shader.name == oldShaderName)
                    {
                        if (excludeShader == null || excludeShader.Contains(sharedMat.shader.name) == false)
                        {
                            sharedMat.shader = newShader;
#if UNITY_EDITOR
                            sharedMat.shader = Shader.Find(newShader.name);
#endif
                        }
                    }
                }
            }
        },true);
    }

    static public void CopyPropFloatFrom(this Material toMat, Material fromMat, string propName)
    {
        if (toMat == null || fromMat == null || toMat.HasProperty(propName) == false || fromMat.HasProperty(propName) == false)
            return;

        toMat.SetFloat(propName, fromMat.GetFloat(propName));
    }

    static public void CopyPropTexFrom(this Material toMat, Material fromMat, string propName)
    {
        if (toMat == null || fromMat == null || toMat.HasProperty(propName) == false || fromMat.HasProperty(propName) == false)
            return;

        toMat.SetTexture(propName, fromMat.GetTexture(propName));
    }

    static public void CopyPropColorFrom(this Material toMat, Material fromMat, string propName)
    {
        if (toMat == null || fromMat == null || toMat.HasProperty(propName) == false || fromMat.HasProperty(propName) == false)
            return;

        toMat.SetColor(propName, fromMat.GetColor(propName));
    }

# region C# Related Expend Method

    static public string ToStr<T>(this T[] arr, string splitChar = " ")
    {
        if (arr == null)
            return "null";

        string msg = "";
        foreach (T value in arr)
        {
            msg += splitChar + value.ToString();
        }

        return msg;
    }

    static public string ToStr<T>(this List<T> list, string splitChar = " ")
    {
        if (list == null)
            return "null";

        string msg = "";
        foreach (T value in list)
        {
            msg += splitChar + value.ToString();
        }

        return msg;
    }

    static public bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    /// <summary>
    /// 转换颜色
    /// </summary>
    static public string U(this string str, Color color)
    {
        string strColor = ((int)(color.r * 255)).ToString("X") +
                          ((int)(color.g * 255)).ToString("X") +
                          ((int)(color.b * 255)).ToString("X");
        return string.Format("<color=#" + strColor + ">{0}</color>", str);
    }

    /// <summary>
    /// 带正负号的角度
    /// </summary>
    static public float Angle2(this Vector3 vec, Vector3 dir)
    {
        float angle = Vector3.Angle(vec, dir);
        float crossValue = Vector3.Cross(vec, dir).y;
        angle = crossValue >= 0 ? angle : -angle;
        return angle;
    }

    static public Color toColor(this int[] arr)
    {
        if(arr == null || arr.Length < 3)
        {
            Debug.LogError("int[].toColor length < 3");
            return Color.white;
        }

        Color color = new Color(arr[0], arr[1], arr[2], 255);
        if (arr.Length == 4)
            color.a = arr[3];

        return color;
    }
#endregion
}
