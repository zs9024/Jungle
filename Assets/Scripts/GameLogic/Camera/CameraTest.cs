using UnityEngine;
using System.Collections;

public class CameraTest : MonoBehaviour {

    public Transform target;
    public float smoothTime;
    private Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        // Define a target position above and behind the target transform
	    //定义一个目标位置在目标变换的上方并且在后面
	    Vector3 targetPosition = target.TransformPoint(new Vector3(0, 5, -10));

	    // Smoothly move the camera towards that target position
	    //平滑地移动摄像机朝向目标位置
	    transform.position = Vector3.SmoothDamp(transform.position, targetPosition,ref velocity, smoothTime);

	}
}
