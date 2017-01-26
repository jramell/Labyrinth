using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroductionController : MonoBehaviour {

    public Text textoIntroductorio;

    public float tiempoParaAparecer;

    public float tiempoParaQuedarse;

    public float tiempoParaDesaparecer;

    Color textColor;

	void Start()
    {
        textColor = textoIntroductorio.color;
        //textColor.a = 0;
        //textoIntroductorio.color = textColor;
        //textColor = temp;
        StartCoroutine(Introduction());
    }

    IEnumerator Introduction()
    {
        float rateOfFade = 1 / tiempoParaAparecer * 0.01f;
        while (textoIntroductorio.color.a < 1)
        {
            textColor.a += rateOfFade;
            textoIntroductorio.color = textColor;
            yield return new WaitForSeconds(rateOfFade);
        }
        yield return new WaitForSeconds(tiempoParaQuedarse);

       rateOfFade = 1 / tiempoParaDesaparecer * 0.01f;
        while (textoIntroductorio.color.a > 0)
        {
            textColor.a -= rateOfFade;
            textoIntroductorio.color = textColor;
            yield return new WaitForSeconds(rateOfFade);
        }
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneController.ESCENA_PRIMER_NIVEL);
    }
}
