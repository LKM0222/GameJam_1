using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor.Rendering;
using UnityEngine;

public class Move : MonoBehaviour
{
    // 오브젝트 기본 이동속도
    public float speed;
    // 오브젝트 달리기 이동속도
    public float run_speed;
    // 달리기 유무에 따른 이동속도
    float current_speed;

    // 오브젝트의 움직임을 제어
    bool canMove;

    // 이동 방향키 값을 받을 Vector
    Vector2 vector;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 오브젝트가 이동 할 수 있다면
        if (!canMove)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                ObejctMoving();
            }
        }
    }

    public void ObejctMoving()
    {
        // 현재 이동중이므로 true
        canMove = true;

        // LeftShift를 누르면 이동속도를 빠르게
        if (Input.GetKey(KeyCode.LeftShift))
            current_speed = run_speed;
        else
            current_speed = speed;

        // vector에 x, y 값을 Input으로 받아옴
        vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // 수평 이동이라면 y축을 0으로
        if (vector.x != 0)
            vector.y = 0;
        // 수직 이동이라면 x축을 0으로
        if (vector.y != 0)
            vector.x = 0;

        // 수평 이동
        if (vector.x != 0)
            transform.Translate(vector.x * Time.deltaTime * current_speed, 0, 0);
        // 수직 이동
        else if (vector.y != 0)
            transform.Translate(0, vector.y * Time.deltaTime * current_speed, 0);

        // 이동이 완료되면 false
        canMove = false;
    }
}

