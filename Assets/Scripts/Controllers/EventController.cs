using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EventController : MonoBehaviour
{

    GameObject enemigo;
    Enemigo enemigoScript;
    GameObject player;
    TestCamera playerScript;
    public static int currentEvent;

    [Tooltip("Animator del enemigo")]
    public Animator enemigoAnim;

    [Tooltip("Porcentaje de velocidad normal del enemigo en el evento 1")]
    [Range(0.0f, 1.0f)]
    public float PorcentajeDeVelEnemigoEvt1;

    public Transform enemySpawnPoint;

    public Image winScreen;

    MenuController menuController;

    public float tiempoParaGanar;

    AudioSource winSFX;

    MusicController musicController;

    public static bool playedMoreThanOneTime;

    void Start()
    {
        enemigo = GameObject.FindGameObjectWithTag("Enemigo");
        enemigoScript = enemigo.GetComponent<Enemigo>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<TestCamera>();
        menuController = FindObjectOfType<MenuController>();
        winSFX = GetComponent<AudioSource>();
        musicController = FindObjectOfType<MusicController>();
    }

    void Update()
    {
        if (currentEvent == 1)
        {
            if (enemigoScript.targetReached)
            {
                enemigoScript.SetTarget(new Vector3(-20.8f, 173.4f, 14.4f));
                
                //menuController.SetInteractionText("[F] Skip");
                currentEvent++;
            }

            if (playedMoreThanOneTime)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(TestCamera.ESCAPE) || Input.GetKeyDown(TestCamera.INTERACTION_KEY) || Input.GetKeyDown(TestCamera.ALTERNATIVE_INTERACTION_KEY))
                {
                    SkipIntroduction();
                }
            }
        }

        else if (currentEvent == 2)
        {
            if (enemigoScript.targetReached)
            {
                SkipIntroduction();
            }

            if (playedMoreThanOneTime)
            {
                if (Input.GetKeyDown(TestCamera.ESCAPE) || Input.GetKeyDown(TestCamera.INTERACTION_KEY) || Input.GetKeyDown(TestCamera.ALTERNATIVE_INTERACTION_KEY))
                {
                    SkipIntroduction();
                }
            }
        }

        else if (currentEvent == 4)
        {
            menuController.SetInteractionText("[SHIFT] RUN");
            currentEvent++;
        }

        else if (currentEvent == 5)
        {
            if (Input.GetKeyDown(TestCamera.RUN_KEY))
            {
                menuController.SetInteractionText("");
                currentEvent++;
            }
        }
    }

    public void SkipIntroduction()
    {
        enemigo.GetComponent<NavMeshAgent>().enabled = false;
        enemigo.transform.position = enemySpawnPoint.position;
        enemigo.GetComponent<NavMeshAgent>().enabled = true;
        Debug.Log("enemy.position = " + enemigo.transform.position);
        Debug.Log("enemy spawn position = " + enemySpawnPoint.position);
        enemigoScript.Whistle();
        //Reset enemy state to normal
        enemigoScript.EnableChasing();
        playerScript.Enable();
        enemigoScript.DisableChasing();
        playedMoreThanOneTime = true;
        currentEvent = 3;
    }

    public void PlayEnemyIntroduction()
    {
        PrepararIntroduccionEnemigo();
        enemigoScript.Whistle();
        enemigoScript.SetTarget(new Vector3(-34.75f, 173.4f, 19.3f));
        currentEvent++;
    }

    public void PlayWinEvent()
    {
        enemigo.SetActive(false);
        playerScript.Disable();
        StartCoroutine(Win());
    }


    IEnumerator Win()
    {
        tiempoParaGanar = winSFX.clip.length - 5.5f;
        musicController.StopMusic();
        winSFX.Play();
        Color tempColor = winScreen.color;
        float rateOfFade = (1 / tiempoParaGanar) * 0.01f;
        while (winScreen.color.a < 1)
        {
            tempColor.a += rateOfFade;
            winScreen.color = tempColor;
            yield return new WaitForSeconds(0.01f);
        }
        //yield return new WaitForSeconds(1f);
        Application.Quit();
    }
    /// <summary>
    /// Pone los valores adecuados de velocidad y tiempo de sonido del paso del enemigo para el evento de introducción
    /// </summary>
    void PrepararIntroduccionEnemigo()
    {
        enemigoScript.CambiarVelocidadEnPorcentaje(PorcentajeDeVelEnemigoEvt1);
    }

}
