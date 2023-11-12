using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeMove : MonoBehaviour
{
    // 주사위 오브젝트 2개
    public GameObject dice1;
    public GameObject dice2;

    // 주사위를 던지는 속도
    public float speed;

    // 주사위들이 던져졌는지 아닌지
    public bool dice1_isJumpping;
    public bool dice2_isJumpping;

    // 주사위들의 Rigidbody
    public Rigidbody dice1_Rb;
    public Rigidbody dice2_Rb;

    // 주사위들의 숫자
    public int rand_num1;
    public int rand_num2;

    public static cubeMove instance;

    // 주사위들의 Animator
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
        // 주사위 두개가 바닥에 있다면
        if (!dice1_isJumpping && !dice2_isJumpping)
        {
            // 마우스 좌클릭으로 주사위 던지기
            if (Input.GetMouseButtonDown(0))
            {
                // 애니메이션 초기화
                dice1_animator.SetBool("Throw", false);
                dice1_animator.SetInteger("Number", 0);
                dice2_animator.SetBool("Throw", false);
                dice2_animator.SetInteger("Number", 0);

                //랜덤 숫자 부여
                rand_num1 = Random.Range(1, 7);
                rand_num2 = Random.Range(1, 7);

                // 애니메이션 대기 시간 코루틴
                StartCoroutine(ThrowDice());

                // 주사위가 현재 던져져있음
                dice1_isJumpping = true;
                dice2_isJumpping = true;

                dice1.transform.position = new Vector3(-1, 1f, plane.transform.position.z);
                dice2.transform.position = new Vector3(1, 1f, plane.transform.position.z);

                // 주사위 로테이션 초기화
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