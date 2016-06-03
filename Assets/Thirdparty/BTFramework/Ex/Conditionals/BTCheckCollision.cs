using UnityEngine;
using System.Collections;

namespace BT.Ex
{
    /// <summary>
    /// 检测碰撞
    /// </summary>
    public class BTCheckCollision : BTConditional
    {
        public enum CheckOpt
        {
            ControllerHit,
            RayCast
        }

        private CheckOpt _checkOpt;
        private bool _collidered = false;

        //检测的层级
        private string _layerName;
        private LayerMask _mask;
        //射线的距离
        private float _distance;

        private Transform _trans;

        private float _centY;
        public BTCheckCollision(string layerName,float distance, CheckOpt checkOpt)
        {
            _layerName = layerName;
            _distance = distance;
            _checkOpt = checkOpt;
        }

        public BTCheckCollision(CheckOpt checkOpt)
        {
            _checkOpt = checkOpt;
        }

        public override void Activate(BTDatabase database)
        {
            base.Activate(database);

            _trans = database.transform;
            _mask = 1 << (LayerMask.NameToLayer(_layerName));

            var controller = _trans.GetComponent<CharacterController>();
            if(controller != null)
            {
                _centY = controller.center.y * _trans.localScale.y;
            }
        }

        public override bool Check()
        {
            bool ret = false;
            switch (_checkOpt)
            {
                case CheckOpt.ControllerHit:
                    ret = _collidered;
                    break;
                case CheckOpt.RayCast:
                    Vector3 origin = new Vector3(_trans.position.x, _centY, _trans.position.z);
                    RaycastHit hitinfo;
                    if (ret = Physics.Raycast(origin, _trans.forward, out hitinfo, _distance, _mask.value))
                    {
                        Debug.DrawLine(origin, hitinfo.point, Color.red, 2);
                        //Debug.Log("Hit GO : " + hitinfo.collider.gameObject);
                    }                    
                    break;
                default:
                    break;
            }

            return ret;
        }


        public void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if(hit.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log("OnControllerColliderHit Player");
                _collidered = true;
            }
        }
    }
}
