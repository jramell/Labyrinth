using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemigo : MonoBehaviour {

    public GameObject player;

    private Vector3 lastPlayerKnownLocation;

    /// <summary>
    /// In which state is the monster in? 
    /// 1 - knows the exact player location because he just heard a sound
    /// 2 - does not know the exact player location, so wanders around his approximate location. Passes to this state when he gets to the player's exact location and the 
    ///     player isn't there because he moved without making so much noise. Can go to 1 if he hears the player and gets his exact location.
    /// 3 - the player got too far, so he teleports somewhat close to him. After this, he starts wandering again.
    /// </summary>
    private int state;

    /// <summary>
    /// El monstruo escucha sonido del jugador, con lo que actualiza la última posición que conoce de él
    /// </summary>
    /// <param name="position"></param>
    public void EscucharSonido(Vector3 position)
    {
        lastPlayerKnownLocation = position;
        gameObject.GetComponent<NavMeshAgent>().SetDestination(lastPlayerKnownLocation);
    }

    void Start()
    {
        EscucharSonido(player.transform.position);
    }
}
