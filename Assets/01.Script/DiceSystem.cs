using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
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

    public bool diceFlag; //주사위가 굴려졋는지 확인하는 플래그

    public bool isDrag;

    #region Instance
    private static DiceSystem _instance;
    public static DiceSystem Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType(typeof(DiceSystem)) as DiceSystem;

            return _instance;
        }
    }
    #endregion

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
        try
        {
            thePlayer = GameManager.Instance.nowPlayer.GetComponent<PlayerManager>();
        }
        catch (NullReferenceException)
        {

        }
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
        isDrag = true;
        // 투시와 레이저빔의 사용이 모두 끝났을 때 주사위를 굴릴 수 있게(=> 사용중이라면 굴릴 수 없게)
        //if (thePlayer.myTurn && theGM.penetrateComplete && theGM.laserComplete && theTSI.cursorPos == 1)
        if (GameManager.Instance.myCharactor.myTurn && theGM.penetrateComplete && theGM.laserComplete && theTSI.cursorPos == 1)
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
        isDrag = false;
        // 투시와 레이저빔의 사용이 모두 끝났을 때 주사위를 굴릴 수 있게(=> 사용중이라면 굴릴 수 없게)
        //if (thePlayer.myTurn && theGM.penetrateComplete && theGM.laserComplete && theTSI.cursorPos == 1)
        if (GameManager.Instance.myCharactor.myTurn && theGM.penetrateComplete && theGM.laserComplete && theTSI.cursorPos == 1)
        {
            // 팻말이 일청 위치 좌표를 넘어서면 주사위를 굴림
            if (this.transform.localPosition.y < 470)
            {
                theTSI.cursorPos = 2;


                RollDice();
            }

            // 팻말의 위치를 다시 초기 위치로 돌려놓음
            this.transform.localPosition = nowPos;
        }



    }

    public void RollDice()
    {
        int dNum = UnityEngine.Random.Range(1, 9);
        DiceData dData = new(dNum, GameManager.Instance.turnIndex); //서버로 전송하기 위해 데이터 클래스화
        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.Dice, JsonUtility.ToJson(dData));
        Backend.Match.SendDataToInGameRoom(data);
    }

    public IEnumerator RollDiceCoroutine()
    {
        print("diceCoroutine Start");
        yield return new WaitUntil(() => diceFlag == true); //서버에서 주사위값을 저장할때까지 기다림.


        //원래는 다이스를 굴리면 플레이어에게 저장했지만, 이제는 게임매니저에 저장되어서 현재 턴의 플레이어에게 할당할것.
        // GameManager.Instance.diceNum = Random.Range(1,9);
        // 게임매니저에 저장시킬 변수는 EventManager로 이동.
        //if() //내 순서라면 플레이어 다이스 넘에 저장.
        thePlayer.diceNum = GameManager.Instance.diceNum;
        AudioManager.instance.Play("RollDice_Sound");

        thePlayer.diceNum = UnityEngine.Random.Range(1, 9);

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
        diceFlag = false;
    }
}
