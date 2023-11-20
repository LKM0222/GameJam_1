using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject pos1,pos2;
    [SerializeField] float speed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {  
        this.transform.position = Vector3.MoveTowards(this.transform.position,pos2.transform.position,Time.deltaTime * speed);
    }
}
