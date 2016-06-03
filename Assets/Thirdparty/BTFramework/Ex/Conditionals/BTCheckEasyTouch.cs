using UnityEngine;
using System.Collections;

namespace BT.Ex
{
    /// <summary>
    /// 检测是否有触摸
    /// by zs 
    /// </summary>
    public class BTCheckEasyTouch : BTConditional
    {
        private bool checkedTouch = false;

        private Transform _trans;
        private Seeker seeker;

        public override void Activate(BTDatabase database)
        {
            base.Activate(database);

            EasyTouch.On_SimpleTap += On_SimpleTap;

            _trans = database.transform;
            seeker = _trans.GetComponent<Seeker>();
        }

        //当不可用时移除事件
        void OnDisable()
        {
            EasyTouch.On_SimpleTap -= On_SimpleTap;
        }

        //当销毁时移除事件
        void OnDestroy()
        {
            EasyTouch.On_SimpleTap -= On_SimpleTap;
        }

        public override bool Check()
        {
            return checkedTouch;
        }


        /// <summary>
        /// 响应点击地面
        /// </summary>
        /// <param name="gesture"></param>
        public void On_SimpleTap(Gesture gesture)
        {
            Debug.Log("On_SimpleTap...");
            if (gesture.pickObject != null && !gesture.isHoverReservedArea)
            {
                if (gesture.pickObject.layer == LayerMask.NameToLayer("Plane"))
                {
                    Debug.Log("BTCheckEasyTouch [On_SimpleTap] picked layer plane,gesture.hitPoint = " + gesture.hitPoint);
                    checkedTouch = true;
                    _database.SetData<Vector3>("TargetPositon", gesture.hitPoint);

                    seeker.StartPath(_trans.position, gesture.hitPoint);
                }
            }
        }

        public override void Clear()
        {
            base.Clear();

            checkedTouch = false;
        }
    }
}




