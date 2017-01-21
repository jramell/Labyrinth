using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestJenn : MonoBehaviour
{


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
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
            CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetKey(keyWalk))
        {
            Walk();
            animator.SetBool("walking", true);

        }
        else
        {
            animator.SetBool("walking", false);
        }
        if (Input.GetKey(keyRotateRight))
            Rotate(1);
        if (Input.GetKey(keyRotateLeft))
            Rotate(-1);

    }


    private void Walk()
    {
        transform.Translate(
            movementDirection *
            movementSpeed *
            Time.deltaTime);
    }

    private void Rotate(int direction)
    {
        transform.Rotate(new Vector3(0,
            rotationSpeed * direction * Time.deltaTime,
            0));
    }
       
  
   }

