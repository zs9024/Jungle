using Common.Event;
using Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BT.Ex
{
    //受到攻击伤害的行为，先不加检测
    public class BTActionOnDamaged : BTAction
    {
        private bool beAttacking = false;

        public BTActionOnDamaged()
        {

        }

        public override void Activate(BTDatabase database)
        {
            base.Activate(database);
            
        }

        protected override void Enter()
        {
            base.Enter();
        }

        protected override BTResult Execute()
        {

            return BTResult.Success;
        }

        

    }
}
