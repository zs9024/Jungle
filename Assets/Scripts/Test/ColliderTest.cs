using UnityEngine;
using System.Collections;

public class ColliderTest : MonoBehaviour {


    Animator animator;
    void Start()
    {
//         var vec3 = transform.TransformDirection(Vector3.forward);
//         Debug.Log(vec3);
// 
//         var wpos = transform.TransformPoint(Vector3.zero);
//         Debug.Log(wpos);

        //获取角色控制器对象
        CharacterController controller = GetComponent<CharacterController>();

        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter( Collider other )
    {
        Debug.Log("OnTriggerEnter");
    }

    void OnTriggerExit( Collider other )
    {
        Debug.Log("OnTriggerExit");
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay");
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        Debug.Log("OnCollisionEnter");
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        Debug.Log("OnCollisionExit");
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {

        //得到接收碰撞名称
        GameObject hitObject = hit.collider.gameObject;

        Debug.Log(hitObject);

        var body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * 2;
    }

    //这里测试射线碰撞
    void Update()
    {

        if (animator != null)
            animator.Play("attack2");

        //          if(Physics.Raycast(transform.position, Vector3.right))
        //          {
        //              Debug.Log("Raycast");
        //          }
        //         Debug.DrawLine(transform.position, new Vector3(10,0,4.1f), Color.red, 2);

        //         RaycastHit hitinfo;
        //         if(Physics.Raycast(transform.position, Vector3.right, out hitinfo))
        //         {
        //             Debug.Log(hitinfo.transform);
        //             Debug.DrawLine(transform.position, hitinfo.point, Color.red, 2);
        //            }

        //检测所有
        //RaycastHit[] hitinfos = Physics.RaycastAll(transform.position, Vector3.right, 100);
        //foreach(RaycastHit hit in hitinfos)
        //{
        //    Debug.Log(hit.transform);
        //    Debug.DrawLine(transform.position, hit.point, Color.red, 2);
        //}      
        

        //层检测
        //1 << 10  打开第10的层。
        //~(1 << 10) 打开除了第10之外的层。
        //~(1 << 0) 打开所有的层。
        //(1 << 10) | (1 << 8) 打开第10和第8的层。

//         LayerMask mask = 1 << (LayerMask.NameToLayer("Plane"));
//         RaycastHit hitinfo;
//         if (Physics.Raycast(transform.position, Vector3.right, out hitinfo,100,mask.value))
//         {
//             Debug.Log(hitinfo.transform);
//             Debug.DrawLine(transform.position, hitinfo.point, Color.red, 2);
//         }

        //检测球形范围的所有碰撞器
        //LayerMask mask = 1 << (LayerMask.NameToLayer("Plane"));
        //Collider[] cols = Physics.OverlapSphere(transform.position, 7, mask.value);
        //foreach (Collider col in cols)
        //{
        //    Debug.Log(col.transform);
        //    Debug.DrawLine(transform.position, col.transform.position, Color.red, 2);
        //} 


//         LayerMask mask = 1 << (LayerMask.NameToLayer("Plane"));
//         if(Physics.CheckSphere(transform.position, 7, mask.value))
//         {
//             Debug.Log("CheckSphere");
//         }
    }


    
}
