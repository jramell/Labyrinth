using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour {

    GameObject enemigo;
    Enemigo enemigoScript;
    GameObject player;
    TestCamera playerScript;

    int currentEvent;

    public Transform enemySpawnPoint;

    void Start()
    {
        enemigo = GameObject.FindGameObjectWithTag("Enemigo");
        enemigoScript = enemigo.GetComponent<Enemigo>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<TestCamera>();
    }

    void Update()
    {
        if (currentEvent == 1)
        {
            if (enemigoScript.targetReached)
            {
                enemigoScript.SetTarget(new Vector3(-20.8f, 173.4f, 14.4f));
                currentEvent++;
            }
        }

        else if (currentEvent == 2)
        {
            if (enemigoScript.targetReached)
            {
                enemigo.transform.position = enemySpawnPoint.position;
                enemigoScript.Whistle();
                enemigoScript.EnableChasing();
                playerScript.Enable();
            }
        }
    }

	public void PlayEnemyIntroduction()
    {
        enemigoScript.Whistle();
        enemigoScript.SetTarget( new Vector3(-34.75f, 173.4f, 19.3f) );
        currentEvent++;
    }

}
