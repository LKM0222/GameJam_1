using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeMove : MonoBehaviour
{

    public int speed;
    public bool isJumpping = false;

    private Rigidbody rb;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveCube();
    }

    void moveCube()
    {
        if (!isJumpping)
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.isJumpping = true;
                transform.position = new Vector3(Random.Range(-8f, -11f), 1f, 0);
                transform.rotation = Quaternion.identity;
                // rb.AddForce(transform.up * 10);
                rb.AddTorque(Random.Range(150, 200), Random.Range(150, 200), Random.Range(150, 200));
                this.GetComponent<Rigidbody>().velocity = Vector3.up * this.speed;
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground")
        {
            this.isJumpping = false;
        }
    }

}