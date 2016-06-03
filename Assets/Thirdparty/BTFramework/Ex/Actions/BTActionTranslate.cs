using UnityEngine;
using System.Collections;

namespace BT.Ex
{

    public class BTActionTranslate : BTAction
    {
        private Transform _trans;
        private float _moveSpeed;
        private Vector3 _direction;

        private BTDataReadOpt _dataReadOpt;

        private Vector3 euler;

        public BTActionTranslate(Transform trans, float moveSpeed, BTDataReadOpt dataReadOpt)
        {
            this._trans = trans;
            this._moveSpeed = moveSpeed;
            this._dataReadOpt = dataReadOpt;

            euler = _trans.eulerAngles;
        }

        public BTActionTranslate(Transform trans, Vector3 direction, float moveSpeed, BTDataReadOpt dataReadOpt)
            : this(trans, moveSpeed, dataReadOpt)
        {
            this._direction = direction;          
        }

        protected override BTResult Execute()
        {
            KeyOpt keyOpt = KeyOpt.None;
            if (_dataReadOpt == BTDataReadOpt.ReadEveryTick)
            {
                keyOpt = _database.GetData<KeyOpt>(BTConditionOpt.KeyCode.ToString());
            }

            switch(keyOpt)
            {
                case KeyOpt.GetKeyW:
                    _trans.eulerAngles = new Vector3(euler.x, 0, euler.z);
                    _trans.Translate(Vector3.back * Time.deltaTime * _moveSpeed);
                    break;

                case KeyOpt.GetKeyS:
                    _trans.eulerAngles = new Vector3(euler.x, euler.y + 180, euler.z);
                    _trans.Translate(Vector3.back * Time.deltaTime * _moveSpeed);
                    break;

                case KeyOpt.GetKeyA:
                    _trans.eulerAngles = new Vector3(euler.x, euler.y + 90, euler.z);
                    _trans.Translate(Vector3.back * Time.deltaTime * _moveSpeed);
                    break;

                case KeyOpt.GetKeyD:
                    _trans.eulerAngles = new Vector3(euler.x, euler.y - 90, euler.z);
                    _trans.Translate(Vector3.back * Time.deltaTime * _moveSpeed);
                    break;

                case KeyOpt.MovingJoyStick:
                    MovingJoystick move = _database.GetData<MovingJoystick>("MovingJoystick");
                    if (move != null)
                    {
                        ////获取摇杆偏移量
                        //float joyPositionX = move.joystickAxis.x;
                        //float joyPositionY = move.joystickAxis.y;
                        //if (joyPositionY != 0 || joyPositionX != 0)
                        //{
                        //    //设置角色的朝向（朝向当前坐标+摇杆偏移量）
                        //    _trans.LookAt(new Vector3(_trans.position.x + joyPositionX, _trans.position.y, _trans.position.z + joyPositionY));
                        //    //移动玩家的位置（按朝向位置移动）
                        //    _trans.Translate(-Vector3.forward * Time.deltaTime * _moveSpeed);
                        //}

                        float angle = move.Axis2Angle(true);
                        _trans.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
                        //_trans.Translate(Vector3.back * Time.deltaTime * _moveSpeed);

                        var controller = BTCharacator.CharacController;
                        if(controller != null)
                        {
                            var direction = _trans.TransformDirection(Vector3.back);
                            controller.Move(direction * Time.deltaTime * _moveSpeed);
                        }

                        UpdateGravity();
                    }
                    break;

                case KeyOpt.MouseDown0:
                    return BTResult.Failed;
            }

            return BTResult.Running;
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
