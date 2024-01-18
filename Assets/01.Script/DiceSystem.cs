using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiceSystem : MonoBehaviour, IDragHandler, IEndDragHandler
{

    TurnSignScript theTSI;
    CardManager theCM;
    GameManager theGM;

    [SerializeField] PlayerManager thePlayer; //플레이어의 정보
    [SerializeField] Vector3 nowPos;//현재 팻말의 위치 저장
    [SerializeField] GameObject EggObj; //주사위 오브젝트
    [SerializeField] Text diceNumText; //주사위 눈금 택스트
    [SerializeField] Animator EggAnimator; //애니메이터 상태 체크 위한 변수
    [SerializeField] bool animatorFlag; //애니메이터가 시작되고부터 Update의 로직을 실행시키기위해 추가.

    // Start is called before the first frame update
    void Start()
    {
        nowPos = this.transform.localPosition;
        theTSI = FindObjectOfType<TurnSignScript>();
        theCM = FindObjectOfType<CardManager>();
        theGM = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (animatorFlag)
        {
            // EggAnimator가 Finish에 들어가서 애니메이션이 종료됐다면
            if (!EggAnimator.GetCurrentAnimatorStateInfo(0).IsName("Egg"))
            {
                EggObj.SetActive(false);
                thePlayer.canMove = true;
                animatorFlag = false;
            }
        }
    }

    // 팻말을 드래그했을 때 호출
    public void OnDrag(PointerEventData eventData)
    {
        // 투시와 레이저빔의 사용이 모두 끝났을 때 주사위를 굴릴 수 있게(=> 사용중이라면 굴릴 수 없게)
        if (thePlayer.myTurn && theGM.penetrateComplete && theGM.laserComplete)
        {
            Vector3 yPos = new Vector3(0f, eventData.position.y, 0f);

            if (yPos.y < 200f) yPos = new Vector3(0f, 200f, 0f);
            else if (yPos.y > nowPos.y) yPos = new Vector3(0f, nowPos.y, 0f);

            this.transform.localPosition = new Vector3(nowPos.x, yPos.y, nowPos.z);
        }
    }

    // 팻말을 드롭했을 때 호출
    public void OnEndDrag(PointerEventData eventData1)
    {
        // 투시와 레이저빔의 사용이 모두 끝났을 때 주사위를 굴릴 수 있게(=> 사용중이라면 굴릴 수 없게)
        if (thePlayer.myTurn && theGM.penetrateComplete && theGM.laserComplete)
        {
            // 팻말이 일청 위치 좌표를 넘어서면 주사위를 굴림
            if (this.transform.localPosition.y < 470)
            {
                theTSI.cursorPos = 2;

                //AudioManager.instance.Play("diceSound");

                RollDice();
            }

            // 팻말의 위치를 다시 초기 위치로 돌려놓음
            this.transform.localPosition = nowPos;
        }
    }

    public void RollDice()
    {
        thePlayer.diceNum = Random.Range(1, 9);

        // 주사위컨트롤 카드 사용 시, 해당 함수 호출
        if (thePlayer.lowerDiceFlag)
        {
            theCM.LowerDiceControl();
        }

        if (thePlayer.higherDiceFlag)
        {
            theCM.HigherDiceControll();
        }

        // 주사위를 랜덤하게 굴린 다음 text에 적용
        diceNumText.text = thePlayer.diceNum.ToString();

        // 아래로 당기시오 텍스트를 숨기고, 주사위를 활성화하고, animatorFlag를 true로 켜서 업데이트문에 들어가게함
        thePlayer.downInformationText.gameObject.SetActive(false);
        EggObj.SetActive(true);
        animatorFlag = true;
    }
}
