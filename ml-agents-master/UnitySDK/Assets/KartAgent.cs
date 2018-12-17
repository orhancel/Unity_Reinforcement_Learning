using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class KartAgent : Agent
{
    private KartAcademy myAcademy;
    private Vector3 previous;
    private float velocity;
    public float speed;
    public float rotationSpeed;
    public float visibleDistance;
    public GameObject startPos;
    public bool crash = false;
    private List<GameObject> barriers=new List<GameObject>();

    private float Round(float x)
    {
        return (float)System.Math.Round(x,1);
    }

    public override void InitializeAgent()
    {
        previous = this.transform.position;
        myAcademy = GameObject.Find("KartAcademy").GetComponent<KartAcademy>();
        previous = transform.position;
    }

    /// <summary>
    /// We collect the normalized rotations, angularal velocities, and velocities of both
    /// limbs of the reacher as well as the relative position of the target and hand.
    /// </summary>
    public override void CollectObservations()
    {
        for(int i=10;i>-10;i--)
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(i * 9.0f, Vector3.up) * transform.forward * visibleDistance, Color.green);
        RaycastHit hit;
        //Physics.Raycast(this.transform.position, this.transform.forward, out hit, visibleDistance);
        //Debug.Log(hit.distance);
        for (int i = 10; i >= -10; i--)
        {
            if (Physics.Raycast(this.transform.position, 
                Quaternion.AngleAxis(i*9.0f, Vector3.up) * this.transform.forward, out hit, visibleDistance))
            {
                
                AddVectorObs(1 - Round(hit.distance / visibleDistance));
      //  
            }
            else
            {
                AddVectorObs(0);
            }
        }
        velocity = ((transform.position - previous).magnitude) / Time.deltaTime;
        previous = transform.position;
        AddVectorObs(velocity);

    }



/// <summary>
/// The agent's four actions correspond to torques on each of the two joints.
/// </summary>
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(1f / agentParameters.maxStep);
        float gas = Mathf.Clamp(vectorAction[0], -1f, 1f);
        float wheel = Mathf.Clamp(vectorAction[1], -1f, 1f);

        float translation = Time.deltaTime * speed * gas;
        float rotation = Time.deltaTime * rotationSpeed * wheel;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

    }

    /// <summary>
    /// Used to move the position of the target goal around the agent.
    /// </summary>
   /* void UpdateGoalPosition()
    {
        var radians = goalDegree * Mathf.PI / 180f;
        var goalX = 8f * Mathf.Cos(radians);
        var goalY = 8f * Mathf.Sin(radians);
        goal.transform.position = new Vector3(goalY, -1f, goalX) + transform.position;
    }*/

    /// <summary>
    /// Resets the position and velocity of the agent and the goal.
    /// </summary>
    public override void AgentReset()
    {
        this.transform.position = startPos.transform.position;
        this.transform.rotation = startPos.transform.rotation;
        crash = false;
        velocity = 0;
        previous = startPos.transform.position;
        for(int i = 0; i < barriers.Count; i++)
        {
            barriers[i].SetActive(true);
            //Debug.Log("reset");
            //Debug.Log(barriers[i]);
        }
        barriers.Clear();
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("carp");
        Debug.Log(other.gameObject.transform.name);
        if (other.gameObject.CompareTag("Finish"))
        {
            SetReward(2f);
            Done();
            //Debug.Log("Finish");
        }
        else if (other.gameObject.CompareTag("checkpoint"))
        {
            //Debug.Log("checkpoint");
            barriers.Add(other.gameObject);
            SetReward(1f);
            //other.gameObject.tag = "terrain";
            other.gameObject.SetActive(false);
           
        }
        else
        {
            SetReward(-1f);
            Done();
            Debug.Log("Crash");
            crash = true;
        }
    }
  


    // Use this for initialization
    /*void Start () {
		
	}*/

    // Update is called once per frame
    /*void Update () {

        float translation = Time.deltaTime * speed * 0.05f;
        float rotation = Time.deltaTime * rotationSpeed * 0f;
        transform.Translate(0, 0, translation);

        transform.Rotate(0, rotation, 0);
        Debug.DrawRay(transform.position, transform.forward * 10.0f, Color.red);
        for (int i = 10; i > -10; i--)
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(i * 9.0f, Vector3.up) * transform.forward * 10.0f, Color.green);
    }*/
}
