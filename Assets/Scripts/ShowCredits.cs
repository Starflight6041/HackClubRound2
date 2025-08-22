using TMPro;
using UnityEngine;

public class ShowCredits : MonoBehaviour
{
    public TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void DisplayCredits()
    {
        text.gameObject.SetActive(!text.gameObject.activeSelf);
    }
}
