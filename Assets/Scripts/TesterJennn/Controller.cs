using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    public GameObject target;
    public float speed;
    public float rangoAtaqueCorre;
    public float rangoAtaqueWalk;
    public bool rangoAtaqueAyuda = false;
    public float attackCoolDown;
    public bool canAttack = true;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Verificar();

    }

    public void Verificar()
    {
        //calculo para hayar que el enemigo sga al objetivo
        Vector3 direction = target.transform.position - this.transform.position;
        //cambio a radianes de la mira del enemigo
        float radianes = Mathf.Atan2(direction.x, direction.z);
        //cambio a angulos de la mira del enemigo
        float angulos = radianes * Mathf.Rad2Deg;
        //asignacion de los angulos de seguimiento entre el enemigo y el objetivo
        this.transform.eulerAngles = (new Vector3(0, angulos, 0));

        //calculo del rango de ataque 
        if (direction.magnitude <= rangoAtaqueCorre)
        {
            Debug.Log("ESTA EN EL RANGO DE CORRER");
            //EL ENEMIGO ESTA  CERCA te asusta 
        }
        if (rangoAtaqueAyuda)
        {
            Debug.Log("PIDIO AYUDA");
            //enviar posicion al enemigo
        }


    }
}
/*else
        {
            //calculo de un paso en direccion al objetivo
            Vector3 step = direction.normalized
                * speed
                * Time.deltaTime;
            //calculo para la nueva posicion del enemigo a razon de la posicion del objetivo
            Vector3 newPosition = this.transform.position + step;
            //asignacion de la nueva posicion
            this.transform.position = newPosition;
        }*/
