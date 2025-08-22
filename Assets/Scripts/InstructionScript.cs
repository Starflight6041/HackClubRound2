using TMPro;
using UnityEngine;

public class InstructionScript : MonoBehaviour
{
    public TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.gameObject.SetActive(false);
    }
    public void ShowInstructions()
    {
        text.gameObject.SetActive(!text.gameObject.activeSelf);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
