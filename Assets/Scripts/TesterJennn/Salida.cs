using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Salida : MonoBehaviour
{

    //------LLAMANDO EVENTOS DE FMOD ----------------------------------------// 

    //Musica
    [FMODUnity.EventRef]
    public string EventoAngel = "event:/Angel";
    public static FMOD.Studio.EventInstance AudioEventoAngel;

    //-------------------------------------------------------------------------//

    void Start()
    {
        AudioEventoAngel = FMODUnity.RuntimeManager.CreateInstance(EventoAngel);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<TestCamera>().WinLevel();
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
