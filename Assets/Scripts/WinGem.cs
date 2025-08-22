using UnityEngine;
using StarterAssets;
public class WinGem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ThirdPersonController>())
        {
            other.gameObject.GetComponent<ThirdPersonController>().IncrementWin();
            Destroy(gameObject);
        }
    }
}
