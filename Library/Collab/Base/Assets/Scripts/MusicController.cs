using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

    //------LLAMANDO EVENTOS DE FMOD ----------------------------------------// 

    //Musica
    [FMODUnity.EventRef]
    public string EventoMusic = "event:/Music";
    public static FMOD.Studio.EventInstance AudioEventoMusic;
    public static FMOD.Studio.ParameterInstance ParamMusic;

    //-------------------------------------------------------------------------//

    public float timeToWaitBeforeChange;

    public float fadingTime;

    bool playingSuspenseMusic;

    bool shouldKeepWaiting;

    void Awake() {

        // -----------------FMOD & UNITY----------------------------------//
        AudioEventoMusic = FMODUnity.RuntimeManager.CreateInstance(EventoMusic);
        AudioEventoMusic.getParameter("Musica", out ParamMusic);
        AudioEventoMusic.start();
        //----------------------------------------------//
    }

    //public bool isSuspensePlaying
    //{
    //    get
    //    {
    //        return playingSuspenseMusic;
    //    }

    //    set
    //    {
    //        playingSuspenseMusic = value;
    //    }
    //}

        IEnumerator TestTransition()
    {
        PlaySuspenseMusic();
        yield return new WaitForSeconds(10f);
        PlayChasingMusic();
    }

	public void PlaySuspenseMusic()
    {
        //if (isSuspensePlaying)
        //{
        //    shouldKeepWaiting = false;
        //}

        //else
        //{
        //    StartCoroutine(WaitToFade());
        //}
        ParamMusic.setValue(2.0f);
        Debug.Log("paramMusic value = 2.0f");
    }

    public void PlayChasingMusic()
    {
        ParamMusic.setValue(1.0f);
        Debug.Log("paramMusic value = 1.0f");
        //ParamMusic.setValue(2.0f);
        ////If chasing music is already playing and you want it to play again, stop the fading process (if it exists)
        //if (!isSuspensePlaying)
        //{
        //    shouldKeepWaiting = false;
        //}

        //else
        //{
        //    StartCoroutine(WaitToFade());
        //}
    }

    //IEnumerator WaitToFade()
    //{
    //    float counter = 0f; 
    //    while (counter < timeToWaitBeforeChange && shouldKeepWaiting)
    //    {
    //        yield return new WaitForSeconds(0.01f);
    //        counter += 0.01f;
    //    }

    //    //If waiting wasn't interrumpted (if there wasn't a request to play again the same song
    //    if (shouldKeepWaiting)
    //    {
    //        Fade();
    //    }
    //}

    //void Fade()
    //{

    //    //Fade between sounds
    //    if(isSuspensePlaying)
    //    {
    //        //Fade from suspense to other song
    //    }

    //    else
    //    {
    //        ParamMusic.setValue(2.0f);
    //        //Fade from other song to suspense
    //    }
    //}
}
