using UnityEngine;
using System.Collections;

public class AnimCtrlTest : MonoBehaviour {

    private Animator animator;
    AnimatorStateInfo animState;

    AnimEvent animEvent;
    TrailWeapon trail;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();

        animEvent = gameObject.GetComponent<AnimEvent>();
        if (animEvent == null)
        {
            animEvent = gameObject.AddComponent<AnimEvent>();
        }

        trail = gameObject.GetComponent<TrailWeapon>();
        if (trail == null)
        {
            trail = gameObject.AddComponent<TrailWeapon>();
            trail.TrailObj = gameObject.transform.Find2("weapon_trail", false).GetComponent<WeaponTrail>();
        }
	}
	
	// Update is called once per frame
    void Update()
    {
        animState = animator.GetCurrentAnimatorStateInfo(0);
        if (animState.IsName("Base Layer.attack"))
        {
            float endTime = Mathf.Repeat(animState.normalizedTime, 1.0f);
            if (endTime >= 0.9f)
            {
                animator.SetInteger("State", 2);
            }
            addCurrentAnimAttackTrail();

            
        }

    }


    private bool _hasSetAnimEvent = false;
    private float breakTime;
    /// <summary>
    /// 给当前的攻击动画增加事件，实现WeaponTrail
    /// </summary>
    private void addCurrentAnimAttackTrail()
    {
        if (!_hasSetAnimEvent)
        {
            AnimatorClipInfo[] info = animator.GetCurrentAnimatorClipInfo(0);
            foreach (AnimatorClipInfo i in info)
            {
                var clip = i.clip; 
                //Debug.Log(clip.name + "; " + clip.frameRate + " ;" + clip.length);
                if (clip.name.Contains("attack"))
                {
                     breakTime = Mathf.Repeat(animState.normalizedTime, 1.0f);
                     Debug.Log(breakTime);
                     StartCoroutine(setEvent(0.1f));
                     //加刀光
                    animEvent.AddAnimEvent(clip, 0.41f, trail.TrailStart);
                    animEvent.AddAnimEvent(clip, 0.6f, trail.TrailStop);
                    
                    animEvent.AddAnimEvent(clip, 0.8f, voidEvent);
                    //animEvent.AddAnimEvent(null, clip, 0.51f, attackEvent);

                    

                    

                    _hasSetAnimEvent = true;
                }
            }
        }
    }

    void LateUpdate()
    {
//         if (!_hasSetAnimEvent)
//         {
//             animator.SetInteger("State", 5);
//         }

        //Debug.LogWarning( animator.GetInteger("State"));
    }

    private IEnumerator setEvent(float t)
    {
        yield return null;

        animator.CrossFade(Animator.StringToHash("Base Layer." + "attack"), 0.01f, 0, breakTime - 2*Time.deltaTime);
    }
    private void attackEvent(object param)
    {
        if (param == null)
        {
            return;
        }

        Debug.Log("attackEvent");
    }

     private void voidEvent()
    {
        Debug.Log("voidEvent");
    }

    private bool firstTime = true;
    void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,100,50),"Attack"))
        {
            //if(firstTime)
            //{
            //    animator.CrossFade(Animator.StringToHash("Base Layer." + "attack2"),0f);
            //    //animator.Play("attack2");
            //    firstTime = false;
            //}
            //else
            animator.SetInteger("State", 5);
        }

        if (GUI.Button(new Rect(0, 50, 100, 50), "Run"))
        {
            animator.SetInteger("State", 3);
        }
        if(GUI.Button(new Rect(0,100,100,50),"Check"))
        {
            if (animState.nameHash == Animator.StringToHash("Base Layer." + "attack2"))
            {
                float endTime = Mathf.Repeat(animState.normalizedTime, 1.0f);
                Debug.Log("Check" + endTime);
            }

            if (animState.IsName("Base Layer." + "attack2"))
            {
                //Debug.Log("Check2");
            }
        }
    }
}
