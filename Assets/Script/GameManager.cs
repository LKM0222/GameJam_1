using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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

    // 현재 진행된 턴 수
    public int turnCount;

    // 턴을 종료하고 턴을 넘기는 체크 플래그
    public bool nextTurn = false;

    //UI가 활성화 되어있는지 확인하는 플래그
    public bool UIFlag = false;

    // 카드를 담을 GameObject
    public GameObject cardsObj;
    // 카드 Prefab
    public GameObject cardPrefab;

    TurnSignScript theTSI;

    public List<GameObject> buyedTiles = new List<GameObject>();
    public GameObject clickedTile;

    public GameObject tpTile; //텔레포트 활성화 시 다음턴에 움질일 위치 정함.

    // 투시를 사용하면 카드들을 보여줄 오브젝트
    public GameObject showCardObject;
    // 카드 이미지만 담은 프리팹
    public GameObject onlyCardImg;



    // Start is called before the first frame update
    void Start()
    {
        theTSI = FindObjectOfType<TurnSignScript>();
        //AudioManager.instance.Play("mainSound");
    }

    // Update is called once per frame
    void Update()
    {
        // 턴을 종료하고 상대 턴으로 넘어갔다면
        if (nextTurn)
        {
            //나머지가 1이면 1플레이어, 0이면 2플레이어
            if (turnCount % 2 == 1)
            {
                // 각각의 플레이어의 myTurn을 바꿔주고 nowPlayer를 현재 턴을 가진 플레이어로 바꿈
                players[0].myTurn = true;
                players[1].myTurn = false;
                nowPlayer = players[0];
                theTSI.cursorPos = 1;
                CardListUpdate();
            }
            else
            {
                // 각각의 플레이어의 myTurn을 바꿔주고 nowPlayer를 현재 턴을 가진 플레이어로 바꿈
                players[1].myTurn = true;
                players[0].myTurn = false;
                nowPlayer = players[1];
                theTSI.cursorPos = 1;
                CardListUpdate();
            }
            nextTurn = false;
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
                // Destroy로 왜 전부 지우는거지?
                Destroy(cardsObj.transform.GetChild(i).gameObject);
            }
        }

        // 삭제 이후 플레이어가 가진 카드 갯수만큼 다시 복제
        if (nowPlayer.cards.Count > 0)
        {
            for (int i = 0; i < nowPlayer.cards.Count; i++)
            {
                // 카드 프리팹을 복제한 이후, 위치를 맞춰주고, 카드의 정보와 이미지를 바꿔준다.
                var _card = Instantiate(cardPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, cardsObj.transform);
                _card.transform.localPosition = new Vector3(0f, 0f, 0f);
                _card.GetComponent<CardManager>().cardInfo = nowPlayer.cards[i];
                _card.GetComponent<SpriteRenderer>().sprite = nowPlayer.cards[i].cardImg;
            }
        }
    }

    public void NextTurnFunc()
    { //턴 종료일때 호출하는 메서드. 공통으로 들어가는 요소만 넣었음.
        turnCount += 1;//턴넘김
        nextTurn = true;
    }
}
