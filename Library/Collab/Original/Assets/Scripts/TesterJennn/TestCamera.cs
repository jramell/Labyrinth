using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestCamera : MonoBehaviour
{

    //------------LLAMANDO EVENTOS DE FMOD STUDIO-----------------------// 

    // PASOS JUGADOR
    [FMODUnity.EventRef]
    public string EventoSteps = "event:/Steps";
    public static FMOD.Studio.EventInstance AudioEventoSteps;

    // MUSICA 
    [FMODUnity.EventRef]
    public string EventoMusic = "event:/Music";
    public static FMOD.Studio.EventInstance AudioEventoMusic;
    public static FMOD.Studio.ParameterInstance ParamMusic;

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

    void Start()
    {

        //---------------SINCRONIZANDO EVENTOS DE FMOD STUDIO--------------//
        AudioEventoSteps = FMODUnity.RuntimeManager.CreateInstance(EventoSteps);

        AudioEventoMusic = FMODUnity.RuntimeManager.CreateInstance(EventoMusic);
        AudioEventoMusic.getParameter("Musica", out ParamMusic);
        AudioEventoMusic.start();
        //-----------------------------------------------------------------//

        isDead = false;
        cameraMovementEnabled = true;
        mainCamera = FindObjectOfType<Camera>();
        xRot = transform.rotation.eulerAngles.x;
        yRot = transform.rotation.eulerAngles.y;
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        enemigoScript = enemigo.GetComponent<Enemigo>();
        musicController = FindObjectOfType<MusicController>();
        staminaLocal = staminaMax;
    }

    void Update()
    {
        if (!isDead)
        {
            CheckInput();

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
        }
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
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Main");
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
        if (Input.GetKey(KeyCode.E))
        {
            Run();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PedirAyuda();
        }
    }

    void PlayWalkingSound()
    {
        //Suena el sonido de cuando el jugador da pasos
    }

    void PlayRunningSound()
    {
        //Suena el sonido de cuando el jugador corre (o se acelera el de cuando el jugador da pasos, luego se cuadra mejor)
    }

    private void Walk()
    {
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

        Vector3 orig = movementDirection;
        movementDirection = new Vector3(1, 0, 0);
        transform.Translate(movementDirection * movementSpeed * Time.deltaTime);
        movementDirection = orig;
    }

    private void WalkLeft()
    {
        // ---- esta linea va a reproducir el audio de los pasos----//
        PlayWalkingSound();
        Vector3 orig = movementDirection;
        movementDirection = new Vector3(-1, 0, 0);
        transform.Translate(movementDirection * movementSpeed * Time.deltaTime);
        movementDirection = orig;
    }

    private void Run()
    {
        if (staminaMax >= 0)
        {
            transform.Translate(movementDirection * movementSpeedRun * Time.deltaTime);
            HacerRuido(5);
            Debug.Log("PERSONAJE CORRIO");
            UsoStamina();
            PlayRunningSound();
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
        PlayWalkingSound();
        transform.Translate(
            movementDirection *
            movementSpeed *
            -1 *
            Time.deltaTime);

    }

    private void PedirAyuda()
    {
        Vector3 position = this.transform.position;
        enemigoScript.EscucharSonido(transform.position);
        PlayHelpSound();
    }

    void PlayHelpSound()
    {
        //Suena el sonido de cuando el jugador pide ayuda (
    }

    private void WinLevel()
    {
        Debug.Log("WIN !!!!!!!!!!!!");
        Invoke("LoadWinScene", 3f);
    }
    private void LoadWinScene()
    {
        //cargar  scena ganadora
        // SceneManager.LoadScene("aqui");
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

