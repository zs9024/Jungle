using UnityEngine;
using System.Collections;

namespace BT.Ex
{
    /// <summary>
    /// 检查某些按键或者鼠标或者触摸条件
    /// 按键事件的类型以及该事件的相关数据存到database
    /// by zs
    /// </summary>
    public class BTCheckKeyOrTouch : BTConditional
    {
        private string _dataName ;
        private int _readDataId;
        private ActionExecution _ae = ActionExecution.None;

        private KeyOpt _keyOpt = KeyOpt.None;
        public BTCheckKeyOrTouch(ActionExecution ae)
        {
            _ae = ae;
        }

        public override void Activate(BTDatabase database)
        {
            base.Activate(database);

            _dataName = BTConditionOpt.KeyCode.ToString();
        }

        public override bool Check()
        {
            if (Input.GetKey(KeyCode.W))
            {
                _keyOpt = KeyOpt.GetKeyW;
                _database.SetData<KeyOpt>(_dataName, _keyOpt);
                return true;
            }

            if (Input.GetKey(KeyCode.S))
            {
                _keyOpt = KeyOpt.GetKeyS;
                _database.SetData<KeyOpt>(_dataName, _keyOpt);
                return true;
            }

            if (Input.GetKey(KeyCode.A))
            {
                _keyOpt = KeyOpt.GetKeyA;
                _database.SetData<KeyOpt>(_dataName, _keyOpt);
                return true;
            }

            if (Input.GetKey(KeyCode.D))
            {
                _keyOpt = KeyOpt.GetKeyD;
                _database.SetData<KeyOpt>(_dataName, _keyOpt);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 满足该条件时动作执行的时长
        /// </summary>
        public enum ActionExecution
        {
            None,
            OneTime,
            Duration
        }
    }

    public enum KeyOpt
    {
        None,
        GetKeyW,
        GetKeyS,
        GetKeyA,
        GetKeyD,
        MouseDown0,         //左键按下
        MouseDown1,          //右键安县
        MovingJoyStick
    }
}
