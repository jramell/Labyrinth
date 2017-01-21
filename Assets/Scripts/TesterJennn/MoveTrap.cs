using UnityEngine;
using System.Collections;

public class MoveTrap : MonoBehaviour
{


    public GameObject target;
    public float rangoAtaque;
    public float attackCoolDown;
    public bool canAttack = true;
    public float speed;
    public int numeroAtaques;

    int contador = 0;
    int contaAtaques = 0;
    float tiempoPasa = 0;
    float tiempoMax = 1;
    public GUIText contandoText;
    public Animation animation;


    void Start()
    {
       
    }


    // Update is called once per frame
    void Update()
    {
        int aux = ContarTiempo();
        contandoText.text = aux.ToString();



        //calculo para hayar que el enemigo sga al objetivo
        Vector3 direction = target.transform.position - this.transform.position;
        //cambio a radianes de la mira del enemigo

        //calculo del rango de ataque 
        if (direction.magnitude <= rangoAtaque)
        {
            if (canAttack)
            {
                animation = GetComponent<Animation>();
                InvokeRepeating("IniciarAnimacion", 3, 3);
                contaAtaques++;
                Attack(contaAtaques);
                Debug.Log("CONTADOR EN " + contaAtaques + "trap " + gameObject.name);
            }



        }


    }

    private void Attack(int contaAtaques)
    {

        if (contaAtaques <= numeroAtaques)
        {
            this.transform.position = new Vector3(this.transform.position.x, 0, this.transform.position.z);
            canAttack = false;
            Invoke("ResetAttackCoolDown", attackCoolDown);
            target.GetComponent<SkeletonController>().GetHit();

        }
        else
        {
            Debug.Log("ME MORI" + gameObject.name);

            OnDestroy();
            //this.transform.position = new Vector3(this.transform.position.x, (float)-2.8, this.transform.position.z);
            //canAttack = false;

        }




    }
    private void IniciarAnimacion()
    {
        GetComponent<Animation>().Play();
        Invoke("noAttack", 0.05f);

    }
    private void noAttack()
    {
        canAttack = false;
    }


    private void OnDestroy()
    {
        Destroy(gameObject);
    }


    private int ContarTiempo()
    {
        tiempoPasa += Time.deltaTime;
        if (tiempoPasa >= tiempoMax)
        {
            tiempoPasa = 0;
            contador++;
        }
        return contador;
    }

    private void ResetAttackCoolDown()
    {//tiempo de espera para el siguente ataque
        this.transform.position = new Vector3(this.transform.position.x, (float)-1.8, this.transform.position.z);
        canAttack = true;
    }


}
