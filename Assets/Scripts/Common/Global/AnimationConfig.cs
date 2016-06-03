using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Anim
{
    public class AnimationConfig
    {
        public static Dictionary<string, int> AnimNameStateDic = new Dictionary<string, int>();

        public static bool SetAnimNameState(string name,int state)
        {
            if(string.IsNullOrEmpty(name))
            {
                return false;
            }

            if(!AnimNameStateDic.ContainsKey(name))
            {
                AnimNameStateDic.Add(name, state);
            }

            return true;
        }

        public static int GetAnimState(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }

            if (!AnimNameStateDic.ContainsKey(name))
            {
                return -1;
            }

            return AnimNameStateDic[name];
        }
    }
}
