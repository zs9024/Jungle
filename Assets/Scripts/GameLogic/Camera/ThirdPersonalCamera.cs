// 第三人称摄像机，平移和旋转会同时进行平滑
using UnityEngine;
public class ThirdPersonalCamera : MonoBehaviour
{
    /// Camera Control Params
    public GameObject Target = null;
    public float Distance = 10f;
    public float RotateX = 0f;                                  // pitch 俯仰
    public float RotateY = 0f;                                  // yaw 偏航
    public float SmoothTime = 0.01f;                            // 平滑时间，默认为1s
    public float ScrollWheelSpeed = 1000f;                      // 中轴调整速度
    public Vector3 TargetOffset = Vector3.zero;                 // 目标偏移量

    private Vector3 Velocity = Vector3.zero;                    // 平滑初速度
    private Vector3 Offset = Vector3.zero;                      // 根据上面三个变量计算出
    private const float MinDistance = 5f;                       // 镜头最近距离
    private const float MaxDistance = 100f;                     // 镜头最远距离

    void Start()
    {
        if (Target == null) return;
    }

    public void Shake()
    {
        StartCoroutine("ShakeCoroutine");
    }

    void LateUpdate()
    {
        if (Target == null) return;

        UpdateDistance();

        float radX = Mathf.Deg2Rad * RotateX;
        float radY = Mathf.Deg2Rad * RotateY;

        Offset.x = Distance * Mathf.Cos(radX) * Mathf.Cos(radY);
        Offset.z = Distance * Mathf.Cos(radX) * Mathf.Sin(radY);
        Offset.y = Distance * Mathf.Sin(radX);

        Vector3 targetPos = Target.transform.position + Offset + TargetOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref Velocity, SmoothTime);

        transform.LookAt(Target.transform.position + TargetOffset);
    }

    private void UpdateDistance()
    {
        float horizontal = Input.GetAxis("Mouse ScrollWheel") * ScrollWheelSpeed * Time.deltaTime;
        Distance -= horizontal;
    }

}