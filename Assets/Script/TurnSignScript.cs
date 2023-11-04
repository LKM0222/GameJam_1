using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSignScript : MonoBehaviour
{
    [SerializeField] GameObject cursor;
    public int cursorPos;
    [SerializeField] float speed = 147f;
    [SerializeField] Vector3[] pos = new Vector3[3];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cursorPos == 1){//card
            cursor.transform.localPosition = new Vector3(-147f,cursor.transform.position.y,0f);
            
        }
        if(cursorPos == 2){//egg
            cursor.transform.localPosition = new Vector3(0f,cursor.transform.position.y,0f);
        }
        if(cursorPos == 3){//move
            cursor.transform.localPosition = new Vector3(147f,cursor.transform.position.y,0f);
        }
    }
}
