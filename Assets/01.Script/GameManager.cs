using System.Collections;
using System.Collections.Generic;
using BackEnd;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BackEnd.Tcp;
using UnityEngine.UIElements;
using System.Linq;
using Demo_Project;

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

    // 플레이어 이미지 스프라이트
    public Sprite[] signSprites = new Sprite[2];
    // public BuildingType[] buildingSprite = new BuildingType[1];
    // 모든 카드의 정보를 담는 Card 배열
    public Card[] cards;

    // 모든 빌딩의 정보를 담는 Building 배열
    public Building[] buildings = new Building[4];

    //전체 플레이어 리스트
    public PlayerManager[] players = new PlayerManager[2];
    // 현재 턴을 가진 플레이어 정보
    public PlayerManager nowPlayer;
    public PlayerManager controlPlayer;

    public TextManager textManager;

    // 현재 진행된 턴 수
    public int turnCount;

    // 턴을 종료하고 턴을 넘기는 체크 플래그
    public bool nextTurn = false;

    //UI가 활성화 되어있는지 확인하는 플래그
    public bool UIFlag = false;

    // 투시, 레이저빔의 카드 사용이 완료되었는지 체크하는 플래그
    public bool penetrateComplete = true;
    public bool laserComplete = true;

    // 카드를 담을 GameObject
    public GameObject cardsObj;
    // 카드 Prefab
    public GameObject cardPrefab;

    TurnSignScript theTSI;

    public GameObject clickedTile;

    public GameObject seletedTile; //텔레포트 활성화 시 다음턴에 움질일 위치 정함.
    public GameObject tpImg; //텔레포트 활성화 시 표시할 그림

    // 투시를 사용하면 카드들을 보여줄 오브젝트
    public GameObject showCardListObject;
    // 카드를 획득하면 보여줄 오브젝트
    public GameObject showCardObject;
    // 카드 이미지만 담은 프리팹
    public GameObject onlyCardImg;


    #region Turn
    [Header("TurnCard")]
    public List<int> playerCount = new();
    public int turnIndex; //자신이 몇번째 턴인지 정보 저장.

    public List<GameObject> turnCards = new List<GameObject>();

    public GameObject turnCardParent;

    public PlayerManager myCharactor;

    #endregion

    #region Dice
    public int diceNum;
    #endregion

    [SerializeField] GameObject player1TurnImg, player2TurnImg;
    public bool isActiveTurnImage;
    public GameObject player1TeleportEffect, player2TeleportEffect;
    public GameObject gameOverUI;

    public GameObject player1_floatingObject, player2_floatingObject;
    public GameObject floatingTextPrefab;

    //server
    public bool successFalg = false;

    //Invisible
    public int invisibleCardNum = -1;

    //SessionID
    public SessionId mySessionId;
    public SessionId[] sessionArr = new SessionId[2]; //0: 2player, 1: 1Player

    // Start is called before the first frame update
    void Start()
    {
        //턴 선택 카드 번호 랜덤으로 설정
        TurnCardSet tsdata = new(Random.Range(0, 2));
        string jsonData = JsonUtility.ToJson(tsdata);
        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.TurnCardSet, jsonData);
        Backend.Match.SendDataToInGameRoom(data);

        theTSI = FindObjectOfType<TurnSignScript>();
        //AudioManager.instance.Play("mainSound");
        //턴 선택 카드 번호 랜덤으로 설정(이거 eventmanager로 이동한거 아님?)
        // if (Random.Range(0, 2) == 0)
        // {
        //     turnCards[0].GetComponent<ButtonScript>().turnNum = 1;
        //     turnCards[1].GetComponent<ButtonScript>().turnNum = 2;
        // }
        // else
        // {
        //     turnCards[0].GetComponent<ButtonScript>().turnNum = 2;
        //     turnCards[1].GetComponent<ButtonScript>().turnNum = 1;
        // }
        AudioManager.instance.Play("MainGame_Sound");
    }

    // Update is called once per frame
    void Update()
    {
        //이 모든 과정을 턴 카드를 뽑았을때 실행되도록 수정.
        //턴카드를 뽑았을 때, palyerCount가 1씩 증가. 길이가 2가 됐다면 플레이어가 모두 카드를 뽑았다는 뜻. 이때부터 게임 시작.
        if (playerCount.Count > 1)
        {
            //여기서 컨트롤 플레이어에 맞춰서 카드 보여줘야될거같은데
            //사실 컨트롤 플레이어가 필요없긴 한데... 일단 그런 식으로 할꺼다라는 말.

            // 턴을 종료하고 상대 턴으로 넘어갔다면
            if (nextTurn)
            {
                /*
                //나머지가 1이면 1플레이어, 0이면 2플레이어
                if (turnCount % 2 == 1)
                {
                    //player의 myturn을 하나로 만들어야될듯....
                    // 각각의 플레이어의 myTurn을 바꿔주고 nowPlayer를 현재 턴을 가진 플레이어로 바꿈
                    players[0].myTurn = true;
                    players[1].myTurn = false;
                    nowPlayer = players[0];
                    CardListUpdate();
                AudioManager.instance.Play("TurnChange_Sound");

                }
                else
                {
                    // 각각의 플레이어의 myTurn을 바꿔주고 nowPlayer를 현재 턴을 가진 플레이어로 바꿈
                    players[1].myTurn = true;
                    players[0].myTurn = false;
                    nowPlayer = players[1];
                    CardListUpdate();
                AudioManager.instance.Play("TurnChange_Sound");

                }
                */
                print("turnCount is " + turnCount % 2);
                CardListUpdate();
                if (turnCount % 2 == turnIndex)
                {
                    //player의 myturn을 하나로 만들어야될듯....
                    // 각각의 플레이어의 myTurn을 바꿔주고 nowPlayer를 현재 턴을 가진 플레이어로 바꿈
                    myCharactor.myTurn = true;
                    nowPlayer = myCharactor;
                    // CardListUpdate();
                }
                else
                {
                    // 각각의 플레이어의 myTurn을 바꿔주고 nowPlayer를 현재 턴을 가진 플레이어로 바꿈
                    myCharactor.myTurn = false;
                    nowPlayer = myCharactor.againstPlayer;
                    if (nowPlayer.tpFlag)
                    {
                        nowPlayer.myTurn = true;
                    }
                    // CardListUpdate();
                }
                nextTurn = false;
                theTSI.cursorPos = 1;
                StartCoroutine(TurnImgCoroutine(turnCount % 2));
            }
        }

    }

    // 플레이어가 가진 카드의 목록 업데이트
    public void CardListUpdate()
    {
        // 카드를 1개 이상 가지고 있다면 전부 삭제
        if (cardsObj.transform.childCount != 0)
        {
            for (int i = 0; i < cardsObj.transform.childCount; i++)
            {
                Destroy(cardsObj.transform.GetChild(i).gameObject);
            }
        }

        // 삭제 이후 플레이어가 가진 카드 갯수만큼 다시 복제
        if (myCharactor.cards.Count > 0)
        {
            for (int i = 0; i < myCharactor.cards.Count; i++)
            {
                // 카드 프리팹을 복제한 이후, 위치를 맞춰주고, 카드의 정보와 이미지를 바꿔준다.
                var _card = Instantiate(cardPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, cardsObj.transform);
                _card.transform.localPosition = new Vector3(0f, 0f, 0f);
                _card.GetComponent<CardManager>().cardInfo = myCharactor.cards[i];
                _card.GetComponent<SpriteRenderer>().sprite = myCharactor.cards[i].cardImg;
            }
        }
    }

    public void NextTurnFunc()
    {
        if (CheckGameOver() < 2)
        {
            MatchGameResult matchGameResult = new MatchGameResult();
            matchGameResult.m_winners = new List<SessionId>();
            matchGameResult.m_losers = new List<SessionId>();

            if(CheckGameOver() == 1){
                //player2 패배
                matchGameResult.m_winners.Add(sessionArr[1]);
                matchGameResult.m_losers.Add(sessionArr[0]);
            }
            if(CheckGameOver() == 0){
                //player1 패배
                matchGameResult.m_winners.Add(sessionArr[0]);
                matchGameResult.m_losers.Add(sessionArr[1]);
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
            yield return new WaitForSeconds(1f);
            player1TurnImg.SetActive(false);
            players[0].downInformationText.gameObject.SetActive(true);
            players[1].downInformationText.gameObject.SetActive(false);
        }
        if (turn == 0)
        {
            player2TurnImg.SetActive(true);
            yield return new WaitForSeconds(1f);
            player2TurnImg.SetActive(false);
            players[0].downInformationText.gameObject.SetActive(false);
            players[1].downInformationText.gameObject.SetActive(true);
        }
        isActiveTurnImage = false;
    }

    int CheckGameOver()
    {
        // if (nowPlayer.playerMoney < 0) return true;
        // else return false;
        int i;
        for(i = 0; i< GameManager.Instance.players.Length; i++){
            if(players[i].playerMoney < 0)
            return i;
        }
        return i;
    }

    public void SetFloatingText(PlayerManager _player, int _value, bool sign)
    {
        if (_player.playerId == 0)
        {
            // 플로팅 텍스트 프리팹 복제 및 텍스트 정렬
            GameObject prefab = Instantiate(floatingTextPrefab, player1_floatingObject.transform);
            prefab.transform.GetChild(0).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Left;
            prefab.transform.GetChild(1).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Left;

            // 부호에 따라 텍스트 내용과 색상을 바꿔줌
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
            // 플로팅 텍스트 프리팹 복제 및 텍스트 정렬
            GameObject prefab = Instantiate(floatingTextPrefab, player2_floatingObject.transform);
            prefab.transform.GetChild(0).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Right;
            prefab.transform.GetChild(1).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Right;

            // 부호에 따라 텍스트 내용과 색상을 바꿔줌
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
}

class GameOverClass{
    public bool overFlag;
    public SessionId sessionId;

    public GameOverClass(bool _overFlag, SessionId _sessionId){
        overFlag = _overFlag;
        sessionId = _sessionId;
    }
}

