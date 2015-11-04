using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 引擎类，程序入口
/// 初始化一些设置；全局监听；注册tick
/// </summary>
public class Engine : AbstractView {

    private static Engine instance;

    public static int mainThreadId = 0;

    private List<ITick> tickList = new List<ITick>();

    /// <summary>
    /// 单例实例
    /// </summary>
    public static Engine Instance
    {
        get
        {
            if (instance != null)
                return instance;

            instance = Object.FindObjectOfType<Engine>();
            if (instance != null)
                return instance;

            var obj = new GameObject("Engine");
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            instance = obj.AddComponent<Engine>();
            return instance;
        }
    }

    public override void Awake()
    {
        instance = this;
        UnityEngine.Object.DontDestroyOnLoad(this);

        initGlobalConfig();

        if (Application.isPlaying)
        {
            mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            print("MainThread Id:" + mainThreadId);
        }

        base.Awake();
    }

    private void initGlobalConfig()
    {
        GlobalConfig.SetPath();
        GlobalConfig.OriginResolution = Screen.currentResolution;
    }

    public override void Start()
    {
        Caching.CleanCache();
        string sysInfo = getSystemInfo();
        Debug.Log("SystemInfo > " + sysInfo);

        base.Start();
    }

    public override void Update()
    {
        ticks();
        keyActionListen();

        base.Update();
    }


    /// <summary>
    /// 获取系统信息，设备信息等
    /// </summary>
    /// <returns></returns>
    private string getSystemInfo()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append("Record Time : ");
        sb.AppendLine(System.DateTime.Now.ToString());

        sb.Append("deviceModel : ").AppendLine();
        sb.AppendLine(SystemInfo.deviceModel);
        sb.Append("deviceName : ").AppendLine();
        sb.AppendLine(SystemInfo.deviceName);
        sb.Append("systemMemorySize : ").AppendLine();
        sb.AppendLine(SystemInfo.systemMemorySize.ToString("0M"));
        sb.Append("graphicsMemorySize : ").AppendLine();
        sb.AppendLine(SystemInfo.graphicsMemorySize.ToString("0M"));
        sb.AppendLine(SystemInfo.deviceType.ToString());
        sb.Append("deviceUniqueIdentifier : ").AppendLine();
        sb.AppendLine(SystemInfo.deviceUniqueIdentifier.ToString());
        sb.Append("graphicsDeviceName : ").AppendLine();
        sb.AppendLine(SystemInfo.graphicsDeviceName);
        sb.Append("graphicsDeviceVendor : ").AppendLine();
        sb.AppendLine(SystemInfo.graphicsDeviceVendor);
        sb.Append("graphicsDeviceVendorID : ").AppendLine();
        sb.AppendLine(SystemInfo.graphicsDeviceVendorID.ToString());
        sb.Append("graphicsDeviceVersion : ").AppendLine();
        sb.AppendLine(SystemInfo.graphicsDeviceVersion);
        sb.Append("processorType : ").AppendLine();
        sb.AppendLine(SystemInfo.processorType.ToString());
        sb.Append("processorCount : ").AppendLine();
        sb.AppendLine(SystemInfo.processorCount.ToString());

        return sb.ToString();
    }


    /// <summary>
    /// 按键监听
    /// </summary>
    private void keyActionListen()
    {
        if (Input.GetKeyDown(KeyCode.Escape))   //返回键
        {
            Application.Quit();
        }
    }


    /// <summary>
    /// 加入tick对象
    /// </summary>
    /// <param name="tk"></param>
    public static void AddTick(ITick tk)
    {
        if (tk == null)
        {
            Debug.Log("Warning: Don't add the null object to tick object list");
            return;
        }

        if (instance.tickList.Exists(t => t == tk))
        {
            Debug.Log("Warning: Don't add the same tick object twice.");
            return;
        }

        instance.tickList.Add(tk);
    }

    /// <summary>
    /// 移除tick对象
    /// </summary>
    /// <param name="tk"></param>
    public static void RemoveTick(ITick tk)
    {
        if (instance.tickList == null || instance.tickList.Count <= 0)
        {
            Debug.LogWarning("The TickList has error:: null or 0");
            return;
        }

        if (!instance.tickList.Remove(tk))
        {
            Debug.Log("Warning: Remove tick object error. May be the tick object is not in list.");
        }
    }

    /// <summary>
    /// 循环tick列表，调用tick对象的update方法
    /// </summary>
    private void ticks()
    {
        if (tickList != null && tickList.Count > 0)
        {
            foreach (ITick tk in tickList)
            {
                if (tk == null)
                    continue;

                tk.Update();
            }
        }
    }

}


/// <summary>
/// 每帧检测调用接口
/// </summary>
public interface ITick
{
    void Update();
}