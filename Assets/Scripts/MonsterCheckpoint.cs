using UnityEngine;
using System;
public class MonsterCheckpoint : MonoBehaviour
{
    public GameObject monster;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Math.Abs(monster.transform.position.x - gameObject.transform.position.x) + Math.Abs(monster.transform.position.y - gameObject.transform.position.y) + Math.Abs(monster.transform.position.z - gameObject.transform.position.z) < 4 && monster.GetComponent<Monster>().GetPos() == gameObject)
        {
            monster.GetComponent<Monster>().ChangePatrolPointStatus(true);
        }
    }
}
