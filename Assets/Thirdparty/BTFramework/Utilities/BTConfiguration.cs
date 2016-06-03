using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT {

	public static class BTConfiguration {

		public static bool ENABLE_DATABASE_LOG;
	}

    

    public class TrailEvent
    {
        private float _startTime;
        private float _stopTime;
        private int _times;
        //宽度，高度等 

        public float StartTime 
        { 
            get { return _startTime; } 
        }

        public float StopTime 
        {
            get { return _stopTime; } 
        }

        public float Times
        {
            get { return _times; }
        }

        public TrailEvent(float startTime, float stopTime, int tmies = 1)
        {
            _startTime = startTime;
            _stopTime  = stopTime;
            _times     = tmies;
        }
    }
}