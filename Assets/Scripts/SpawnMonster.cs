using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class SpawnMonster : MonoBehaviour
{
    public NavMeshAgent monsterAgent;
    public Transform pos; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ThirdPersonController>())
        {
            monsterAgent.Warp(pos.position);
            Debug.Log("warped!");

            
            
        }  
    }
}
