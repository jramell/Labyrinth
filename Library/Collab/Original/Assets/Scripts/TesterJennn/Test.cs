using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    public GameObject enemigo;
    public KeyCode keyHelp;
    public KeyCode keyRotateLeft;
    public KeyCode keyRotateRight;
    public KeyCode keyRotateDown;
    public KeyCode keyRotateUp;
    public KeyCode keyRun;
    public KeyCode keyWalk;
    public KeyCode KeyBackward;
    public float movementSpeed;
    public float movementSpeedRun;
    public Vector3 movementDirection;
    public float rotationSpeed;
    public float rangoAyuda;
    public float rangoCorrer;

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
    private const string VERTICAL = "Vertical";

    void Start()
    {
        cameraMovementEnabled = true;
        mainCamera = FindObjectOfType<Camera>();
        xRot = transform.rotation.eulerAngles.x;
        yRot = transform.rotation.eulerAngles.y;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        enemigoScript = enemigo.GetComponent<Enemigo>();
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
    }

    private void CheckInput()
    {
        if (Input.GetKey(keyWalk))
        {
            Walk();
        }
        if (Input.GetKey(KeyBackward))
        {
            Backward();
        }

        if (Input.GetKey(keyRun))
            Run();
        if (Input.GetKeyDown(keyHelp))
            PedirAyuda();
        if (Input.GetKey(keyRotateDown))
            RotateDown(1);
        if (Input.GetKey(keyRotateUp))
            RotateDown(-1);
    }

    private void Walk()
    {
        transform.Translate(
            movementDirection *
            movementSpeed *
            Time.deltaTime);

        Step();
    }

    void Walk(Vector3 direction)
    {
        transform.Translate(
    movementDirection *
    movementSpeed *
    Time.deltaTime);
    }

    void Step()
    {

    }

    private void Run()
    {

        if (staminaMax > 0)
        {
            transform.Translate(movementDirection * movementSpeedRun * Time.deltaTime);
            HacerRuido(5);
            Debug.Log("PERSONAJE CORRIO");
            UsoStamina();

        }
        else
        {
            Debug.Log("STAMINA EN CERO");
            Walk();
        }

    }

    private void UsoStamina()
    {
        staminaMax = staminaMax - 1;
        Debug.Log("STAMINA STATE:::" + staminaMax);



    }
    private void LlenarStamina()
    {//tiempo de espera para la stamina
        canRun = true;
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
        Vector3 posicion = this.transform.position;
        //SONAR SALIDA
        Debug.Log("PIDIO AYUDA!!!");
        enemigoScript.EscucharSonido(transform.position);

    }

    private void PlaySound()
    {
        //darle play al sonido, resibirlo por parametro 
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

    private void SetGetKilledAnim()
    {
        //animacion para morir

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

