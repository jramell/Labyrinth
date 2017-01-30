using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maneja la mecánica de spray. Asume que está añadido al jugador y que sólo hay una cámara en la escena
/// </summary>
public class SprayController : MonoBehaviour
{

    [Tooltip("Qué tan lejos de la pared se puede hacer spray")]
    public float rangoSpray;

    public GameObject spray;

    AudioSource spraySFX;
    Ray ray;
    Camera currentCamera;
    RaycastHit hit;
    MenuController menuController;
    EventController eventController;

    void Start()
    {
        currentCamera = FindObjectOfType<Camera>();
        spraySFX = GetComponent<AudioSource>();
        menuController = FindObjectOfType<MenuController>();
        eventController = FindObjectOfType<EventController>();
    }
    void Update()
    {
        if (Input.GetKeyDown(TestCamera.INTERACTION_KEY) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(TestCamera.ALTERNATIVE_INTERACTION_KEY))
        {
            ray = new Ray(currentCamera.transform.position, currentCamera.transform.forward * 2);

            Physics.Raycast(ray.origin, ray.direction, out hit);

            if (hit.collider != null)
            {
                if (hit.collider.tag == "Laberinto")
                {

                    if (hit.distance < rangoSpray)
                    {

                        Vector3 target = Vector3.MoveTowards(hit.point, transform.position, 0.1f);
                        Instantiate(spray, target, Quaternion.LookRotation(hit.normal * -1, Vector3.up));
                        spraySFX.Play();
                    }
                }
            }
        }
        //ray = new Ray(currentCamera.transform.position, currentCamera.transform.forward * 2);

        //Physics.Raycast(ray.origin, ray.direction, out hit);

        //if (hit.collider != null)
        //{
        //    if (hit.collider.tag == "Laberinto")
        //    {
        //        if (Input.GetKeyDown(TestCamera.INTERACTION_KEY))
        //        {
        //            if (hit.distance < rangoSpray)
        //            {

        //                Vector3 target = Vector3.MoveTowards(hit.point, transform.position, 0.1f);
        //                Instantiate(spray, target, Quaternion.LookRotation(hit.normal * -1, Vector3.up));
        //                spraySFX.Play();
        //            }
        //        }
        //    }

        //}
        //else if (hit.collider.tag == "Salida")
        //{
        //    menuController.SetInteractionText("[F] Save ")
        //    if (Input.GetKeyDown(TestCamera.INTERACTION_KEY))
        //        {
        //    eventController.PlayWinEvent();

        //    }
        //}


    }
}
