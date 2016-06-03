using Common.Event;
using Common.Global;
using UnityEngine;

namespace BT.Ex
{
    public class BTActionDeath : BTAction
    {
        private bool beDead = false;
        private Death _death;

        private GameObject _go;
        private Animator animator;
        private int stateHash;
        public BTActionDeath(GameObject go)
        {
            _go = go;
        }

        public override void Activate(BTDatabase database)
        {
            base.Activate(database);

            animator = _go.GetComponent<Animator>();
            stateHash = Animator.StringToHash("Base Layer" + "." + "Death");

            EventDispatcher.AddEventListener<Death>(BTreeEventConfig.OnDeath, OnDeath);
        }

        protected override void Enter()
        {
            base.Enter();
        }

        protected override BTResult Execute()
        {
            if(beDead)
            {
                AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
                if (animState.nameHash == stateHash)
                {
                    float endTime = Mathf.Repeat(animState.normalizedTime, 1.0f);
                    if (endTime >= 0.95f)
                    {
                        Debug.Log("BTActionAnimTransition Success");
                        if(_death.callback != null)
                        {
                            _death.callback();
                        }
                        
                    }
                }

                return BTResult.Success;
            }

            return BTResult.Failed;
        }

        public void OnDeath(Death death)
        {
            if (death != null && death.decedent != null && death.decedent.gObj == _go)
            {
                beDead = true;
                _death = death;
                animator.SetInteger("State", GlobalConfig.AC_STATE_Die);
            }
        }

    }
}
