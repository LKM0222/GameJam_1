using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiceSystem : MonoBehaviour,IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [SerializeField] PlayerManager thePlayer; //플레이어의 정보
    [SerializeField] Vector3 nowPos;//현재 팻말의 위치 저장
    [SerializeField] GameObject EggObj; //주사위 오브젝트
    [SerializeField] Text diceNumText; //주사위 눈금 택스트
    [SerializeField] Animator EggAnimator; //애니메이터 상태 체크 위한 변수
    [SerializeField] bool animatorFlag; //애니메이터가 시작되고부터 Update의 로직을 실행시키기위해 추가.
    TurnSignScript theTSI;
    // Start is called before the first frame update
    void Start()
    {
        nowPos = this.transform.localPosition;
        theTSI = FindObjectOfType<TurnSignScript>();
    }
    private void Update()
    {
        if(animatorFlag){
            if(!EggAnimator.GetCurrentAnimatorStateInfo(0).IsName("Egg")){ //애니메이션이 종료되었다면
                //애니메이션 오브젝트를 비활성화 시키고
                EggObj.SetActive(false);
                //플레이어 무브 플래그 활성화
                thePlayer.movingFlag = true;
                animatorFlag = false;
            }
        }
        
    }

    public void OnPointerDown(PointerEventData eventData2){
        print("Pointer Down");
        theTSI.cursorPos = 2;
        OnDrag(eventData2);
    }

    public void OnDrag(PointerEventData eventData){
        if(thePlayer.myTurn){
            Vector3 ypos = new Vector3(0f, eventData.position.y, 0f);
            if(ypos.y < 200f){
                ypos = new Vector3(0f, 200f, 0f);
            }
            this.transform.localPosition = new Vector3(nowPos.x, ypos.y , nowPos.z);
        }
    }
    public void OnEndDrag(PointerEventData eventData1){
        this.transform.localPosition = nowPos;
        EggObj.SetActive(true);
        thePlayer.diceNum = Random.Range(1,9); //테스트중, 끝나면 다시 변환
        thePlayer.diceFlag = true;
        diceNumText.text = thePlayer.diceNum.ToString();
        thePlayer.downInformationText.gameObject.SetActive(false);
        animatorFlag = true;
    }

}
