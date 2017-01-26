using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

    public GameObject mainMenu;

    MusicController musicController;

    GameObject player;

    bool shouldRotate;

    bool hasBeenZero;

    void Start()
    {
        musicController = FindObjectOfType<MusicController>();
        musicController.PlaySuspenseMusic();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        //shouldRotate = true;
        if (shouldRotate)
        {
            bool rotationXMet = Mathf.Approximately(player.transform.eulerAngles.x, 0);
            //bool rotationYMet = Mathf.Round(player.transform.eulerAngles.y) != 90;

            if (rotationXMet)
            {
                player.transform.Rotate(-Vector3.right * Time.deltaTime * 8f);
                //Debug.Log("x:" + player.transform.eulerAngles.x);
            }

            else if (player.transform.eulerAngles.y > 92 || player.transform.eulerAngles.y < 90)
            {
                player.transform.Rotate(Vector3.up * Time.deltaTime * 80f);
                //Debug.Log(player.transform.eulerAngles.y);
            }

            else
            {
                shouldRotate = false;
                player.transform.GetComponent<TestCamera>().Enable();
            }
        }
    }


    public void OnStartClick()
    {
        mainMenu.SetActive(false);
        Cursor.visible = false;
        shouldRotate = true;
    }

    public void OnCreditsClick()
    {

    }

    public void OnExitClick()
    {
        Application.Quit();
    }
}
