using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeMove : MonoBehaviour
{
    public GameObject dice1;
    public GameObject dice2;

    public float speed;

    public bool dice1_isJumpping;
    public bool dice2_isJumpping;

    public Rigidbody dice1_Rb;
    public Rigidbody dice2_Rb;

    public int rand_num1;
    public int rand_num2;

    public static cubeMove instance;

    public Animator dice1_animator;
    public Animator dice2_animator;

    public GameObject plane;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MoveCube();
    }

    void MoveCube()
    {
        if (!dice1_isJumpping && !dice2_isJumpping)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dice1_animator.SetBool("Throw", false);
                dice1_animator.SetInteger("Number", 0);
                dice2_animator.SetBool("Throw", false);
                dice2_animator.SetInteger("Number", 0);

                rand_num1 = Random.Range(1, 7);
                rand_num2 = Random.Range(1, 7);

                StartCoroutine(ThrowDice());

                dice1_isJumpping = true;
                dice2_isJumpping = true;

                dice1.transform.position = new Vector3(-1, 1f, plane.transform.position.z);
                dice2.transform.position = new Vector3(1, 1f, plane.transform.position.z);

                dice1.transform.rotation = Quaternion.identity;
                dice2.transform.rotation = Quaternion.identity;

                dice1_Rb.AddTorque(101, 102, 103);
                dice2_Rb.AddTorque(101, 102, 103);


                dice1.GetComponent<Rigidbody>().velocity = Vector3.up * speed;
                dice2.GetComponent<Rigidbody>().velocity = Vector3.up * speed;
            }
        }
    }

    IEnumerator ThrowDice()
    {
        yield return new WaitForSeconds(0.01f);
        dice1_animator.SetBool("Throw", true);
        dice1_animator.SetInteger("Number", rand_num1);
        dice2_animator.SetBool("Throw", true);
        dice2_animator.SetInteger("Number", rand_num2);
        yield return new WaitForSeconds(2f);
    }


}