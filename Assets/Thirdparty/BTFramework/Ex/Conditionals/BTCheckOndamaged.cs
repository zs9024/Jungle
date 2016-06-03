using Common.Event;
using Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BT.Ex
{
    public class BTCheckOnDamaged : BTConditional
    {
        private bool beDamaging = false;

        private GameObject _go;

        public BTCheckOnDamaged(GameObject go)
        {
            _go = go;
        }

        public override void Activate(BTDatabase database)
        {
            base.Activate(database);

            EventDispatcher.AddEventListener<Damage>(AnimEventConfig.OnDamage, OnDamaged);
        }

        public override bool Check()
        {
            return beDamaging;
        }

        public override void Clear()
        {
            base.Clear();

            beDamaging = false;
        }

        public void OnDamaged(Damage damage)
        {
            if (damage != null && damage.Sufferers != null )
            {
                beDamaging = true;
            }
        }
    }
}
