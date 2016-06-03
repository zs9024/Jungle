using UnityEngine;
using System.Collections;

namespace BT.Ex
{
    public class BTActionCharacterMove : BTActionMove
    {
        private CharacterController controller;
        public BTActionCharacterMove(Transform trans, float speed, float tolerance, string readDataName
            , DataOpt dataOpt, UsageOpt usageOpt, BTDataReadOpt dataReadOpt)
            : base(trans, speed, tolerance, readDataName, dataOpt, usageOpt,dataReadOpt)
        {
            
        }

        public BTActionCharacterMove(Transform trans, float speed, float tolerance, Transform targetTrans, BTDataReadOpt dataReadOpt)
            : base(trans,speed,tolerance,targetTrans,dataReadOpt)
        {
            
        }


        public override void Activate (BTDatabase database)
        {
            base.Activate(database);

            this.controller = _database.transform.GetComponent<CharacterController>();
        }

        protected override void Enter()
        {
            base.Enter();
        }

        protected override BTResult Execute ()
        {
            if (_dataReadOpt == BTDataReadOpt.ReadEveryTick)
            {
                ReadVec3Data();
            }

            Vector3 direction = Vector3.zero;

            switch (_usageOpt)
            {
                case UsageOpt.Direction:
                    direction = _vec3Data;
                    break;
                case UsageOpt.Position:
                    direction = _vec3Data - _trans.position;
                    break;
            }

            if (direction.sqrMagnitude <= _tolerance * _tolerance)
            {
                //Debug.Log("BTActionCharacterMove direction= " + direction);
                return BTResult.Success;
            }
            else
            {
                if (controller != null)
                {
                    if (_dataOpt == DataOpt.FixedPosition)
                    {
                        lookAdjust(_originPos);
                    }
                    else
                    {
                        lookAdjust(_targetTrans.position);
                    }
                    controller.Move(direction.normalized * Time.deltaTime * _speed);
                }
            }

            return BTResult.Running;
        }

        /// <summary>
        /// 调整朝向
        /// </summary>
        /// <param name="targetPos"></param>
        private void lookAdjust(Vector3 targetPos)
        {
            var pos = new Vector3(targetPos.x, _trans.position.y, targetPos.z);
            _trans.LookAt(pos);
            Vector3 eulerAngles = _trans.eulerAngles;
            _trans.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, eulerAngles.z);
        }

    }

}
