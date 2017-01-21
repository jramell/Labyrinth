using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class SkeletonController : MonoBehaviour {

    public KeyCode keyWalk;
    public KeyCode keyRotateLeft;
    public KeyCode keyRotateRight;
    public KeyCode keyAttack;
     
    public float movementSpeed;
    public Vector3 movementDirection;

    public float rotationSpeed;

    public int healthPoints;
    
    private Animator animator;
    public bool isDead = false;

    public float attackRange;
    public int enemiesKilled;
    public int enemiesToKill;


	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if(!isDead)
        CheckInput();
	}

    private void CheckInput()
    {
        if (Input.GetKey(keyWalk)) { 
             Walk();
            animator.SetBool("walking", true);

        } else
        {
            animator.SetBool("walking", false);
        }
        if (Input.GetKey(keyRotateRight))
            Rotate(1);
        if (Input.GetKey(keyRotateLeft))
            Rotate(-1);
        if (Input.GetKeyDown(keyAttack))
            Attack();
        
    }


    private void Walk (){
        transform.Translate(
            movementDirection *
            movementSpeed *
            Time.deltaTime);
                    }

    private void Rotate(int direction)
    {
        transform.Rotate(new Vector3(0,
            rotationSpeed*direction*Time.deltaTime,
            0));
    }
    private void Attack()
    {
       
        animator.SetTrigger("attack");
        BaseEnemy enemy = GetNearestEnemy();
        if (enemy != null)
        {
            Destroy(enemy.gameObject,1.2f);
            enemiesKilled = enemiesKilled + 1;
            if (enemiesKilled>=enemiesToKill)
            {
                WinLevel();
            }
          
        }
    }

    private void WinLevel()
    {
        Debug.Log("WIN !!!!!!!!!!!!");
        Invoke("LoadWinScene",3f);
    }
    private void LoadWinScene()
    {
        SceneManager.LoadScene("Ganaste");
    }

    public void GetHit() {
        
        if (!isDead) {
           // this.transform.position = new Vector3(this.transform.position.x, );
            healthPoints = healthPoints - 1;
             if (healthPoints <= 0) {
                GetKilled();
            }
            else  {
                Invoke("SetGetHitAnim", 0.55f);
            }
        }
       
    }

    private void SetGetHitAnim(){
               animator.SetTrigger("getHit");
            }

    private void GetKilled()
    {     
        isDead = true;
        Invoke("SetGetKilledAnim", 0.55f);
        LoseLevel();
    }

    private void LoseLevel()    {
        Debug.Log("YOU LOSE !!!!!!!!!!!!");
        Invoke("LoadLoseScene", 3f);
    }
    private void LoadLoseScene()    {
        SceneManager.LoadScene("LoseScene");
    }

    private void SetGetKilledAnim()
    {
        animator.SetTrigger("getKilled");
        
    }
    private BaseEnemy GetNearestEnemy()
    {
        BaseEnemy[] enemies = FindObjectsOfType<BaseEnemy>();
    foreach(BaseEnemy enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= attackRange)
                return enemy;
        }

        return null;
    }
}
