using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    //------------LLAMANDO EVENTOS DE FMOD STUDIO-----------------------// 

    // PASOS JUGADOR
    [FMODUnity.EventRef]
    private string EventoSteps = "event:/Steps";
    private static FMOD.Studio.EventInstance AudioEventoSteps;

    // MUSICA 
    [FMODUnity.EventRef]
    private string EventoMusic = "event:/Music";
    private static FMOD.Studio.EventInstance AudioEventoMusic;
    private static FMOD.Studio.ParameterInstance ParamMusic;

    //CORAZON


    [FMODUnity.EventRef]
    private string EventoHearth = "event:/Corazon";
    private static FMOD.Studio.EventInstance AudioEventoCorazon;

    //-----------------------------------------------------------------//

    public GameObject enemigo;
    public Light Luz;
    public GameObject Salida;

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
    public bool isDead = false;
    public bool canRun = false;

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

    private const string MOUSE_Y = "Mouse Y";
    private const string MOUSE_X = "Mouse X";
    public bool bandera = false;

    //-----------------------LIGHT---------------//
    public int lightFactor = 0;

    // Light that should be controlled by the LightController
    public Light controlledLight01 = null;
    public Light l;


    void Start()
    {

        //---------------SINCRONIZANDO EVENTOS DE FMOD STUDIO--------------//
        AudioEventoSteps = FMODUnity.RuntimeManager.CreateInstance(EventoSteps);
        AudioEventoCorazon = FMODUnity.RuntimeManager.CreateInstance(EventoHearth);



        AudioEventoMusic = FMODUnity.RuntimeManager.CreateInstance(EventoMusic);
        AudioEventoMusic.getParameter("Musica", out ParamMusic);
        AudioEventoMusic.start();
        //-----------------------------------------------------------------//

        cameraMovementEnabled = true;
        mainCamera = FindObjectOfType<Camera>();
        xRot = transform.rotation.eulerAngles.x;
        yRot = transform.rotation.eulerAngles.y;
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        enemigoScript = enemigo.GetComponent<Enemigo>();
        staminaLocal = staminaMax;
    }

    void Update()
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
        if (controlledLight01 != null)
        { // If we have a light as a field
            l = controlledLight01.GetComponent<Light>();
        }
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
            WalkRight();
        if (Input.GetKey(KeyCode.A))
            WalkLeft();
        if (Input.GetKey(KeyCode.E))
            Run();
        if (Input.GetKeyDown(KeyCode.Q))
            PedirAyuda();
    }

    private void Walk()
    {
        // ---- esta linea va a reproducir el audio de los pasos----//
        AudioEventoSteps.start();

        transform.Translate(
            movementDirection *
            movementSpeed *
            Time.deltaTime);
    }

    private void WalkRight()
    {
        // ---- esta linea va a reproducir el audio de los pasos----//

        AudioEventoSteps.start();

        Vector3 orig = movementDirection;
        movementDirection = new Vector3(1, 0, 0);
        transform.Translate(movementDirection * movementSpeed * Time.deltaTime);
        movementDirection = orig;
    }

    private void WalkLeft()
    {
        // ---- esta linea va a reproducir el audio de los pasos----//
        AudioEventoSteps.start();


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

            Invoke("LlenarStamina", attackCoolDown);
        }

        AudioEventoCorazon.getParameter("Corazon", out ParamMusic);
        AudioEventoCorazon.start();



    }
    public void pruebaBusquedaSalida()
    {
        float rangoCaliente = 5;
        float rangoMedio = 15;
        float rangoFrio = 20;

        Vector3 direction = Salida.transform.position - this.transform.position;
        //cambio a radianes de la mira a la salida
        float radianes = Mathf.Atan2(direction.x, direction.z);
        //cambio a angulos de la mira de la salida
        float angulos = radianes * Mathf.Rad2Deg;
        //asignacion de los angulos de seguimiento entre la salida y el personaje
        this.transform.eulerAngles = (new Vector3(0, angulos, 0));


        if (direction.magnitude <= rangoCaliente)
        {
            Debug.Log("ROJO, RANGO CALIENTE");
        }
        if (direction.magnitude <= rangoMedio)
        {
            Debug.Log("AMARILLO, RANGO MEDIO ");
            //color AMARILLO EN LA LUZ
        }
        if (direction.magnitude >= rangoFrio)
        {
            Debug.Log("AZUL, RANGO LEJANO");
            //COLOR AZUL EN LA LUZ 

        }
        Debug.Log("ESTA DEMASIADO LEJOS ");
    }

    private Color buscarSalida()
    {
        float rangoCaliente = 5;
        float rangoMedio = 15;
        float rangoFrio = 20;

        Vector3 direction = Salida.transform.position - this.transform.position;
        //cambio a radianes de la mira a la salida
        float radianes = Mathf.Atan2(direction.x, direction.z);
        //cambio a angulos de la mira de la salida
        float angulos = radianes * Mathf.Rad2Deg;
        //asignacion de los angulos de seguimiento entre la salida y el personaje
        this.transform.eulerAngles = (new Vector3(0, angulos, 0));

        Debug.Log("MAGNITUDDDD" + direction.magnitude);

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
        Debug.Log("STAMINA STATE:::" + staminaMax);
        canRun = false;
    }
    private void LlenarStamina()
    {//tiempo de espera para la stamina
        canRun = true;
        staminaMax = staminaLocal;
    }

    private void Backward()
    {
        transform.Translate(
            movementDirection *
            movementSpeed *
            -1 *
            Time.deltaTime);

    }

    private void PedirAyuda()
    {
        //SONAR SALIDA
        Debug.Log("PIDIO AYUDA!!!");
        //  enemigoScript.EscucharSonido(transform.position);


    }

    private void PlaySound()
    {
        //darle play al sonido, resibirlo por parametro 
    }


    public void EncenderLuz(GameObject Luz, bool bandera)
    {
        if (bandera)
        {
            GetComponent<Light>().enabled = true;

        }

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
            rotationSpeed * direction * Time.deltaTime, 0, 0));
    }
}

