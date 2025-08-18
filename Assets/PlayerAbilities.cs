using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public InputAction ignite;
    public GameObject fire;
    void Start()
    {
        ignite = InputSystem.actions.FindAction("Light");
        fire.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    

}
