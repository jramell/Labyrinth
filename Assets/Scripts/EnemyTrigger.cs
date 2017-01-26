using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{

    Enemigo enemigo;

    bool done;

    void Start()
    {
        enemigo = FindObjectOfType<Enemigo>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (!done)
        {
            if (col.gameObject.tag == "Player")
            {
                enemigo.EnableChasing();
                done = true;
            }
        }
    }
}
