using System;
using UnityEngine;

public class AbstractView : MonoBehaviour {
    
    public Action OnAwake = empty;
    public Action OnStart = empty;
    public Action OnUpdate = empty;
    public Action OnFixedupdate = empty;
    public Action OnRelease = empty;
    public Action OnLateUpdate = empty;
    public Action OnQuit = empty;
    public Action OnLevelLoaded = empty;
    public Action<Collision> OnCollisionBegin = empty;
    public Action<Collision> OnCollisionEnd = empty;
    public Action<Collider> OnTriggerBegin = (obj)=>{};
    public Action<Collider> OnTriggerEnd = (obj)=>{};
    public Action<bool> OnPause;
	public Action<bool> OnFocus;
    
    protected Action mainThreadDelegate = empty;
    protected System.Collections.Generic.LinkedList<TimeCallback> cdic;
    
    public virtual void OnLevelWasLoaded(int level){
        if(OnLevelLoaded != null)OnLevelLoaded();
    }
        
    public virtual void Awake () {
        if (OnAwake != null)
            OnAwake ();
    }
        
    public virtual void Start () {
        if (OnStart != null)
            OnStart ();
    }
        
    public virtual void Update () {
        if (OnUpdate != null)
            OnUpdate ();
        //
        lock(mainThreadDelegate){
            if (mainThreadDelegate != empty)
            {
                mainThreadDelegate();
                mainThreadDelegate = empty;
            }           
        }
    }
    
    public virtual void OnDestroy(){
        if(OnRelease != null){
            OnRelease();
        }
    }
        
    public virtual void FixedUpdate () {
        if (OnFixedupdate != null)
            OnFixedupdate ();
    }
    
    public virtual void LateUpdate(){
        if(OnLateUpdate != null)OnLateUpdate();
    }
    
    public virtual void OnCollisionEnter(Collision collision) {
        if(OnCollisionBegin != null)OnCollisionBegin(collision);
    }
    
    public virtual void OnCollisionExit(Collision collision) {
        if(OnCollisionEnd != null)OnCollisionEnd(collision);
    }
    
    public virtual void OnTriggerEnter(Collider other) {
        OnTriggerBegin(other);
    }
    
    public virtual void OnTriggerExit(Collider other){
        OnTriggerEnd(other);
    }
    
    public virtual void OnApplicationPause(bool paused){
        if(OnPause != null)OnPause(paused);
    }
	public virtual void OnApplicationFocus(bool focused)
	{
		if(OnFocus!=null)
		{
			OnFocus(focused);
		}
	}
	
    
    public virtual void OnApplicationQuit(){
        OnQuit();
    }
    
    private static void empty(){}
    
    private static void empty(Collision c){}
    
    public void CallDelay<T>(Action<T> callback,float delay,T obj){
        StartCoroutine (delayFunc<T> (callback,delay,obj));
    }
    
    private System.Collections.IEnumerator delayFunc<T>(Action<T> callback,float delay,T obj){
        yield return new WaitForSeconds(delay);
        callback(obj);
    }
    
    public void CallDelay(Action callback,float delay){
//        StartCoroutine (delayFunc (callback,delay));
        if(callback != null){
            TimeCallback tc = new TimeCallback();
            tc.ac = callback;
            tc.time = Time.time + delay;
            tc.deleted = false;
            if(cdic == null){
                cdic = new System.Collections.Generic.LinkedList<TimeCallback>();
            }
            cdic.AddLast (tc);
            StartCoroutine (delayFunc2 (tc));
        }
    }
    
    public void StopCallFunc(Action callback){
        if(callback == null || cdic == null)return;
        TimeCallback tc = getTc (callback);
        if(tc != null){
            tc.deleted = true;
        }
    }
	
	public void StopAllCallFunc(){
		if(cdic == null)return;
		foreach(TimeCallback tb in cdic){
			tb.deleted = true;
		}
	}
    
    private System.Collections.IEnumerator delayFunc(Action callback,float delay){
        yield return new WaitForSeconds(delay);
        callback();
    }
    
    private System.Collections.IEnumerator delayFunc2(TimeCallback tc){
        if(tc.deleted){
            tc.time = -1;
        }
        while(Time.time < tc.time){
            yield return 1;
        }
        if(!tc.deleted)tc.ac();
        if(cdic != null){
            cdic.Remove (tc);
        }
    }
    
    public void Attach(Action callback){
        if(callback != null){
            lock(mainThreadDelegate){
                mainThreadDelegate += callback;
            }
        }
    }
    
    private TimeCallback getTc(Action ac){
        if(cdic == null)return null;
        foreach(TimeCallback t in cdic){
            if(ac == t.ac){
                return t;
            }
        }
        return null;
    }
}

public class TimeCallback{
    public Action ac = null;
    public float time = 0;
    public bool deleted = false;
}

