using Unity.VisualScripting;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public GameObject player;
    private float targetRotation;
    private Vector3 targetPosition;
    public CharacterController controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition = player.transform.position; // when adding non chase states, change the target position
        targetRotation = Mathf.Atan2(targetPosition.x - transform.position.x, targetPosition.z - transform.position.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, targetRotation, 0);
        Vector3 targetDirection = transform.rotation * Vector3.forward;
        
        
    }
}
