using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoving : MonoBehaviour
{
    float speed = 3.0f;
    // Start is called before the first frame update
    void Start(){

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow)){
            this.gameObject.transform.position += Vector3.up *Time.deltaTime * speed;
        }
        if(Input.GetKey(KeyCode.DownArrow)){
            this.gameObject.transform.position += Vector3.down *Time.deltaTime * speed;
        }
        if(Input.GetKey(KeyCode.LeftArrow)){
            this.gameObject.transform.position += Vector3.left *Time.deltaTime * speed;
        }
        if(Input.GetKey(KeyCode.RightArrow)){
            this.gameObject.transform.position += Vector3.right *Time.deltaTime * speed;
        }
        
        
    }
}
