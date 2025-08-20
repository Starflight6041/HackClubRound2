using StarterAssets;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public GameObject player;
    private float targetRotation;
    public float timeSinceChasingPlayer;

    public CinemachineBrain brain;
    private Vector3 targetPosition;
    //public CharacterController controller;
    public int state = 1;
    public NavMeshAgent agent;
    //public Rigidbody rigid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //rigid.isKinematic = true;
        Jumpscare();

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit1;
        RaycastHit hit2;
        RaycastHit hit3;
        RaycastHit hit4;
        Physics.Raycast(transform.position, Vector3.forward, out hit1, 1);
        Physics.Raycast(transform.position, Vector3.back, out hit2, 1);
        Physics.Raycast(transform.position, Vector3.left, out hit3, 1);
        Physics.Raycast(transform.position, Vector3.right, out hit4, 1);
        
        if (state == 0)
        {

        }
        else if (state == 1)
        {
            //Debug.Log("test");
            //Debug.Log("test");
            //Debug.Log("test");

            //Debug.Log("test");
            Chase();
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
        targetPosition = player.transform.position;
        Debug.Log("yes");
    }
    public void Idle()
    {

    }
    public void Jumpscare()
    {
        //agent.enabled = false;
        brain.enabled = false;

        //Destroy(rigid);
        //player.transform.LookAt(gameObject.transform);
        Camera.main.transform.position = gameObject.transform.position + new Vector3(.5f, 3f, -3);
        Camera.main.transform.LookAt(gameObject.transform.position + Vector3.up * 2);

        gameObject.transform.LookAt(player.transform.position);
        //Destroy(player);
        state = 0;
        gameObject.GetComponent<Animator>().Play("bite");

    }
    


}
