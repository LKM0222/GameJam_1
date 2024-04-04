using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiceSystem : MonoBehaviour, IDragHandler, IEndDragHandler
{
    TurnSignScript theTurnSign;

    [SerializeField] Vector3 nowPos;
    [SerializeField] GameObject diceObject;
    [SerializeField] Text diceNumText;
    [SerializeField] Animator diceAnimator;
    [SerializeField] bool animatorFlag;

    public bool diceFlag;
    public bool isDrag;

    void Start()
    {
        nowPos = this.transform.localPosition;
        theTurnSign = FindObjectOfType<TurnSignScript>();
    }

    void Update()
    {
        // 애니메이션이 실행됐고, 그 애니메이션이 종료됐다면
        if (animatorFlag && !diceAnimator.GetCurrentAnimatorStateInfo(0).IsName("Egg"))
        {
            diceObject.SetActive(false);
            GameManager.Instance.nowPlayer.canMove = true;
            animatorFlag = false;
        }
    }

    // 팻말을 드래그했을 때 호출
    public void OnDrag(PointerEventData eventData)
    {
        isDrag = true;

        if (GameManager.Instance.nowPlayer.name == this.name[..6])
        {
            if (GameManager.Instance.myCharactor.myTurn && GameManager.Instance.laserComplete && theTurnSign.cursorPos == 1)
            {
                Vector3 yPos = new Vector3(0f, eventData.position.y, 0f);

                if (yPos.y < 200f) yPos = new Vector3(0f, 200f, 0f);
                else if (yPos.y > nowPos.y) yPos = new Vector3(0f, nowPos.y, 0f);

                this.transform.localPosition = new Vector3(nowPos.x, yPos.y, nowPos.z);
            }
        }
    }

    // 팻말을 드롭했을 때 호출
    public void OnEndDrag(PointerEventData eventData1)
    {
        isDrag = false;

        if (GameManager.Instance.myCharactor.myTurn && GameManager.Instance.laserComplete && theTurnSign.cursorPos == 1)
        {
            if (this.transform.localPosition.y < 470)
            {
                theTurnSign.cursorPos = 2;

                RollDice();
            }
            this.transform.localPosition = nowPos;
        }
    }

    public void RollDice()
    {
        int diceNum = UnityEngine.Random.Range(1, 9);

        if (GameManager.Instance.nowPlayer.lowerDiceFlag)
        {
            diceNum = UnityEngine.Random.Range(1, 5);
            GameManager.Instance.nowPlayer.lowerDiceFlag = false;
        }

        if (GameManager.Instance.nowPlayer.higherDiceFlag)
        {
            diceNum = UnityEngine.Random.Range(5, 9);
            GameManager.Instance.nowPlayer.higherDiceFlag = false;
        }

        DiceData dData = new(diceNum, GameManager.Instance.turnIndex);
        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.Dice, JsonUtility.ToJson(dData));
        Backend.Match.SendDataToInGameRoom(data);
    }

    public IEnumerator RollDiceCoroutine()
    {
        yield return new WaitUntil(() => diceFlag == true);

        GameManager.Instance.nowPlayer.diceNum = GameManager.Instance.diceNum;

        AudioManager.Instance.Play("RollDice_Sound");

        GameManager.Instance.nowPlayer.downInformationText.gameObject.SetActive(false);
        diceNumText.text = GameManager.Instance.nowPlayer.diceNum.ToString();
        diceObject.SetActive(true);
        animatorFlag = true;
    }
}
