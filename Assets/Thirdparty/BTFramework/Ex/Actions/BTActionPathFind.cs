using UnityEngine;
using System.Collections;
using Pathfinding;

namespace BT.Ex
{
    public class BTActionPathFind : BTAction
    {
        private Transform _trans;
        private Seeker seeker;


        private CharacterController controller;
        //一个path类的对象。表示路径
        public Path path;
        //角色每秒的速度
        public float speed = 100;
        //当角色与一个航点的距离小于这个值时，角色便可转向路径上的下一个航点
        public float nextWaypointDistance = 1;
        //角色正朝其行进的航点
        private int currentWaypoint = 0;

        private int turnSpeed = 100;

        public BTActionPathFind()
        {

        }

        public override void Activate(BTDatabase database)
        {
            base.Activate(database);
            _trans = database.transform;
            seeker = _trans.GetComponent<Seeker>();
            controller = _trans.GetComponent<CharacterController>();
            seeker.pathCallback += OnPathComplete;
        }

        protected override void Enter()
        {
            base.Enter();
        }

        protected override BTResult Execute()
        {
            if (path == null)
            {
                return BTResult.Running;
            }
            //如果当前路点编号大于这条路径上路点的总和，那么已经到达路径的终点
            if (currentWaypoint >= path.vectorPath.Count)
            {
                Debug.Log("EndOfPathReached");
                return BTResult.Success;
            }

            var curpoint = path.vectorPath[currentWaypoint];
            var pos = new Vector3(curpoint.x, _trans.position.y, curpoint.z);
            _trans.LookAt(pos);
            Vector3 eulerAngles = _trans.eulerAngles;
            _trans.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y + 180, eulerAngles.z);

            var direction = _trans.TransformDirection(Vector3.back);
            controller.Move(direction * Time.deltaTime * 2);

//             Vector3 dir = (path.vectorPath[currentWaypoint] - _trans.position).normalized;
//             Quaternion targetRotation = Quaternion.LookRotation(dir);
//             _trans.rotation = Quaternion.Slerp(_trans.rotation, targetRotation, Time.deltaTime * turnSpeed);
//             Vector3 eulerAngles = _trans.eulerAngles;
//             _trans.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y + 180, eulerAngles.z);
// 
//             controller.Move(dir * Time.deltaTime * 3);

            UpdateGravity();

            //如果当前位置与当前路点的距离小于一个给定值，可以转向下一个路点
            if (Vector3.Distance(_trans.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            return BTResult.Running;
        }

        //当寻路结束后调用这个函数
        public void OnPathComplete(Path p)
        {
            Debug.Log("FindThePath" + p.error);
            //如果找到了一条路径，保存下来，并且把第一个路点设置为当前路点
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
            }
        }

        float gravity = 2.0f;
        void UpdateGravity()
        {
            if (BTCharacator.CharacController != null && gravity > 0)
            {
                BTCharacator.CharacController.Move(Vector3.down * gravity * Time.deltaTime);
            }
        }
    }
}
