using System;
using System.Collections;
using StarterAssets;
using Unity.Burst.Intrinsics;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class Monster : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject player;
    
    private float targetRotation;
    private float timeChasing;
    private float timeInRange;
    private bool isAtPatrolPoint = false;
    private float timeSinceChase = 0;
    private float timeSinceLosingVision = 0;
    public bool canSee = false;
    public Light jumpscareLight;
    public GameObject target1;
    public GameObject randPos;
    public CinemachineBrain brain;
    private Vector3 targetPosition;
    public GameObject target2;
    public GameObject target3;
    public GameObject target4;
    public ArrayList a = new ArrayList(); 
    //public CharacterController controller;
    public int state = 1;
    public NavMeshAgent agent;
    //public Rigidbody rigid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //rigid.isKinematic = true;
        //Jumpscare();
        a.Add(target1);
        a.Add(target2);
        a.Add(target3);
        a.Add(target4);
        randPos = target1;

    }

    // Update is called once per frame
    void Update()
    {
        canSee = false;
        RaycastHit hit1;
        RaycastHit hit2;
        RaycastHit hit3;
        
        Physics.Raycast(transform.position,  Vector3.RotateTowards(Vector3.back, transform.forward, 6, 6), out hit1, 20);
        Physics.Raycast(transform.position,  Vector3.RotateTowards(new Vector3(0.25f, 0 , -0.5f).normalized, transform.forward, 6, 6), out hit2, 20);
        Physics.Raycast(transform.position, Vector3.RotateTowards(new Vector3(-0.25f, 0, 0.5f).normalized, transform.forward, 6, 6), out hit3, 20);
        //Debug.DrawRay(transform.position, Vector3.RotateTowards(Vector3.back * 20, transform.forward, 6, 6), Color.red, 1);
        //Debug.Log(gameObject.transform.eulerAngles.y);
        timeSinceLosingVision += Time.deltaTime;
        
        if (hit1.collider != null)
        {
            //Debug.Log("yes");
            if (hit1.collider.gameObject.tag == "a")
            {
                Debug.Log("waheee");
                canSee = true;
                timeSinceLosingVision = 0;
                
            }

        }
        if (hit2.collider != null)
        {
            if (hit2.collider.gameObject.tag == "a")
            {
                Debug.Log("waheee");
                canSee = true;
                timeSinceLosingVision = 0;
            }

        }
        if (hit3.collider != null)
        {
            if (hit3.collider.gameObject.tag == "a")
            {
                Debug.Log("waheee");
                canSee = true;
                timeSinceLosingVision = 0;
            }

        }
            
        timeSinceChase += Time.deltaTime;
        /*
        RaycastHit hit1;
        RaycastHit hit2;
        RaycastHit hit3;
        RaycastHit hit4;
        Physics.Raycast(transform.position, Vector3.forward, out hit1, 1);
        Physics.Raycast(transform.position + Vector3.up, Vector3.back, out hit2, 1);
        Physics.Raycast(transform.position + Vector3.up, Vector3.left, out hit3, 1);
        Physics.Raycast(transform.position + Vector3.up, Vector3.right, out hit4, 1);
        Debug.DrawRay(transform.position + Vector3.up, Vector3.forward, Color.red, 100);
        //Debug.Log(hit1.collider);
        if (hit1.collider != null)
        {
            //Debug.Log("yes");
            if (hit1.collider.gameObject.GetComponent<ThirdPersonController>())
            {
                Jumpscare();
            }

        }
        if (hit2.collider != null)
        {
            if (hit2.collider.gameObject.GetComponent<ThirdPersonController>())
            {
                Jumpscare();
            }
            
        }
        if (hit3.collider != null)
        {
            if (hit3.collider.gameObject.GetComponent<ThirdPersonController>())
            {
                Jumpscare();
            }
            
        }
        if (hit4.collider != null)
        {
            if (hit4.collider.gameObject.GetComponent<ThirdPersonController>())
            {
                Jumpscare();
            }
            
        }
        */
        if (state == 0)
        {
            Patrol();
        }
        else if (state == 1)
        {
            //Debug.Log("test");
            //Debug.Log("test");
            //Debug.Log("test");

            //Debug.Log("test");
            Chase();

        }
        else if (state == 2)
        {
            RunUntilSee();
        }
        else
        {
            targetPosition = new Vector3(0, 0, 0);
        }

        //targetRotation = Mathf.Atan2(targetPosition.x - transform.position.x, targetPosition.z - transform.position.z) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, targetRotation, 0);
        //Vector3 targetDirection = transform.rotation * Vector3.forward;
        if (agent.enabled)
        {
            agent.destination = targetPosition;
        }

        //controller.Move(targetDirection.normalized * Time.deltaTime * 5);

    }
    public void Chase()
    {
        if (player)
        {
            //Debug.Log("chasing");
            timeSinceChase = 0;
            targetPosition = player.transform.position;
            timeChasing += Time.deltaTime;
            //Debug.Log(timeInRange);
            if (Math.Abs(player.transform.position.x - gameObject.transform.position.x) + Math.Abs(player.transform.position.y - gameObject.transform.position.y) + Math.Abs(player.transform.position.z - gameObject.transform.position.z) < 10)
            {
                timeInRange += Time.deltaTime;
            }
        //if (timeInRange > 7 && player.GetComponent<ThirdPersonController>().InStealth())
        //{
        //    state = 0;
        //    timeInRange = 0;
        //}
        //Debug.Log("chasing");
            if (timeChasing > 9 && timeSinceLosingVision > 2 && !canSee)
            {
                state = 0;
                timeChasing = 0;
                timeInRange = 0;
            } //maybe change time to be greater later
        }
        

        
        
        //Debug.Log("yes");
    }
    public void ChangeState(int i)
    {
        state = i;
    }
    public void ChangePatrolPointStatus(bool a)
    {
        isAtPatrolPoint = a;
    }
    public GameObject GetPos()
    {
        return randPos;
    }
    public void Patrol()
    {
        
        
        
        if (isAtPatrolPoint)
        {
            randPos = (GameObject)a[UnityEngine.Random.Range(0, a.Count - 1)];

            isAtPatrolPoint = false;
        }
        targetPosition = randPos.transform.position;
        //Debug.Log("patrolling");
        if (timeSinceChase > 40)
        {
            state = 2;
            timeInRange = 0;
            
            isAtPatrolPoint = true;
            //now implement time since losing vision system

        }
        if (player)
        {
            if (Math.Abs(player.transform.position.x - gameObject.transform.position.x) + Math.Abs(player.transform.position.y - gameObject.transform.position.y) + Math.Abs(player.transform.position.z - gameObject.transform.position.z) < 5)
            {
                timeInRange += Time.deltaTime;
            }
        }
        
        if (canSee && timeSinceChase > 1)
        {
            state = 1;
            timeInRange = 0;
            isAtPatrolPoint = true;
        }
        if (timeInRange > 5)
        {
            state = 1;
            timeInRange = 0;
            isAtPatrolPoint = true;
        }
        //if (canSee)
        //{
        //    state = 1;
        // }
    }
    public void RunUntilSee()
    {
        timeSinceChase = 0;
        //Debug.Log("running");
        targetPosition = player.transform.position;
        if (canSee)
        {
            state = 1;
            
            timeInRange = 0;
        }
        if (Math.Abs(player.transform.position.x - gameObject.transform.position.x) + Math.Abs(player.transform.position.y - gameObject.transform.position.y) + Math.Abs(player.transform.position.z - gameObject.transform.position.z) < 10)
        {
            timeInRange += Time.deltaTime;
        }
        if (timeInRange > 2)
        {
            state = 0;
            timeInRange = 0;
        }
        
    }


    public void Idle()
    {

    }
    public void Jumpscare()
    {
        //agent.enabled = false;
        brain.enabled = false;
        agent.enabled = false;
        //Destroy(rigid);
        //player.transform.LookAt(gameObject.transform);
        Destroy(player);
        Camera.main.transform.position = gameObject.transform.position + new Vector3(0, 1f, -3.5f);
        Camera.main.transform.LookAt(gameObject.transform.position + Vector3.up * 2);

        gameObject.transform.LookAt(Camera.main.transform.position + Vector3.down * 3);
        //Destroy(player);
        state = 0;
        audioSource.Play();
        jumpscareLight.gameObject.SetActive(true);
        gameObject.GetComponent<Animator>().Play("bite");

    }
    


}
