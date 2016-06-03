using UnityEngine;
using System.Collections;

namespace BT {

	/// <summary>
	/// BTTree is where the behavior tree should be constructed.
	/// </summary>
	public abstract class BTTree : MonoBehaviour {
		private BTNode _root;
		protected BTDatabase _database;

		public BTNode root {get {return _root;}}

        protected bool _useAnimEvent = false;
        protected AnimEvent _animEvent;

		void Start () {
			_root = Init();

			if (_root.name == null) {
				_root.name = "Root";
			}
			_root.Activate(_database);
		}
		

		void Update () {
			_root.Tick();
		}

		/// <summary>
		/// Init this tree by constructing the behavior tree.
		/// Root node should be returned.
		/// </summary>
		public virtual BTNode Init () {
			_database = GetComponent<BTDatabase>();
			if (_database == null) {
				_database = gameObject.AddComponent<BTDatabase>();
			}

            //每个行为树上挂一个动画事件的接收器
            if(_useAnimEvent)
            {
                _animEvent = GetComponent<AnimEvent>();
                if (_animEvent == null)
                {
                    _animEvent = gameObject.AddComponent<AnimEvent>();
                }
            }
           
			return null;
		}

        /// <summary>
        /// 有些操作必须在LateUpdate中做
        /// </summary>
        void LateUpdate()
        {
            _root.LateTick();
        }
	}

}