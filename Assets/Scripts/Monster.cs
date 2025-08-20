using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public GameObject player;
    private float targetRotation;
    private Vector3 targetPosition;
    //public CharacterController controller;
    public int state = 1;
    public NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("test");
        Debug.Log("test");
        Debug.Log("Test");
        Debug.Log("test");
        Debug.Log("test");
        Debug.Log("test");
    }

    // Update is called once per frame
    void Update()
    {

        if (state == 1)
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
        agent.destination = targetPosition;
        //controller.Move(targetDirection.normalized * Time.deltaTime * 5);

    }
    public void Chase()
    {
        targetPosition = player.transform.position;
    }
}
