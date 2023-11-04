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

    public static cubeMove instance;

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
                dice1_isJumpping = true;
                dice2_isJumpping = true;

                dice1.transform.position = new Vector3(Random.Range(-1, 1), 1f, 0);
                dice2.transform.position = new Vector3(Random.Range(-1, 1), 1f, 0);

                dice1.transform.rotation = Quaternion.identity;
                dice2.transform.rotation = Quaternion.identity;

                dice1_Rb.AddTorque(Random.Range(50, 100), Random.Range(50, 100), Random.Range(50, 100));
                dice2_Rb.AddTorque(Random.Range(50, 100), Random.Range(50, 100), Random.Range(50, 100));


                dice1.GetComponent<Rigidbody>().velocity = Vector3.up * speed;
                dice2.GetComponent<Rigidbody>().velocity = Vector3.up * speed;
            }
        }
    }
}