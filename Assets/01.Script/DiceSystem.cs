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
    }

    void Update()
    {
        if (animatorFlag)
        {
            // EggAnimator가 Finish에 들어가서 애니메이션이 종료됐다면
            if (!EggAnimator.GetCurrentAnimatorStateInfo(0).IsName("Egg"))
            {
                // 주사위 비활성화, movingFlag을 true로 바꿔서 움직이게함
                EggObj.SetActive(false);
                //플레이어 무브 플래그 활성화
                thePlayer.movingCoroutineFlag = true;
                animatorFlag = false;
            }
        }

    }

    // // 해당 스크립트가 붙은 오브젝트(팻말)를 클릭했을 때 호출
    // public void OnPointerDown(PointerEventData eventData2)
    // {
    //     if (thePlayer.myTurn)
    //     {
    //         // 주사위를 굴릴때 cursorPos를 2로 변경
    //         // OnDrag(eventData2);
    //     }
    // }

    // 해당 스크립트가 붙은 오브젝트(팻말)을 드래그했을 때 호출
    public void OnDrag(PointerEventData eventData)
    {
        if (thePlayer.myTurn)
        {
            // eventData의 position.y가 아니라 마우스를 기준으로 받아오면 좋을듯
            Vector3 ypos = new Vector3(0f, eventData.position.y, 0f);
            if (ypos.y < 200f)
            {
                ypos = new Vector3(0f, 200f, 0f);
            }
            else if (ypos.y > nowPos.y)
            {
                ypos = new Vector3(0f, nowPos.y, 0f);
            }
            this.transform.localPosition = new Vector3(nowPos.x, ypos.y, nowPos.z);
        }
    }

    public void OnEndDrag(PointerEventData eventData1)
    {
        if (thePlayer.myTurn)
        {
            if (this.transform.localPosition.y < 470)
            {
                theTSI.cursorPos = 2;

                //AudioManager.instance.Play("diceSound");

                // 팻말의 위치를 다시 초기 위치로 돌려놓음
                this.transform.localPosition = nowPos;

                thePlayer.diceNum = Random.Range(1, 9); //테스트중, 끝나면 다시 변환

                if (thePlayer.lowerDiceFlag)
                {
                    theCM.LowerDiceControl();
                }

                if (thePlayer.higherDiceFlag)
                {
                    theCM.HigherDiceControll();
                }

                // diceFlag를 true로 바꾸고 주사위를 랜덤하게 굴린 다음 text에 적용
                thePlayer.diceFlag = true;
                diceNumText.text = thePlayer.diceNum.ToString();

                // 아래로 당기시오 텍스트를 숨기고, 주사위를 활성화하고, animatorFlag를 true로 켜서 업데이트문에 들어가게함
                thePlayer.downInformationText.gameObject.SetActive(false);
                EggObj.SetActive(true);
                animatorFlag = true;
            }
            else
            {
                // 팻말의 위치를 다시 초기 위치로 돌려놓음
                this.transform.localPosition = nowPos;
            }
        }
    }
}
