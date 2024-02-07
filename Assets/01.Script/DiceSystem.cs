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

    [SerializeField] PlayerManager thePlayer; //?”Œ? ˆ?´?–´?˜ ? •ë³?
    [SerializeField] Vector3 nowPos;//?˜„?¬ ?Œ»ë§ì˜ ?œ„ì¹? ????¥
    [SerializeField] GameObject EggObj; //ì£¼ì‚¬?œ„ ?˜¤ë¸Œì ?Š¸
    [SerializeField] Text diceNumText; //ì£¼ì‚¬?œ„ ?ˆˆê¸? ?ƒ?Š¤?Š¸
    [SerializeField] Animator EggAnimator; //?• ?‹ˆë©”ì´?„° ?ƒ?ƒœ ì²´í¬ ?œ„?•œ ë³??ˆ˜
    [SerializeField] bool animatorFlag; //?• ?‹ˆë©”ì´?„°ê°? ?‹œ?‘?˜ê³ ë???„° Update?˜ ë¡œì§?„ ?‹¤?–‰?‹œ?‚¤ê¸°ìœ„?•´ ì¶”ê??.

    public bool diceFlag; //ì£¼ì‚¬?œ„ê°? êµ´ë ¤ì¡‹ëŠ”ì§? ?™•?¸?•˜?Š” ?”Œ?˜ê·?

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
            // EggAnimatorê°? Finish?— ?“¤?–´ê°??„œ ?• ?‹ˆë©”ì´?…˜?´ ì¢…ë£Œ??‹¤ë©?
            if (!EggAnimator.GetCurrentAnimatorStateInfo(0).IsName("Egg"))
            {
                EggObj.SetActive(false);
                // thePlayer.canMove = true;
                animatorFlag = false;
            }
        }

    }

    // ?Œ»ë§ì„ ?“œ?˜ê·¸í–ˆ?„ ?•Œ ?˜¸ì¶?
    public void OnDrag(PointerEventData eventData)
    {
        // ?ˆ¬?‹œ??? ? ˆ?´???ë¹”ì˜ ?‚¬?š©?´ ëª¨ë‘ ??‚¬?„ ?•Œ ì£¼ì‚¬?œ„ë¥? êµ´ë¦´ ?ˆ˜ ?ˆê²?(=> ?‚¬?š©ì¤‘ì´?¼ë©? êµ´ë¦´ ?ˆ˜ ?—†ê²?)
        if (thePlayer.myTurn && theGM.penetrateComplete && theGM.laserComplete && theTSI.cursorPos == 1)
        {
            Vector3 yPos = new Vector3(0f, eventData.position.y, 0f);

            if (yPos.y < 200f) yPos = new Vector3(0f, 200f, 0f);
            else if (yPos.y > nowPos.y) yPos = new Vector3(0f, nowPos.y, 0f);

            this.transform.localPosition = new Vector3(nowPos.x, yPos.y, nowPos.z);
        }
    }

    // ?Œ»ë§ì„ ?“œë¡??–ˆ?„ ?•Œ ?˜¸ì¶?
    public void OnEndDrag(PointerEventData eventData1)
    {
        // ?ˆ¬?‹œ??? ? ˆ?´???ë¹”ì˜ ?‚¬?š©?´ ëª¨ë‘ ??‚¬?„ ?•Œ ì£¼ì‚¬?œ„ë¥? êµ´ë¦´ ?ˆ˜ ?ˆê²?(=> ?‚¬?š©ì¤‘ì´?¼ë©? êµ´ë¦´ ?ˆ˜ ?—†ê²?)
        if (thePlayer.myTurn && theGM.penetrateComplete && theGM.laserComplete && theTSI.cursorPos == 1)
        {
            // ?Œ»ë§ì´ ?¼ì²? ?œ„ì¹? ì¢Œí‘œë¥? ?„˜?–´?„œë©? ì£¼ì‚¬?œ„ë¥? êµ´ë¦¼
            if (this.transform.localPosition.y < 470)
            {
                theTSI.cursorPos = 2;


                RollDice();
            }

            // ?Œ»ë§ì˜ ?œ„ì¹˜ë?? ?‹¤?‹œ ì´ˆê¸° ?œ„ì¹˜ë¡œ ?Œ? ¤?†“?Œ
            this.transform.localPosition = nowPos;
        }



    }

    public void RollDice()
    {
        int dNum = Random.Range(1, 9);
        DiceData dData = new(dNum, GameManager.Instance.turnIndex); //?„œë²„ë¡œ ? „?†¡?•˜ê¸? ?œ„?•´ ?°?´?„° ?´?˜?Š¤?™”
        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.Dice, JsonUtility.ToJson(dData));
        Backend.Match.SendDataToInGameRoom(data);
    }

    public IEnumerator RollDiceCoroutine()
    {
        print("diceCoroutine Start");
        yield return new WaitUntil(() => diceFlag == true); //?„œë²„ì—?„œ ì£¼ì‚¬?œ„ê°’ì„ ????¥?• ?•Œê¹Œì?? ê¸°ë‹¤ë¦?.


        //?›?˜?Š” ?‹¤?´?Š¤ë¥? êµ´ë¦¬ë©? ?”Œ? ˆ?´?–´?—ê²? ????¥?–ˆì§?ë§?, ?´? œ?Š” ê²Œì„ë§¤ë‹ˆ????— ????¥?˜?–´?„œ ?˜„?¬ ?„´?˜ ?”Œ? ˆ?´?–´?—ê²? ?• ?‹¹?• ê²?.
        // GameManager.Instance.diceNum = Random.Range(1,9);
        // ê²Œì„ë§¤ë‹ˆ????— ????¥?‹œ?‚¬ ë³??ˆ˜?Š” EventManagerë¡? ?´?™.
        //if() //?‚´ ?ˆœ?„œ?¼ë©? ?”Œ? ˆ?´?–´ ?‹¤?´?Š¤ ?„˜?— ????¥.
        thePlayer.diceNum = GameManager.Instance.diceNum;
        AudioManager.instance.Play("RollDice_Sound");

        thePlayer.diceNum = Random.Range(1, 9);

        // ì£¼ì‚¬?œ„ì»¨íŠ¸ë¡? ì¹´ë“œ ?‚¬?š© ?‹œ, ?•´?‹¹ ?•¨?ˆ˜ ?˜¸ì¶?
        if (thePlayer.lowerDiceFlag)
        {
            theCM.LowerDiceControl();
        }

        if (thePlayer.higherDiceFlag)
        {
            theCM.HigherDiceControll();
        }

        // ì£¼ì‚¬?œ„ë¥? ?œ?¤?•˜ê²? êµ´ë¦° ?‹¤?Œ text?— ? ?š©
        diceNumText.text = thePlayer.diceNum.ToString();

        // ?•„?˜ë¡? ?‹¹ê¸°ì‹œ?˜¤ ?…?Š¤?Š¸ë¥? ?ˆ¨ê¸°ê³ , ì£¼ì‚¬?œ„ë¥? ?™œ?„±?™”?•˜ê³?, animatorFlagë¥? trueë¡? ì¼œì„œ ?—…?°?´?Š¸ë¬¸ì— ?“¤?–´ê°?ê²Œí•¨
        thePlayer.downInformationText.gameObject.SetActive(false);
        EggObj.SetActive(true);
        animatorFlag = true;
        diceFlag = false;
    }
}
