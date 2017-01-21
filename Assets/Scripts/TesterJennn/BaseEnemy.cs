using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour {

    public GameObject target;
    public float speed;
    public float rangoAtaque;
    public float attackCoolDown;
    public bool canAttack=true;
    public Animator animator;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update() {
        //calculo para hayar que el enemigo sga al objetivo
        Vector3 direction = target.transform.position - this.transform.position;
        //cambio a radianes de la mira del enemigo
        float radianes = Mathf.Atan2(direction.x, direction.z);
        //cambio a angulos de la mira del enemigo
        float angulos = radianes * Mathf.Rad2Deg;
        //asignacion de los angulos de seguimiento entre el enemigo y el objetivo
        this.transform.eulerAngles = (new Vector3(0, angulos, 0));

        //calculo del rango de ataque 
        if (direction.magnitude <= rangoAtaque)
        {
            animator.SetBool("walking", false);
            if(canAttack)
                 Attack();
        }
        else
        {
            animator.SetBool("walking", true);
            //calculo de un paso en direccion al objetivo
            Vector3 step = direction.normalized
                * speed
                * Time.deltaTime;
            //calculo para la nueva posicion del enemigo a razon de la posicion del objetivo
            Vector3 newPosition = this.transform.position + step;
            //asignacion de la nueva posicion
            this.transform.position = newPosition;
        }
      
	}
    private void Attack()
    {
        animator.SetTrigger("attack");
        canAttack = false;
        Invoke("ResetAttackCoolDown", attackCoolDown);
        target.GetComponent<SkeletonController>().GetHit();

    }

    private void ResetAttackCoolDown()
    {//tiempo de espera para el siguente ataque
        canAttack = true;
    }



}
