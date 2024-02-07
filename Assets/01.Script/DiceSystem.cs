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

    [SerializeField] PlayerManager thePlayer; //?��?��?��?��?�� ?���?
    [SerializeField] Vector3 nowPos;//?��?�� ?��말의 ?���? ????��
    [SerializeField] GameObject EggObj; //주사?�� ?��브젝?��
    [SerializeField] Text diceNumText; //주사?�� ?���? ?��?��?��
    [SerializeField] Animator EggAnimator; //?��?��메이?�� ?��?�� 체크 ?��?�� �??��
    [SerializeField] bool animatorFlag; //?��?��메이?���? ?��?��?��고�???�� Update?�� 로직?�� ?��?��?��?��기위?�� 추�??.

    public bool diceFlag; //주사?���? 굴려졋는�? ?��?��?��?�� ?��?���?

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
        if (animatorFlag)
        {
            // EggAnimator�? Finish?�� ?��?���??�� ?��?��메이?��?�� 종료?��?���?
            if (!EggAnimator.GetCurrentAnimatorStateInfo(0).IsName("Egg"))
            {
                EggObj.SetActive(false);
                // thePlayer.canMove = true;
                animatorFlag = false;
            }
        }

    }

    // ?��말을 ?��?��그했?�� ?�� ?���?
    public void OnDrag(PointerEventData eventData)
    {
        // ?��?��??? ?��?��???빔의 ?��?��?�� 모두 ?��?��?�� ?�� 주사?���? 굴릴 ?�� ?���?(=> ?��?��중이?���? 굴릴 ?�� ?���?)
        if (thePlayer.myTurn && theGM.penetrateComplete && theGM.laserComplete && theTSI.cursorPos == 1)
        {
            Vector3 yPos = new Vector3(0f, eventData.position.y, 0f);

            if (yPos.y < 200f) yPos = new Vector3(0f, 200f, 0f);
            else if (yPos.y > nowPos.y) yPos = new Vector3(0f, nowPos.y, 0f);

            this.transform.localPosition = new Vector3(nowPos.x, yPos.y, nowPos.z);
        }
    }

    // ?��말을 ?���??��?�� ?�� ?���?
    public void OnEndDrag(PointerEventData eventData1)
    {
        // ?��?��??? ?��?��???빔의 ?��?��?�� 모두 ?��?��?�� ?�� 주사?���? 굴릴 ?�� ?���?(=> ?��?��중이?���? 굴릴 ?�� ?���?)
        if (thePlayer.myTurn && theGM.penetrateComplete && theGM.laserComplete && theTSI.cursorPos == 1)
        {
            // ?��말이 ?���? ?���? 좌표�? ?��?��?���? 주사?���? 굴림
            if (this.transform.localPosition.y < 470)
            {
                theTSI.cursorPos = 2;


                RollDice();
            }

            // ?��말의 ?��치�?? ?��?�� 초기 ?��치로 ?��?��?��?��
            this.transform.localPosition = nowPos;
        }



    }

    public void RollDice()
    {
        int dNum = Random.Range(1, 9);
        DiceData dData = new(dNum, GameManager.Instance.turnIndex); //?��버로 ?��?��?���? ?��?�� ?��?��?�� ?��?��?��?��
        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.Dice, JsonUtility.ToJson(dData));
        Backend.Match.SendDataToInGameRoom(data);
    }

    public IEnumerator RollDiceCoroutine()
    {
        print("diceCoroutine Start");
        yield return new WaitUntil(() => diceFlag == true); //?��버에?�� 주사?��값을 ????��?��?��까�?? 기다�?.


        //?��?��?�� ?��?��?���? 굴리�? ?��?��?��?��?���? ????��?���?�?, ?��?��?�� 게임매니????�� ????��?��?��?�� ?��?�� ?��?�� ?��?��?��?��?���? ?��?��?���?.
        // GameManager.Instance.diceNum = Random.Range(1,9);
        // 게임매니????�� ????��?��?�� �??��?�� EventManager�? ?��?��.
        //if() //?�� ?��?��?���? ?��?��?��?�� ?��?��?�� ?��?�� ????��.
        thePlayer.diceNum = GameManager.Instance.diceNum;
        AudioManager.instance.Play("RollDice_Sound");

        thePlayer.diceNum = Random.Range(1, 9);

        // 주사?��컨트�? 카드 ?��?�� ?��, ?��?�� ?��?�� ?���?
        if (thePlayer.lowerDiceFlag)
        {
            theCM.LowerDiceControl();
        }

        if (thePlayer.higherDiceFlag)
        {
            theCM.HigherDiceControll();
        }

        // 주사?���? ?��?��?���? 굴린 ?��?�� text?�� ?��?��
        diceNumText.text = thePlayer.diceNum.ToString();

        // ?��?���? ?��기시?�� ?��?��?���? ?��기고, 주사?���? ?��?��?��?���?, animatorFlag�? true�? 켜서 ?��?��?��?��문에 ?��?���?게함
        thePlayer.downInformationText.gameObject.SetActive(false);
        EggObj.SetActive(true);
        animatorFlag = true;
        diceFlag = false;
    }
}
