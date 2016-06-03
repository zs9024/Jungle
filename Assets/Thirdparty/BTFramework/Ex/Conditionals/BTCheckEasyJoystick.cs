using UnityEngine;
using System.Collections;

namespace BT.Ex
{
    /// <summary>
    /// 检测是否移动了摇杆
    /// by zs
    /// </summary>
    public class BTCheckEasyJoystick : BTConditional
    {
        private bool checkedMoving = false;

        public override void Activate(BTDatabase database)
        {
            base.Activate(database);

            EasyJoystick.On_JoystickMove += OnJoystickMove;
            EasyJoystick.On_JoystickMoveEnd += OnJoystickMoveEnd;
        }

        //当摇杆不可用时移除事件
        void OnDisable()
        {
            EasyJoystick.On_JoystickMove -= OnJoystickMove;
            EasyJoystick.On_JoystickMoveEnd -= OnJoystickMoveEnd;
        }

        //当摇杆销毁时移除事件
        void OnDestroy()
        {
            EasyJoystick.On_JoystickMove -= OnJoystickMove;
            EasyJoystick.On_JoystickMoveEnd -= OnJoystickMoveEnd;
        }

        public override bool Check()
        {
            return checkedMoving;
        }


        void OnJoystickMoveEnd(MovingJoystick move)
        {
            Debug.Log("OnJoystickMoveEnd : " + move.joystickName);
            if (move.joystickName == "Movejoystick")
            {
                checkedMoving = false;
                _database.RemoveData("KeyCode");
                _database.RemoveData("MovingJoystick");
            }
        }

        
        void OnJoystickMove(MovingJoystick move)
        {
            if (move.joystickName == "Movejoystick")
            {
                checkedMoving = true;
                _database.SetData<KeyOpt>("KeyCode", KeyOpt.MovingJoyStick);
                _database.SetData<MovingJoystick>("MovingJoystick", move);
            }
        }



    }
}

