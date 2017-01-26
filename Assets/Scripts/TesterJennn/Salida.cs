using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Salida : MonoBehaviour
{

    EventController eventController;
    //------LLAMANDO EVENTOS DE FMOD ----------------------------------------// 

    //Musica
    [FMODUnity.EventRef]
    public string EventoAngel = "event:/Angel";
    public static FMOD.Studio.EventInstance AudioEventoAngel;

    //-------------------------------------------------------------------------//

    void Start()
    {
        AudioEventoAngel = FMODUnity.RuntimeManager.CreateInstance(EventoAngel);
        eventController = FindObjectOfType<EventController>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            eventController.PlayWinEvent();
        }
    }

    public void JugadorLlamo()
    {
        PlayCelestialSound();
    }

    void PlayCelestialSound()
    {
        AudioEventoAngel.start();
    }
}
