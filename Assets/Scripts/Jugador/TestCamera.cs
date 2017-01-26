using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestCamera : MonoBehaviour
{

    public const KeyCode RUN_KEY = KeyCode.LeftShift;
    public const KeyCode INTERACTION_KEY = KeyCode.F;
    //------------LLAMANDO EVENTOS DE FMOD STUDIO-----------------------// 

    // PASOS JUGADOR
    [FMODUnity.EventRef]
    public string EventoJugador = "event:/Jugador";
    public static FMOD.Studio.EventInstance AudioEventoJugador;
    public static FMOD.Studio.ParameterInstance ParamPasos;

    // HELP 
    [FMODUnity.EventRef]
    public string EventoHelp = "event:/Help";
    public static FMOD.Studio.EventInstance AudioEventoHelp;

    //-----------------------------------------------------------------//

    public GameObject enemigo;
    public float movementSpeed;
    public float movementSpeedRun;
    public Vector3 movementDirection;
    public float rotationSpeed;
    public float rangoAyuda;
    public float rangoCorrer;

    public float attackCoolDown;
    public float staminaMax;
    private float staminaLocal;

    private Enemigo enemigoScript;

    private bool isDead = false;
    private bool canRun = false;

    /// <summary>
    /// Mouse sensitivity
    /// </summary>
    public float mouseSensitivity;

    /// <summary>
    /// Is the player allowed to move the camera right now?
    /// </summary>
    private bool cameraMovementEnabled;

    private Camera mainCamera;

    private float xRot;

    private float yRot;

    private MusicController musicController;

    private const string MOUSE_Y = "Mouse Y";
    private const string MOUSE_X = "Mouse X";

    //nuevo
    public GameObject deathScreen;

    private bool shouldTakeStep;

    private float timeBetweenStepsWalking = 0.75f;

    private float timeBetweenStepsRunning = 0.4f;

    private float timeBetweenSteps;

    float stepCounter;

    private bool isRunning;

    private GameObject salida;

    private Salida salidaScript;
    public GameObject Salida;

    bool playerEnabled;

    public bool debug = true;


    //-----------------------LIGHT---------------//
    public int lightFactor = 0;

    // Light that should be controlled by the LightController
    public Light controlledLight01 = null;
    public Light l;

    void Start()
    {
        if (debug)
        {
            Enable();
        }
        //---------------SINCRONIZANDO EVENTOS DE FMOD STUDIO--------------//
        AudioEventoHelp = FMODUnity.RuntimeManager.CreateInstance(EventoHelp);

        AudioEventoJugador = FMODUnity.RuntimeManager.CreateInstance(EventoJugador);
        AudioEventoJugador.getParameter("correr", out ParamPasos);

        //-----------------------------------------------------------------//

        timeBetweenSteps = timeBetweenStepsWalking;
        isDead = false;
        cameraMovementEnabled = false;
        mainCamera = FindObjectOfType<Camera>();
        xRot = transform.rotation.eulerAngles.x;
        yRot = transform.rotation.eulerAngles.y;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        enemigoScript = enemigo.GetComponent<Enemigo>();
        musicController = FindObjectOfType<MusicController>();
        staminaLocal = staminaMax;
        //musicController.PlayChasingMusic();
        //            ParamPasos.setValue(0.0f);
        salidaScript = FindObjectOfType<Salida>();
        salida = salidaScript.gameObject;
    }

    void Update()
    {
        if (!isDead && playerEnabled)
        {
            if (!shouldTakeStep)
            {
                if (isRunning)
                {
                    timeBetweenSteps = timeBetweenStepsRunning;
                }
                else
                {
                    timeBetweenSteps = timeBetweenStepsWalking;
                }
                stepCounter += Time.deltaTime;
                if (stepCounter > timeBetweenSteps)
                {
                    shouldTakeStep = true;
                    stepCounter = 0;
                }
            }

            CheckInput();


        }

        if (cameraMovementEnabled)
        {
            //So the player is able to rotate relative to the y axis with the camera. This is setup this way so its local axis is synchronized
            //with what the player sees
            transform.localEulerAngles = new Vector3(0, yRot, 0);

            //So only the camera rotates relative to the x axis, not the player
            mainCamera.transform.localEulerAngles = new Vector3(xRot, 0, 0);

            //Because vertical camera rotation is relative to the in-game x axis
            xRot += Input.GetAxis(MOUSE_Y) * mouseSensitivity * -1;

            //Because horizontal camera rotation is relative to the in-game y axis
            yRot += Input.GetAxis(MOUSE_X) * mouseSensitivity;

            xRot = Mathf.Clamp(xRot, -55, 35);
        }
        if (controlledLight01 != null)
        { // If we have a light as a field
            l = controlledLight01.GetComponent<Light>();
        }
    }

    public void Enable()
    {
        xRot = transform.rotation.eulerAngles.x;
        yRot = transform.rotation.eulerAngles.y;
        playerEnabled = true;
        cameraMovementEnabled = true;
    }

    public void EnableCamera()
    {
        cameraMovementEnabled = true;
    }

    public void EnableWithRotation(float x, float y)
    {
        xRot = x;
        yRot = y;
        //playerEnabled = true;
    }

    public void Disable()
    {
        playerEnabled = false;
    }

    void PlayDeathSFX()
    {
        //Suena el sonido de cuando se muere el jugador
    }

    public void Die()
    {
        isDead = true;
        deathScreen.SetActive(true);
        StartCoroutine(Restart());
        PlayDeathSFX();
        musicController.StopMusic();
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(1.5f);
        EventController.currentEvent = 0;
        SceneManager.LoadScene(SceneController.ESCENA_PRIMER_NIVEL);
        //Application.Quit();
        Cursor.visible = true;
    }

    private void CheckInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Walk();
        }
        if (Input.GetKey(KeyCode.S))
        {
            Backward();
        }
        if (Input.GetKey(KeyCode.D))
        {
            WalkRight();
        }
        if (Input.GetKey(KeyCode.A))
        {
            WalkLeft();
        }
        if (Input.GetKey(RUN_KEY))
        {
            Run();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PedirAyuda();
        }
    }

    void PlayWalkingSound()
    {
        if (shouldTakeStep)
        {
            AudioEventoJugador.start();
            shouldTakeStep = false;
        }
    }

    void PlayRunningSound()
    {
        ParamPasos.setValue(1.0f);
        AudioEventoJugador.start();
    }

    private void Walk()
    {
        isRunning = false;
        PlayWalkingSound();

        transform.Translate(
            movementDirection *
            movementSpeed *
            Time.deltaTime);
    }

    private void WalkRight()
    {
        // ---- esta linea va a reproducir el audio de los pasos----//
        PlayWalkingSound();
        isRunning = false;
        Vector3 orig = movementDirection;
        movementDirection = new Vector3(1, 0, 0);
        transform.Translate(movementDirection * movementSpeed * Time.deltaTime);
        movementDirection = orig;
    }

    private void WalkLeft()
    {
        // ---- esta linea va a reproducir el audio de los pasos----//
        PlayWalkingSound();
        isRunning = false;
        Vector3 orig = movementDirection;
        movementDirection = new Vector3(-1, 0, 0);
        transform.Translate(movementDirection * movementSpeed * Time.deltaTime);
        movementDirection = orig;
    }

    private void Run()
    {
        if (staminaMax >= 0)
        {
            isRunning = true;
            transform.Translate(movementDirection * movementSpeedRun * Time.deltaTime);
            HacerRuido(5);
            //Debug.Log("PERSONAJE CORRIO");
            UsoStamina();
            //PlayWalkingSound();
        }
    }

    private Color buscarSalida()
    {
        float rangoCaliente = 5;
        float rangoMedio = 15;
        Vector3 direction = Salida.transform.position - this.transform.position;
        //cambio a radianes de la mira a la salida
        float radianes = Mathf.Atan2(direction.x, direction.z);
        //cambio a angulos de la mira de la salida
        float angulos = radianes * Mathf.Rad2Deg;
        //asignacion de los angulos de seguimiento entre la salida y el personaje
        this.transform.eulerAngles = (new Vector3(0, angulos, 0));

        //Debug.Log("MAGNITUDDDD" + direction.magnitude);

        if (direction.magnitude <= rangoCaliente)
        {
            return Color.red;
        }
        else if (direction.magnitude <= rangoMedio)
        {
            return Color.yellow;
            //color AMARILLO EN LA LUZ
        }
        else
        {
            return Color.blue;
            //COLOR AZUL EN LA LUZ 
        }
    }

    private void UsoStamina()
    {
        staminaMax = staminaMax - 1;
        //Debug.Log("STAMINA STATE:::" + staminaMax);
        canRun = false;
        if (staminaMax <= 0)
        {
            Invoke("LlenarStamina", attackCoolDown);
        }
    }
    private void LlenarStamina()
    {//tiempo de espera para la stamina
        canRun = true;
        staminaMax = staminaLocal;
    }

    private void Backward()
    {
        isRunning = false;
        PlayWalkingSound();
        transform.Translate(
            movementDirection *
            movementSpeed *
            -1 *
            Time.deltaTime);

    }

    private void PedirAyuda()
    {
        Color flag = buscarSalida();
        PlayHelpSound();
        //Vector3 position = this.transform.position;
        //enemigoScript.EscucharSonido(transform.position);
        StartCoroutine(SequenceOfHelpEvents(transform.position, flag));
        

    }

    /// <summary>
    /// Maneja todos los eventos que suceden después de que el jugador pide ayuda para que no se confundan
    /// </summary>
    /// <returns></returns>
    IEnumerator SequenceOfHelpEvents(Vector3 positionWhereHelpWasAsked, Color flag)
    {
        Color original = l.color;
        yield return new WaitForSeconds(0.6f);
        //Flag changes color at the same time response sounds
        l.color = flag;
        salidaScript.JugadorLlamo();
        yield return new WaitForSeconds(0.8f);
        l.color = original;
        enemigoScript.EscucharSonido(positionWhereHelpWasAsked);
    }

    void PlayHelpSound()
    {
        AudioEventoHelp.start();
    }

    public void WinLevel()
    {
        //   Debug.Log("WIN !!!!!!!!!!!!");
        //   Invoke("LoadWinScene", 3f);
        enemigo.SetActive(false);
        //eventC
    }

    void PlayVictoria()
    {
        //Suena el sonido de victoria
    }

    private void LoadWinScene()
    {
        SceneManager.LoadScene("MainConMenu");
    }

    private void GetKilled()
    {
        {
            isDead = true;
            // Invoke("SetGetKilledAnim", 0.55f);
            LoseLevel();
        }
    }

    private void LoseLevel()
    {
        Debug.Log("YOU LOSE !!!!!!!!!!!!");
        //  Invoke("LoadLoseScene", 3f);
    }
    private void LoadLoseScene()
    {
        //cargar scena perdedora o menu
    }


    private void HacerRuido(float rango)
    {
        Vector3 direction = enemigo.transform.position - this.transform.position;

        if (direction.magnitude <= rango)
        {
            Debug.Log("ENTRE AL RANGO");
            enemigoScript.EscucharSonido(transform.position);
        }



    }

    private void Rotate(int direction)
    {
        transform.Rotate(new Vector3(0,
            rotationSpeed * direction * Time.deltaTime,
            0));
    }
    private void RotateDown(int direction)
    {
        transform.Rotate(new Vector3(
            rotationSpeed * direction * Time.deltaTime,
            0, 0));
    }

}

