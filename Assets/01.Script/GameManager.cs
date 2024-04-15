using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UnityEngine;
using BackEnd.Tcp;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GameManager : MonoBehaviour
{
    #region instance
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            return _instance;
        }
    }
    #endregion

    [Header("Player")]
    // 게임에 참가하는 플레이어
    public PlayerManager[] players = new PlayerManager[2];
    // 플레이어의 이미지
    public Sprite[] signSprites = new Sprite[2];
    // 턴이 돌아온 플레이어
    public PlayerManager nowPlayer;
    // 플레이어가 컨트롤하는 플레이어
    public PlayerManager myCharactor;


    [Space(10), Header("Card")]
    // 카드 7종의 정보를 담은 카드
    public Card[] cards;
    // 획득한 카드가 들어갈 오브젝트
    public GameObject cardsObj;
    // 카드 프리팹
    public GameObject cardPrefab;
    // 카드 획득 시 카드가 담길 오브젝트
    public GameObject showCardObject;
    // 카드 이미지 프리팹
    public GameObject onlyCardImg;
    // 턴 카드 리스트
    public List<GameObject> turnCards = new List<GameObject>();
    // 턴 카드가 들어갈 오브젝트
    public GameObject turnCardParent;
    //카드 UI를 위한 변수
    //카드를 사용했을 때, 다음위치에 있는 카드가 선택되지 않게 하기 위함.
    public bool cardActive;


    [Space(10), Header("Building")]
    // 빌딩 4종의 정보를 담은 빌딩
    public Building[] buildings = new Building[4];


    [Space(10), Header("Trun")]
    // 턴 카드를 뽑은 횟수
    public List<int> playerCount = new();
    // 현재 진행된 턴 수
    public int turnCount;
    // 플레이어의 턴 순서
    public int turnIndex;
    // 턴을 넘기는 플래그
    public bool nextTurn;


    [Space(10), Header("Flag")]
    public bool UIFlag = false;
    public bool laserComplete = true;
    public bool isActiveTurnImage;


    [Space(10), Header("UI")]
    [SerializeField] GameObject player1TurnImg;
    [SerializeField] GameObject player2TurnImg;
    public GameObject player1TeleportEffect, player2TeleportEffect;
    public GameObject player1_floatingObject, player2_floatingObject;
    public GameObject floatingTextPrefab;
    public GameObject gameOverUI;
    public GameObject RestartBtn;
    public GameObject errorUI;


    [Space(10), Header("ETC")]
    // 타일 선택 시 선택된 타일
    public GameObject seletedTile;
    public int diceNum;
    public int invisibleCardNum = -1;

    TurnSignScript theTSI;


    void Start()
    {
        TurnCardSet tsdata = new(Random.Range(0, 2));
        string jsonData = JsonUtility.ToJson(tsdata);
        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.TurnCardSet, jsonData);
        Backend.Match.SendDataToInGameRoom(data);

        AudioManager.Instance.Play("MainGame_Sound");
        theTSI = FindObjectOfType<TurnSignScript>();

        StartCoroutine(RestartCoroutine());
    }

    void Update()
    {
        // 모든 플레이어가 턴카드를 뽑았고, 턴이 넘어갔을 때
        if (playerCount.Count > 1 && nextTurn)
        {
            nextTurn = false;
            theTSI.cursorPos = 1;

            if (turnCount % 2 == turnIndex)
            {
                myCharactor.myTurn = true;
                nowPlayer = myCharactor;
            }
            else
            {
                myCharactor.myTurn = false;
                nowPlayer = myCharactor.againstPlayer;
                if (nowPlayer.tpFlag) nowPlayer.myTurn = true;
            }

            AudioManager.Instance.Play("TurnChange_Sound");
            StartCoroutine(TurnImgCoroutine(turnCount % 2));
            CardListUpdate();
        }
    }

    // 플레이어가 가진 카드의 목록 업데이트
    public void CardListUpdate()
    {
        if (cardsObj.transform.childCount != 0)
        {
            for (int i = 0; i < cardsObj.transform.childCount; i++)
            {
                Destroy(cardsObj.transform.GetChild(i).gameObject);
            }
        }

        if (myCharactor.cards.Count > 0)
        {
            for (int i = 0; i < myCharactor.cards.Count; i++)
            {
                var _card = Instantiate(cardPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, cardsObj.transform);
                _card.transform.localPosition = new Vector3(0f, 0f, 0f);
                _card.GetComponent<CardManager>().cardInfo = myCharactor.cards[i];
                _card.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.cards[myCharactor.cards[i].cardCode - 1].cardImg;
            }
        }
    }

    // 턴 넘기기 및 게임오버 체크
    public void NextTurnFunc()
    {
        int gameOver = CheckGameOver();

        // Player1 승리
        if (gameOver == 1)
        {
            UIManager.Instance.gameoverUI.SetActive(true);
            UIManager.Instance.goImg.GetComponent<UnityEngine.UI.Image>().sprite = UIManager.Instance.winImg[0];

            MatchGameResult matchGameResult = new MatchGameResult();

            if (myCharactor == players[0])
            {
                UIManager.Instance.goTitle.text = "WIN!";
                UIManager.Instance.goMoney.text = "보유금액 : " + myCharactor.playerMoney.ToString();
            }
            else
            {
                UIManager.Instance.goTitle.text = "LOSE..";
                UIManager.Instance.goMoney.text = "보유금액 : " + myCharactor.playerMoney.ToString();
            }
            Backend.Match.MatchEnd(matchGameResult);
        }
        // Player2 승리
        else if (gameOver == 0)
        {
            UIManager.Instance.gameoverUI.SetActive(true);
            UIManager.Instance.goImg.GetComponent<UnityEngine.UI.Image>().sprite = UIManager.Instance.winImg[1];

            MatchGameResult matchGameResult = new MatchGameResult();

            if (myCharactor == players[1])
            {
                UIManager.Instance.goTitle.text = "WIN!";
                UIManager.Instance.goMoney.text = "보유금액 : " + myCharactor.playerMoney.ToString();
            }
            else
            {
                UIManager.Instance.goTitle.text = "LOSE..";
                UIManager.Instance.goMoney.text = "보유금액 : " + myCharactor.playerMoney.ToString();
            }
            Backend.Match.MatchEnd(matchGameResult);
        }
        else
        {
            turnCount += 1;
            nextTurn = true;
        }
    }

    IEnumerator TurnImgCoroutine(int turn)
    {
        isActiveTurnImage = true;

        if (turn == 1)
        {
            player1TurnImg.SetActive(true);
            UIManager.Instance.player1Text.text = GameManager.Instance.players[0].nickname + "의 차례입니다!";
            yield return new WaitForSeconds(1f);

            player1TurnImg.SetActive(false);
        }
        if (turn == 0)
        {
            player2TurnImg.SetActive(true);
            UIManager.Instance.player2Text.text = GameManager.Instance.players[1].nickname + "의 차례입니다!";
            yield return new WaitForSeconds(1f);

            player2TurnImg.SetActive(false);
        }

        if (myCharactor.myTurn) myCharactor.downInformationText.gameObject.SetActive(true);
        else myCharactor.downInformationText.gameObject.SetActive(false);

        isActiveTurnImage = false;
    }

    int CheckGameOver()
    {
        int i;
        for (i = 0; i < players.Length; i++)
        {
            if (players[i].playerMoney < 0) return i;
        }
        return i;
    }

    public void SetFloatingText(PlayerManager _player, int _value, bool sign)
    {
        if (_player.playerId == 0)
        {
            GameObject prefab = Instantiate(floatingTextPrefab, player1_floatingObject.transform);
            prefab.transform.GetChild(0).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Left;
            prefab.transform.GetChild(1).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Left;

            if (sign)
            {
                prefab.transform.GetChild(0).GetComponent<TMP_Text>().text = "+" + _value.ToString();
                prefab.transform.GetChild(1).GetComponent<TMP_Text>().text = "+" + _value.ToString();
                prefab.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.green;
            }
            else
            {
                prefab.transform.GetChild(0).GetComponent<TMP_Text>().text = "-" + _value.ToString();
                prefab.transform.GetChild(1).GetComponent<TMP_Text>().text = "-" + _value.ToString();
                prefab.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.red;
            }
        }
        else if (_player.playerId == 1)
        {
            GameObject prefab = Instantiate(floatingTextPrefab, player2_floatingObject.transform);
            prefab.transform.GetChild(0).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Right;
            prefab.transform.GetChild(1).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Right;

            if (sign)
            {
                prefab.transform.GetChild(0).GetComponent<TMP_Text>().text = "+" + _value.ToString();
                prefab.transform.GetChild(1).GetComponent<TMP_Text>().text = "+" + _value.ToString();
                prefab.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.green;
            }
            else
            {
                prefab.transform.GetChild(0).GetComponent<TMP_Text>().text = "-" + _value.ToString();
                prefab.transform.GetChild(1).GetComponent<TMP_Text>().text = "-" + _value.ToString();
                prefab.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.red;
            }
        }
    }

    IEnumerator RestartCoroutine()
    {
        //5초동안 게임 시작 불가능하다면
        yield return new WaitForSeconds(3f);
        RestartBtn.SetActive(true);
    }
}