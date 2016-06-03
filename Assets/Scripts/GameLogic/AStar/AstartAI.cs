using UnityEngine;
using System.Collections;

using Pathfinding;
public class AstartAI : MonoBehaviour {

	//目标位置
    public Transform targetPosition;
    //声明一个seeker类的对象
    private Seeker seeker;
    private CharacterController controller;
    //一个path类的对象。表示路径
    public Path path;
    //角色每秒的速度
    public float speed = 100;
    //当角色与一个航点的距离小于这个值时，角色便可转向路径上的下一个航点
    public float nextWaypointDistance = 3;
    //角色正朝其行进的航点
    private int currentWaypoint = 0;

    void Start()
    {
        //获得对Seeker组件的引用
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
        //注册回调函数，在AstartPath完成寻路后调用该函数。
        seeker.pathCallback += OnPathComplete;  
        //调用StartPath函数，开始到目标的寻路
        seeker.StartPath(transform.position, targetPosition.position);
    }

    private int turnSpeed = 100;
    void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }
        //如果当前路点编号大于这条路径上路点的总和，那么已经到达路径的终点
        if (currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log("EndOfPathReached");
            return;
        }
        //计算出去往当前路点所需的行走方向和距离，控制角色移动
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed*Time.fixedDeltaTime;
        controller.SimpleMove(dir);
        //角色转向目标
        Quaternion targetRotation=Quaternion.LookRotation(dir);
        transform.rotation=Quaternion.Slerp(transform.rotation,targetRotation,Time.deltaTime*turnSpeed);
        //如果当前位置与当前路点的距离小于一个给定值，可以转向下一个路点
        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }
    //当寻路结束后调用这个函数
    public void OnPathComplete(Path p)
    {
        Debug.Log("FindThePath"+p.error);
        //如果找到了一条路径，保存下来，并且把第一个路点设置为当前路点
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void OnDisable()
    {
        seeker.pathCallback -= OnPathComplete;
    }
}
