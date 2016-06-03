using UnityEngine;
using System.Collections;
using Common.Event;
using Common.Global;

namespace BT.Ex
{
    /// <summary>
    /// 檢查是否按下攻擊鍵攻擊
    /// </summary>
    public class BTCheckAttack : BTConditional
    {
        private BTCheckAttackOpt _attackOpt;
        private Transform _attacker;
        private Transform _sufferer;

        private bool beAttacking = false;

        private BTCheckAttackOpt eventAttackOpt;

        public BTCheckAttack(BTCheckAttackOpt opt)
        {
            _attackOpt = opt;
        }

        public override void Activate(BTDatabase database)
        {
            base.Activate(database);

            //监测攻击事件（攻击按钮）
            EventDispatcher.AddEventListener<BTCheckAttackOpt, System.Action>(BTreeEventConfig.OnAttack, onAttacked);
        }

        public override bool Check()
        {

            return _attackOpt == eventAttackOpt;
        }

        private void onAttacked(BTCheckAttackOpt attackOpt, System.Action callback)
        {
            eventAttackOpt = attackOpt;
            beAttacking = true;

            if (_database.CheckDataNull("AtkCallback"/*attackOpt.ToString()*/))
            {
                _database.SetData<System.Action>("AtkCallback", callback);
            }
        }



        public override void Clear()
        {
            base.Clear();

            eventAttackOpt = BTCheckAttackOpt.None;
            beAttacking = false;
        }

        private IEnumerator clearManual()
        {
            yield return null;

            beAttacking = false;
        }
    }
}
