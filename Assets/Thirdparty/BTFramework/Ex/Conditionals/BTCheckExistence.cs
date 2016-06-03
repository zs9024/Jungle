using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT.Ex
{
    /// <summary>
    /// 检查范围内的怪物或者玩家英雄
    /// </summary>
    public class BTCheckExistence : BTConditional
    {
        //检测类型
        public enum CheckOpt
        {
            CheckSphere,
            OverlapSphere
        }

        //检测位置
        private Transform _trans;
        //检测半径
        private float _radius;
        //检测的层级
        private string _layerName;
        //
        private CheckOpt _checkOpt;
        //
        private LayerMask _mask;

        public BTCheckExistence(Transform trans, float radius, string layerName, CheckOpt checkOpt)
        {
            _trans = trans;
            _radius = radius;
            _layerName = layerName;
            _checkOpt = checkOpt;
        }

        public override void Activate(BTDatabase database)
        {
            base.Activate(database);

            _mask = 1 << (LayerMask.NameToLayer(_layerName));
        }

        public override bool Check()
        {
            bool ret = false;
            switch(_checkOpt)
            {
                case CheckOpt.CheckSphere:
                    ret = Physics.CheckSphere(_trans.position, _radius, _mask.value);
                    break;
                case CheckOpt.OverlapSphere:
                    Collider[] cols = Physics.OverlapSphere(_trans.position, _radius, _mask.value);
                    ret = (cols != null && cols.Length > 0);
                    break;
                default:
                    break;
            }

            return ret;
        }










        /// <summary>
        /// 查找场景中所有的游戏对象（包括未激活）
        /// </summary>
        /// <param name="bOnlyRoot">是否只查找根对象</param>
        /// <returns></returns>
        public static List<GameObject> GetAllObjectsInScene(bool includeInactive = true, bool bOnlyRoot = true)
        {
            GameObject[] pAllObjects;
            if(includeInactive)
                pAllObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
            else
                pAllObjects = GameObject.FindObjectsOfType<GameObject>();      //只能查找到激活态的物体

            List<GameObject> pReturn = new List<GameObject>();

            foreach (GameObject pObject in pAllObjects)
            {
                //Debug.Log(pObject);
                if (bOnlyRoot)
                {
                    if (pObject.transform.parent != null)
                    {
                        continue;
                    }
                }

                if (pObject.hideFlags == HideFlags.NotEditable || pObject.hideFlags == HideFlags.HideAndDontSave)
                {
                    continue;
                }


                pReturn.Add(pObject);
            }

            return pReturn;
        }


    }
}
