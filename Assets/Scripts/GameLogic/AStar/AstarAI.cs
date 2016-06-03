using UnityEngine;
using System.Collections;
//Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors  
//This line should always be present at the top of scripts which use pathfinding  
using Pathfinding;
public class AstarAI : MonoBehaviour
{
    //The point to move to  
    public Vector3 targetPosition;

    private Seeker seeker;
    private CharacterController controller;

    //The calculated path  
    public Path path;

    //The AI's speed per second  
    public float speed = 200;

    //The max distance from the AI to a waypoint for it to continue to the next waypoint  
    public float nextWaypointDistance = 3;

    //The waypoint we are currently moving towards  
    private int currentWaypoint = 0;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();

        //Start a new path to the targetPosition, return the result to the OnPathComplete function  
        //seeker.StartPath(transform.position, targetPosition, OnPathComplete);  
        //seeker.pathCallback += OnPathComplete;  
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            path = p;
            //Reset the waypoint counter  
            currentWaypoint = 0;
        }
    }

    void Update()
    {
        // see if user pressed the mouse down  
        if (Input.GetMouseButtonDown(0))
        {
            // We need to actually hit an object  
            RaycastHit hit;
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                return;
            // We need to hit something (with a collider on it)  
            if (!hit.transform)
                return;

            // Get input vector from kayboard or analog stick and make it length 1 at most  
            targetPosition = hit.point;

            seeker.StartPath(transform.position, targetPosition);
        }
    }

    public void FixedUpdate()
    {
        if (path == null)
        {
            //We have no path to move after yet  
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log("End Of Path Reached");
            return;
        }

        //Direction to the next waypoint  
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;
        controller.SimpleMove(dir);

        //Check if we are close enough to the next waypoint  
        //If we are, proceed to follow the next waypoint  
        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }
}