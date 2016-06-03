using UnityEngine;
using System.Collections;

namespace BT.Ex {

	public class BTActionMove : BTAction {

		private string _readDataName;
		private int _readDataId = -1;   //初始化为-1
        protected float _speed;
        protected float _tolerance;
        protected DataOpt _dataOpt;
        protected UsageOpt _usageOpt;
		protected BTDataReadOpt _dataReadOpt;
        protected Vector3 _vec3Data;
        protected Transform _targetTrans;
        protected Transform _trans;
        protected Vector3 _originPos;

		public BTActionMove (Transform trans, float speed, float tolerance, string readDataName, DataOpt dataOpt, UsageOpt usageOpt, BTDataReadOpt dataReadOpt) {
			_trans = trans;
			_speed = speed;
			_tolerance = tolerance;
			_readDataName = readDataName;
			_dataOpt = dataOpt;
			_usageOpt = usageOpt;
			_dataReadOpt = dataReadOpt;
		}

		public BTActionMove (Transform trans, float speed, float tolerance, Transform targetTrans, BTDataReadOpt dataReadOpt) : 
		this (trans, speed, tolerance, null, DataOpt.ProvidedTrans, UsageOpt.Position, dataReadOpt) {
			_targetTrans = targetTrans;
		}

		public override void Activate (BTDatabase database) {
			base.Activate (database);

			if (_dataOpt != DataOpt.ProvidedTrans) {
                if (_database.ContainsData(_readDataName))
				    _readDataId = _database.GetDataId(_readDataName);
			}

            if (_dataOpt == DataOpt.FixedPosition)
            {
                _originPos = _trans.position;
            }
		}

		protected override void Enter () {
			base.Enter ();

			if (_dataReadOpt == BTDataReadOpt.ReadAtBeginning) {
				ReadVec3Data();
			}
		}

		protected override BTResult Execute () {
			if (_dataReadOpt == BTDataReadOpt.ReadEveryTick) {
				ReadVec3Data();
			}

			Vector3 direction = Vector3.zero;

			switch (_usageOpt) {
			case UsageOpt.Direction:
				direction = _vec3Data;
				break;
			case UsageOpt.Position:
				direction = _vec3Data - _trans.position;
				break;
			}

			if (direction.sqrMagnitude <= _tolerance * _tolerance) {
				return BTResult.Success;
			}
			else {
				Vector3 position = _trans.position;
				position += direction.normalized * _speed * Time.deltaTime;
				_trans.position = position;
			}
			return BTResult.Running;
		}

		protected void ReadVec3Data () {
			_vec3Data = Vector3.zero;
			
			switch (_dataOpt) {
			case DataOpt.Vec3:
                    if (_readDataId != -1)
                    {
                        _vec3Data = _database.GetData<Vector3>(_readDataId);
                    }
                    else
                    {
                        _vec3Data = _database.GetData<Vector3>(_readDataName);
                    }
				break;
			case DataOpt.Trans:
                if (_readDataId != -1)
				    _vec3Data = _database.GetData<Transform>(_readDataId).position;
				break;
			case DataOpt.ProvidedTrans:
				_vec3Data = _targetTrans.position;
				break;
            case DataOpt.FixedPosition:
                _vec3Data = _originPos;
                break;
			}
		}

		public enum DataOpt {
			Vec3,
			Trans,
			ProvidedTrans,
            FixedPosition
		}

		public enum UsageOpt {
			Position,
			Direction,
		}
	}

}