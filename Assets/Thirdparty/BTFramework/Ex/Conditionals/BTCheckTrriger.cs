using UnityEngine;
using System.Collections;

namespace BT.Ex
{
    /// <summary>
    /// 检测是否响应了角色碰撞事件
    /// </summary>
    public class BTCheckTrriger : BTConditional
    {
        private bool trrigered = false;
        public BTCheckTrriger()
        {

        }

        public override void Activate(BTDatabase database)
        {
            base.Activate(database);

            
        }

        public override bool Check()
        {
            return trrigered;
        }

        public void OnTriggerEnter(Collider other)
        {
            Debug.Log("OnTriggerEnter");
            trrigered = true;
        }

        public void OnTriggerStay(Collider other)
        {
            Debug.Log("OnTriggerStay");
        }

        public void OnTriggerExit(Collider other)
        {
            Debug.Log("OnTriggerExit");
            trrigered = false;
        }


    }

}
