using UnityEngine;
using System.Collections;

namespace BT.Ex {

	public class BTCheckWithinDistance : BTConditional {
		private string _readDataName;
		private int _readDataId;
		private DataOpt _dataOpt;
		private float _distance;
		private Transform _targetTrans;
		private Transform _trans;

        private Vector3 _targetPos;

        private float _tolerance;
		public BTCheckWithinDistance (Transform trans, float distance, string readDataName, DataOpt dataOpt) {
			_trans = trans;
			_distance = distance;
			_readDataName = readDataName;
			_dataOpt = dataOpt;
		}

        public BTCheckWithinDistance(Transform trans, float distance, Transform targetTrans, float tolerance)
            : this(trans, distance, null, DataOpt.ProvidedTrans)
        {
			_targetTrans = targetTrans;
            _tolerance = tolerance;
		}
// 
//         //增加一个构造，加两个属性，用来获得点击位置
//         public BTCheckWithinDistance (Transform trans, float distance, string readDataName, DataOpt dataOpt)
//             :this(trans,distance,readDataName,dataOpt)
//         {
//         }

		public override void Activate (BTDatabase database) {
			base.Activate (database);

			if (_dataOpt != DataOpt.ProvidedTrans) {
                if(_database.ContainsData(_readDataName))
				    _readDataId = _database.GetDataId(_readDataName);
			}

            if (_dataOpt == DataOpt.FixedPosition)
            {
                _targetPos = _trans.position;
            }
            
		}

		public override bool Check () {
			Vector3 dataPosition = Vector3.zero;

			switch (_dataOpt) {
			case DataOpt.Vec3:
				dataPosition = _database.GetData<Vector3>(_readDataId);
				break;
			case DataOpt.Trans:
				Transform trans = _database.GetData<Transform>(_readDataId);
				dataPosition = trans.position;
				break;
			case DataOpt.ProvidedTrans:
				dataPosition = _targetTrans.position;
				break;
            case DataOpt.TouchPosition:           
                if(_database.ContainsData(_readDataName))
                {
                    _targetPos = _database.GetData<Vector3>(_readDataName);
                    _database.SetData<Vector3>("TouchPosition", _targetPos);
                    //lookAdjust(_targetPos);

                    _database.RemoveData(_readDataName);
                }
                
                dataPosition = _targetPos;
                break;
            case DataOpt.FixedPosition:
                dataPosition = _targetPos;
                break;
			}

			Vector3 offset = dataPosition - _trans.position;
            return (offset.sqrMagnitude <= _distance * _distance) && (offset.sqrMagnitude >= _tolerance * _tolerance);
           
		}

        /// <summary>
        /// 调整朝向
        /// </summary>
        /// <param name="targetPos"></param>
        private void lookAdjust(Vector3 targetPos)
        {
            var pos = new Vector3(targetPos.x, _trans.position.y, targetPos.z);
            _trans.LookAt(pos);
            Vector3 eulerAngles = _trans.eulerAngles;
            _trans.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y + 180, eulerAngles.z);
        }


		public enum DataOpt {
			Vec3,
			Trans,
			ProvidedTrans,
            TouchPosition,
            FixedPosition
		}

        

	}
}