using StarterAssets;
using UnityEngine;

public class Gem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.GetComponent<ThirdPersonController>())
    //    {
    //        collision.gameObject.GetComponent<ThirdPersonController>().ChangeMana(50);
    //    }
    //}
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ThirdPersonController>())
        {
            other.gameObject.GetComponent<ThirdPersonController>().ChangeMana(50);
            Destroy(gameObject);
        }
    }
}
