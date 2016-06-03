using UnityEngine;
using System.Collections;
using System;

public class AnimTest : MonoBehaviour
{

    Animator animator;

    Animation animation;

    public AnimationClip clip;
    public GameObject enemy;

    AnimEvent animEvent;
    TrailWeapon trail;

    void Start()
    {
        animator = GetComponent<Animator>();
        //animatorTest();
        //animEvent.AddAnimEvent(enemy, clip, 0.5f, onAttack2);

        trail = GetComponent<TrailWeapon>();

        animEvent = GetComponent<AnimEvent>();
        if(animEvent == null)
            animEvent = gameObject.AddComponent<AnimEvent>();

//         animEvent.AddAnimEvent(clip, 0.2f, trail.TrailStart);
//         animEvent.AddAnimEvent(clip, 0.6f, trail.TrailStop);

        //animEvent.AddAnimEvent3(enemy, clip, 0.2f, onAttack2);
        //animEvent.AddAnimEvent3(enemy, clip, 0.6f, onAttack3);
    }

    void animatorTest()
    {
        AnimatorClipInfo[] infos = animator.GetCurrentAnimatorClipInfo(0);
        Debug.Log(infos.Length);
    }
    public void OnAnimEvent22222()
    {
        Debug.Log("AnimTest OnAnimEvent22222...");
    }

   public void onAttack()
    {
        Debug.Log("onAttack...");
    }

   public void onDeath()
   {
       Debug.Log("onDeath...");
   }

   public void onAttack2(object obj)
   {
       GameObject go = obj as GameObject;
       Debug.Log("onAttack2 ..." + go);
   }

   public void onAttack3(object obj)
   {
       GameObject go = obj as GameObject;
       Debug.Log("onAttack3 ..." + go);
   }

    // Update is called once per frame
    void Update()
    {

//         int mouseButton = -1;
//         if (Input.GetMouseButtonDown(0))
//         {
//             mouseButton = 0;
//         }
//         else if (Input.GetMouseButtonDown(1))
//         {
//             mouseButton = 1;
//         }
// 
//         if (mouseButton != -1)
//         {
//             Vector3 mousePosition = Vector3.zero;//= SHelper.GetMousePositionInWorld();
// 
//             if (mouseButton == 0)
//             {
//                 animator.Play("run");
//                 UpdateFaceDirection(mousePosition);
//             }
//             else if (mouseButton == 1)
//             {
// 
//             }
//         }
// 
//         if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
//         {
//             Application.LoadLevel(0);
//         }
// 
//         if (Input.GetKeyDown(KeyCode.Escape))
//         {
//             Application.Quit();
//         }

        if (animator != null)
            animator.Play("attack2");
        //if (clip == null)
            getAnimation();
    }

    bool hasSet;
    void getAnimation()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.nameHash == Animator.StringToHash("Base Layer.attack2"))
        {
            if (!hasSet)
            {                
                AnimatorClipInfo[] info = animator.GetCurrentAnimatorClipInfo(0);

                foreach (AnimatorClipInfo i in info)
                {
                    clip = i.clip;
                    if(clip.name.Contains("attack2"))
                    {
                        animEvent.AddAnimEvent(clip, 0.2f, trail.TrailStart);
                        animEvent.AddAnimEvent(clip, 0.6f, trail.TrailStop);
                        hasSet = true;
                    }                  
                }
            }
        }
     
    }

    //5.0可用
    //public static void RemoveEventFromAnimator(string functionName, Animator animator)
    //{
    //    AnimatorClipInfo animationClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
    //    AnimationClip animationClip = animationClipInfo.clip;

    //    AnimationEvent[] animationEvents = animationClip.events;
    //    List<AnimationEvent> updatedAnimationEvents = new List<AnimationEvent>();

    //    for (int i = 0; i < animationEvents.Length; i++)
    //    {
    //        AnimationEvent animationEvent = animationEvents;
    //        if (animationEvent.functionName != functionName)
    //        {
    //            updatedAnimationEvents.Add(animationEvent);
    //        }
    //    }

    //    animationClip.events = updatedAnimationEvents.ToArray();
    //}


    private void UpdateFaceDirection(Vector3 _destination)
    {
        Vector3 offset = _destination - transform.position;

        float rotationSpeed = 1f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(offset - transform.position), rotationSpeed * Time.deltaTime);
        //         var go = new GameObject("go");
        //         go.transform.position = offset;
        //         transform.LookAt(go.transform);

        //         float ry = transform.rotation.y;
        //         double b = ry * Math.PI / 180;//转化为弧度值
        //         Vector3 d0 = new Vector3(-(float)System.Math.Sin(b), 0, -(float)System.Math.Cos(b));
        //         Debug.Log(d0);

        //         if (offset.x >= 0)
        //         {
        //             transform.localEulerAngles = new Vector3(0, 180, 0);
        //         }
        //         else
        //         {
        //             transform.localEulerAngles = Vector3.zero;
        //         }
    }
}

