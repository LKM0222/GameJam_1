using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations;

public class MovingObject : MonoBehaviour
{
    // 오브젝트 기본 이동속도
    public float speed;
    // 오브젝트 달리기 이동속도
    public float run_speed;
    // 달리기 유무에 따른 이동속도
    float current_speed;

    // 오브젝트의 움직임을 제어
    bool canMove = true;

    // 이동 방향키 값을 받을 Vector
    Vector2 vector;

    // Animation을 위한 Animator
    Animator animator;

    // 플레이어가 비행을 시작하고 바닥에 내려오는 시간
    float waitTime;

    // 플레이어가 비행 능력을 사용했는지 여부
    bool flying;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 오브젝트가 이동 할 수 있다면
        if (canMove)
        {
            // 방향키가 입력되면 움직이는 기능을 하는 Coroutine을 호출
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                canMove = false;
                StartCoroutine(MoveCoroutine());
            }
        }

        // 만약 비행중이 아니라면
        if (!flying)
        {
            // 제자리에 서있더라도 Z키를 누르면 flying을 true로 바꾸고 애니메이션 실행 후, coroutine으로 3초 대기를 시켜줌
            if (Input.GetKeyDown(KeyCode.Z))
            {
                flying = true;
                animator.SetBool("Flying", true);
                StartCoroutine(WaitTimeCoroutine());
            }
        }
    }

    // 오브젝트를 움직이는 실질적인 함수
    IEnumerator MoveCoroutine()
    {
        // 방향키를 계속 누르고 있다면 Animation이 초기화되지 않게하기 위해서 While문 사용
        while (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
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


            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);
            animator.SetBool("Walking", true);


            // 수평 이동
            if (vector.x != 0)
                transform.Translate(vector.x * Time.deltaTime * current_speed, 0, 0);
            // 수직 이동
            else if (vector.y != 0)
                transform.Translate(0, vector.y * Time.deltaTime * current_speed, 0);

            // 이동이 완료되면 false
            canMove = false;

            yield return new WaitForSeconds(0.01f);
        }
        // 방향키에서 손을 떼면 Walking Animation 종료
        animator.SetBool("Walking", false);

        // 이동이 끝났으니 이제 움직일 수 있다는 뜻의 true
        canMove = true;
    }

    // 3초 대기 후 애니메이션 전환하는 Coroutine
    IEnumerator WaitTimeCoroutine()
    {

        yield return new WaitForSeconds(3f);
        animator.SetBool("Flying", false);
        flying = false;
    }
}

