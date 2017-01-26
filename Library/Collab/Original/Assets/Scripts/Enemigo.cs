using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemigo : MonoBehaviour
{

    //------LLAMANDO EVENTOS DE FMOD ----------------------------------------// 

    //Silbido 
    [FMODUnity.EventRef]
    public string EventoSilbido = "event:/Silbido";
    public static FMOD.Studio.EventInstance AudioEventoSilbido;
    public static FMOD.Studio.ParameterInstance ParamSilbido;

    //Pasos Silbon
    [FMODUnity.EventRef]
    public string EventoStepSilbon = "event:/Steps";
    public static FMOD.Studio.EventInstance AudioEventoStepSilbon;
    public static FMOD.Studio.ParameterInstance ParamStepSilbon;

    //Gruñido Silbon
    [FMODUnity.EventRef]
    public string EventoSilbon = "event:/Silbon";
    public static FMOD.Studio.EventInstance AudioEventoSilbon;

    //Gruñido Huesos
    [FMODUnity.EventRef]
    public string EventoHuesos = "event:/Huesos";
    public static FMOD.Studio.EventInstance AudioEventoHuesos;

    //-----------------------------------------------------------------------//

    private GameObject player;

    [Tooltip("Monster's speed when wandering around")]
    public float wanderingSpeed;

    [Tooltip("Monster's chasing speed when he knows the player's location")]
    public float chasingSpeed;

    [Tooltip("Shows monster's range")]
    public bool debug;

    /// <summary>
    /// How far the enemy can wander around the player when it doesn't know its location
    /// </summary>
    public float distanceToWanderAroundPlayer = 10000f;

    private NavMeshAgent navMeshAgent;

    private Vector3 currentTarget;

    /// <summary>
    /// In which state is the monster in? 
    /// 1 - knows the exact player location because he just heard a sound
    /// 2 - does not know the exact player location, so wanders around his approximate location. Passes to this state when he gets to the player's exact location and the 
    ///     player isn't there because he moved without making so much noise. Can go to 1 if he hears the player and gets his exact location.
    /// </summary>
    //private int state;

    private float distanceFromPlayer;

    /// <summary>
    /// Hears player automatically if player is less than this distance
    /// </summary>
    [Tooltip("When the player is closer than this, the enemy chases him faster")]
    public float distanceToHearPlayer;

    [Tooltip("Time the guy waits before whistling again")]
    public float timeBetweenWhistles = 10f;

    private AudioSource whistle;

    float whistleCounter;

    private bool chasingThePlayer;

    /// <summary>
    /// When The Whistler is further than this distance from the player, the sound will play at full volume. Closer than this it will begin to attenuate
    /// </summary>
    float maxDistanceFromPlayer = 50f;

    /// <summary>
    /// How much time there is between the enemy steps animation. Used to play the footstep and other sounds
    /// </summary>
    float stepTime = 0.75f;

    float stepCounter;

    Vector2 selfWithoutHeight;

    Vector2 targetWithoutHeight;

    MusicController musicController;

    /// <summary>
    /// Counts time to 'recast' the point just in case it gets stuck
    /// </summary>
    float recastCounter;

    float lastMenacingSound;

    float menacingSoundCooldown = 6f;

    bool chasingEnabled;

    bool reachedTarget;
    
    public bool targetReached
    {
        get
        {
            return reachedTarget;
        }
        //set
        //{
        //    reachedTarget = value;
        //}
    }


    void Start()
    {

        // -----------------FMOD & UNITY ----------------------------------//
        AudioEventoSilbido = FMODUnity.RuntimeManager.CreateInstance(EventoSilbido);
        AudioEventoSilbido.getParameter("distancia", out ParamSilbido);

        AudioEventoStepSilbon = FMODUnity.RuntimeManager.CreateInstance(EventoStepSilbon);
        AudioEventoStepSilbon.getParameter("distanciasilbon", out ParamStepSilbon);

        AudioEventoSilbon = FMODUnity.RuntimeManager.CreateInstance(EventoSilbon);

        AudioEventoHuesos = FMODUnity.RuntimeManager.CreateInstance(EventoHuesos);
        //-----------------------------------------------------------------//

        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = wanderingSpeed;
        player = GameObject.FindGameObjectWithTag("Player");
        //EscucharSonido(player.transform.position);
        whistle = GetComponent<AudioSource>();
        //GenerateRandomTarget();
        musicController = FindObjectOfType<MusicController>();
    }

    void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, distanceToHearPlayer);
        }
    }

    void Update()
    {
        targetWithoutHeight = new Vector2(player.transform.position.x, player.transform.position.z);
        distanceFromPlayer = Vector2.Distance(targetWithoutHeight, selfWithoutHeight);
        selfWithoutHeight = new Vector2(transform.position.x, transform.position.z);

        if (chasingEnabled)
        {
            if (distanceFromPlayer < distanceToHearPlayer)
            {
                EscucharSonido(player.transform.position);
            }

            //Only if player is not within attack range
            else
            {
                targetWithoutHeight = new Vector2(currentTarget.x, currentTarget.z);
                float distanceFromTarget = Vector2.Distance(targetWithoutHeight, selfWithoutHeight);
                if (distanceFromTarget < 0.2f)
                {
                    reachedTarget = true;
                    if (chasingThePlayer)
                    {
                        StopChasingThePlayer();
                    }
                    GenerateRandomTarget();
                }
            }

            whistleCounter += Time.deltaTime;
            if (whistleCounter >= timeBetweenWhistles)
            {
                whistleCounter = 0;
                Whistle(1 * (distanceFromPlayer / maxDistanceFromPlayer));
            }

            stepCounter += Time.deltaTime;
            if (stepCounter >= stepTime)
            {
                stepCounter = 0;
                PlayMovementSounds();
            }

            if (!chasingThePlayer)
            {
                recastCounter += Time.deltaTime;
                if (recastCounter > 8f)
                {
                    recastCounter = 0;
                    GenerateRandomTarget();
                }
            }
        }

        else
        {
            targetWithoutHeight = new Vector2(currentTarget.x, currentTarget.z);
            float distanceFromTarget = Vector2.Distance(targetWithoutHeight, selfWithoutHeight);
            if (distanceFromTarget < 0.2f)
            {
                reachedTarget = true;
                //Debug.Log("reached target");
            }
        }
    }

    void ResetValues()
    {
        navMeshAgent.speed = wanderingSpeed;
    }

    /// <summary>
    /// Plays sounds that should play each time The Whistler takes a step
    /// </summary>
    void PlayMovementSounds()
    {
        //--- Los pasos de silbon tienen que ir lugados al movimiento de la pierna! --// 

        //     AudioEventoStepSilbon.start();
        //--- ponerle un Random al saco de huesos, la ida es que no suene siempre -- // 
        //int random = Random.Range(0, 1);
        //if (random > 0.7f)
        //{
        //    AudioEventoHuesos.start();
        //}
    }

    public void Whistle(float distanceFactor)
    {
        ParamSilbido.setValue(distanceFactor);// MODIFICAR.... REPRODUCE EL AUDIO CUANDO SILBON ESTA CERCA
        AudioEventoSilbido.start();
    }

    public void Whistle()
    {
        targetWithoutHeight = new Vector2(player.transform.position.x, player.transform.position.z);
        selfWithoutHeight = new Vector2(transform.position.x, transform.position.z);
        distanceFromPlayer = Vector2.Distance(targetWithoutHeight, selfWithoutHeight);
        float distanceFactor = 1 * (distanceFromPlayer / maxDistanceFromPlayer);
        ParamSilbido.setValue(distanceFactor);
        AudioEventoSilbido.start();
    }

    void GenerateRandomTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * distanceToWanderAroundPlayer;
        //randomDirection += player.transform.position;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distanceToWanderAroundPlayer, -1);
        SetTarget(navHit.position);
    }

    public void SetTarget(Vector3 position)
    {
        reachedTarget = false;
        currentTarget = position;
        navMeshAgent.SetDestination(currentTarget);
    }

    public void EnableChasing()
    {
        chasingEnabled = true;
        GenerateRandomTarget();
    }

    /// <summary>

    /// <param name="position"></param>
    public void EscucharSonido(Vector3 position)
    {
        navMeshAgent.speed = chasingSpeed;
        SetTarget(position);
        if (Time.time > lastMenacingSound + menacingSoundCooldown)
        {
            PlayMenacingSound();
        }
        ChasingThePlayer();
    }

    void ChasingThePlayer()
    {
        chasingThePlayer = true;
        recastCounter = 0;
        musicController.PlayChasingMusic();
    }

    void StopChasingThePlayer()
    {
        chasingThePlayer = false;
        navMeshAgent.speed = wanderingSpeed;
        musicController.PlaySuspenseMusic();
    }

    void PlayMenacingSound()
    {
        lastMenacingSound = Time.time;
        AudioEventoSilbon.start();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            player.GetComponent<TestCamera>().Die();
        }
    }
}
