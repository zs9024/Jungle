using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour
{
    /// [摄像机控制参数]
    public GameObject Target = null;
    public float RotateX = 0f;                                  // pitch 俯仰
    public float RotateY = 0f;                                  // yaw 偏航
    public float Distance = 10f;                                // 摄像机远近
    public float MoveSmoothTime = 0.3f;                         // 位置平滑时间，默认为1s
    public float RotateSmoothTime = 0.3f;                       // 旋转平滑时间
    public float ScrollWheelSpeed = 1000f;                      // 中轴调整速度
    public Vector3 TargetOffset = Vector3.zero;                 // 目标偏移量

    private Vector3 Velocity = Vector3.zero;                    // 平滑初速度
    private Vector3 Offset = Vector3.zero;                      // 根据上面三个变量计算出
    private const float MinDistance = 5f;                       // 镜头最近距离
    private const float MaxDistance = 100f;                     // 镜头最远距离
    private Quaternion tmpRotation = Quaternion.identity;
    private Vector3 tmpPosition = Vector3.zero;

    private float RotateDamping
    {
        get
        {
            if (RotateSmoothTime <= 0f) RotateSmoothTime = 0.001f;
            return 1f / RotateSmoothTime;
        }
    }

    void LateUpdate()
    {
        if (Target == null) return;

        tmpRotation = transform.rotation;
        tmpPosition = transform.position;

        UpdateDistance();
        UpdateRotation();
        UpdatePosition();

        transform.rotation = tmpRotation;
        transform.position = tmpPosition;
    }

    private void UpdateRotation()
    {
        if (!NeedRotate()) return;

        Quaternion wantedRotation = Quaternion.Euler(RotateX, RotateY, 0f);
        // 旋转采用球形插值
        tmpRotation = Quaternion.Slerp(tmpRotation, wantedRotation, Time.deltaTime * RotateDamping);
    }

    private void UpdatePosition()
    {
        // 如果有旋转插值，则位置根据旋转变换；否则，位置自己进行插值过渡
        if (!NeedRotate())
        {
            Offset = Quaternion.Euler(RotateX, RotateY, 0f) * Vector3.forward * Distance;
            Vector3 wantedPos = Target.transform.position - Offset + TargetOffset;
            // 位置采用平滑阻尼过渡
            tmpPosition = Vector3.SmoothDamp(tmpPosition, wantedPos, ref Velocity, MoveSmoothTime);
        }
        else
        {
            Offset = tmpRotation * Vector3.forward * Distance;
            tmpPosition = Target.transform.position - Offset + TargetOffset;
        }
    }

    private void UpdateDistance()
    {
        float horizontal = Input.GetAxis("Mouse ScrollWheel") * ScrollWheelSpeed * Time.deltaTime;
        Distance -= horizontal;
        Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
    }

    private bool NeedRotate()
    {
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        return !(FloatEqual(eulerAngles.x, RotateX) && FloatEqual(eulerAngles.y, RotateY));
    }

    public static bool FloatEqual(float value1, float value2)
    {
        float ret = value1 - value2;
        return ret > -0.0005f && ret < 0.0005f;
    }

}