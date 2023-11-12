using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    //몇번째 플레이어인지 정보
    public int playerId;
    //플레이어의 자금
    public int playerMoney;
    //플레이어 자금 플로팅
    public Text playerMoneyText;
    // 상대 플레이어
    public PlayerManager againstPlayer;

    //플레이어가 밟고 지나가야 될 타일(최대 12칸.)
    [SerializeField] List<GameObject> tileToGo = new List<GameObject>();
    //현재 서 있는 타일의 정보
    public Tile nowTile;
    //플레이어가 서있는 칸의 번호
    [SerializeField] int tileNum;
    // 상대 플레이어가 가지고 있는 타일
    public List<GameObject> againstPlayer_Tile = new List<GameObject>();


    //주사위의 눈금
    public int diceNum;
    // 주사위 굴렸는지 플래그
    public bool diceFlag;
    //코루틴 반복을 방지하는 플래그(myTurn이랑 기능이 똑같은거 아닌가?)
    public bool movingFlag;
    // 각 플레이어마다 부여되는 턴 플래그
    public bool myTurn;


    //플레이어가 가진 카드 리스트
    public List<Card> cards = new List<Card>();
    // 카드 프리팹
    public GameObject cardPrefab;
    // 카드가 들어갈 부모 오브젝트 위치
    public Transform cardParent;

    TurnSignScript theTSI;
    GameManager theGM;
    TileManager theTM; // 플레이어가 가야될 타일 정보 받아오기 위해 추가
    public GameObject VirtualCamera;

    // '아래로 당기시오!!' 텍스트
    public Text downInformationText;



    [Header("Building")]
    public int buildingCount = 0;
    public int groundCount = 0;

    [Header("Buy")]
    [SerializeField] GameObject groundBuyUi;
    [SerializeField] GameObject purchaseUi;

    [Header("Effect")]
    public bool tpFlag;
    bool tpSelectFlag;
    public GameObject tpBack; // 텔레포트 사용 시 적용되는 어두운 Background

    public GameObject tpTile; // 다음 이동할곳 저장
    public bool highSpeedFlag;
    public bool invisibleFlag; // 투명화
    public bool toosiFlag; // 투시
    public bool biggerFlag; // 거대화

    // Start is called before the first frame update
    void Start()
    {
        theTM = FindObjectOfType<TileManager>();
        theGM = FindObjectOfType<GameManager>();
        theTSI = FindObjectOfType<TurnSignScript>();
    }

    // Update is called once per frame
    void Update()
    {
        playerMoneyText.text = playerMoney.ToString();
        // 자신의 턴일때
        if (myTurn)
        {
            // '아래로 당기시오' 텍스트를 띄워줌
            downInformationText.gameObject.SetActive(true);

            for (int i = 0; i < againstPlayer.cards.Count; i++)
            {   // cardPrefab의 Cover(뒷면)을 활성화해서 상대방의 카드 내용을 가려줌
                againstPlayer.cardParent.GetChild(i).GetChild(0).gameObject.SetActive(true);
            }

            for (int i = 0; i < cards.Count; i++)
            {   // cardPrefab의 Cover(뒷면)을 비활성화해서 본인의 카드 내용 보여줌
                cardParent.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }
        // 자신의 턴이 아닐때
        else
        {
            // '아래로 당기시오' 텍스트를 가려줌
            downInformationText.gameObject.SetActive(false);
            // for(int i=0;i<cards.Count; i++){
            //     cardParent.GetChild(i).GetChild(0).gameObject.SetActive(false);
            // }
        }

        // 
        if (movingFlag)
        {
            StartCoroutine("DiceCoroutine");
        }

        if (toosiFlag)
        {
            StartCoroutine("TooSiCoroutine");
        }

        if (tpFlag && myTurn)
        {
            this.tileNum = int.Parse(tpTile.gameObject.name);
            this.tileToGo.Add(tpTile);
            this.transform.position = tileToGo[0].transform.TransformDirection(tileToGo[0].transform.Find("Pos").transform.position);
            this.tileToGo.RemoveAt(0);
            theGM.turnCount += 1;
            theGM.nextTurn = true;
            tpFlag = false;
        }
    }

    IEnumerator DiceCoroutine()
    {
        movingFlag = false;

        // 투명화 카드를 썼다면
        if (invisibleFlag && myTurn)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        }

        // 거대화 카드를 썼다면
        if (biggerFlag && myTurn)
        {
            this.gameObject.transform.localScale = new Vector3(2f, 2f, 0);
        }

        //주사위를 굴렸다면
        if (diceFlag)
        {
            //고속이동을 활성화 했다면 두배로 이동
            if (highSpeedFlag && myTurn)
            {
                // 플레이어가 밟아서 이동해야 하는 타일들을 tileToGo에 추가해줌
                for (int i = 0; i < diceNum * 2; i++)
                {
                    // 전체 타일의 길이를 넘어서서 한바퀴를 돌았다면
                    if (tileNum + i >= theTM.tiles.Length)
                    {
                        // 전체 타일의 길이만큼 뺀 다음에 tileToGo에 추가
                        tileToGo.Add(theTM.tiles[tileNum + i - theTM.tiles.Length].gameObject);
                    }
                    else
                    {
                        tileToGo.Add(theTM.tiles[tileNum + i].gameObject);
                    }

                }
                highSpeedFlag = false;
            }
            else
            {
                // 플레이어가 밟아서 이동해야 하는 타일들을 tileToGo에 추가해줌
                for (int i = 0; i < diceNum; i++)
                {
                    // 전체 타일의 길이를 넘어서서 한바퀴를 돌았다면
                    if (tileNum + i >= theTM.tiles.Length)
                    {
                        // 전체 타일의 길이만큼 뺀 다음에 tileToGo에 추가
                        tileToGo.Add(theTM.tiles[tileNum + i - theTM.tiles.Length].gameObject);
                    }
                    else
                    {
                        tileToGo.Add(theTM.tiles[tileNum + i].gameObject);
                    }

                }
            }

            // 가상카메라 활성화
            VirtualCamera.SetActive(true);

            // 주사위 굴리는거 기다려야됨
            yield return new WaitForSeconds(1f);
            print("주사위 완료");

            //플레이어 이동
            theTSI.cursorPos = 3;
            for (; tileToGo.Count != 0;)
            {
<<<<<<< Updated upstream
                if (tileToGo[0].transform.name == "Tile (23)")
=======
                // 양계장에 도착했다면
                if (tileToGo[0].transform.name == "0")
>>>>>>> Stashed changes
                {
                    this.transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 0f, 1f);
                    // 완주 보상을 100알 지급
                    playerMoney += 100;
                }
                else
                {
                    this.transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                }

                // player를 tile의 로컬좌표를 기준으로 이동시킴 (애니메이션 필요)
                this.transform.position = tileToGo[0].transform.TransformDirection(tileToGo[0].transform.Find("Pos").transform.position);
                // this.transform.position = Vector3.MoveTowards(gameObject.transform.position, tileToGo)
<<<<<<< Updated upstream
                nowTile = tileToGo[0].GetComponent<Tile>(); //현재 타일
                this.gameObject.GetComponent<Animator>().SetInteger("Dir",nowTile.dir);
                if(invisibleFlag){//invisible플래그가 활성화되어있고
                    if(againstPlayer.tileNum == tileNum){ //내가 상대방과 같은 타일을 지나간다면
                        GameObject dCard = againstPlayer.cardParent.GetChild(UnityEngine.Random.Range(0,againstPlayer.cardParent.childCount)).gameObject;
                        dCard.transform.parent = cardParent;
                        //카드 한장을 훔침.
                    }
=======
>>>>>>> Stashed changes

                // 현재 player가 이동한 타일
                nowTile = tileToGo[0].GetComponent<Tile>();
                // 타일의 방향에 맞춰서 플레이어를 회전시키는 애니메이션 적용
                this.gameObject.GetComponent<Animator>().SetInteger("Dir", nowTile.dir);
                AudioManager.instance.Play("moveSound");

                // 투명화 스킬을 썼다면
                if (invisibleFlag)
                {
                    // 상대방이 서있는 타일과 현재 자신이 서있는 타일이 겹칠때
                    if (againstPlayer.tileNum == tileNum)
                    {
                        // 상대방의 카드를 랜덤하게 하나 훔치고, 자신의 카드에 추가함
                        // (상대방 카드 목록에서 삭제, 내 카드 목록 업데이트 필요)
                        GameObject dCard = againstPlayer.cardParent.GetChild(UnityEngine.Random.Range(0, againstPlayer.cardParent.childCount)).gameObject;
                        dCard.transform.parent = cardParent;
                    }
                }
                //애니메이션 나오는 시간동안 기다린 뒤
                yield return new WaitForSeconds(0.5f);

                //리스트에서 첫번째 요소 삭제
                tileToGo.RemoveAt(0);
            }

            // 현재 서있는 위치 업데이트, 한바퀴를 돌았을 경우 전체 타일 갯수만큼 빼줌
            if (tileNum + diceNum > theTM.tiles.Length)
            {
                tileNum += diceNum;
                tileNum -= theTM.tiles.Length;
            }
            else
            {
                tileNum += diceNum;
            }

            // 이동과 주사위 굴리기 모두 마쳤으니 false로 변경
            diceFlag = false;
            movingFlag = false;

            // 현재 서있는 땅이 일반 땅이라면(건물을 지을 수 있는 땅)
            if (!nowTile.specialTile)
            {
                // 자신의 땅이라면
                if (nowTile.ownPlayer == playerId)
                {
                    // 건물이 없으면 건물 구매 UI 활성화
                    if (nowTile.building == null)
                    {
                        purchaseUi.SetActive(true);
                        //카드 선택 방지를 위한 UI활성화 플래그 활성화
                        theGM.UIFlag = true;
                    }
                    // 건물이 있으면 건물 효과 활성화
                    else
                    {
                        //효과를 활성화
                    }
                }
                // 주인이 없는 땅이라면
                else if (nowTile.ownPlayer == -1)
                {
                    //땅 구매 UI를 활성화
                    groundBuyUi.SetActive(true);
                    //카드 선택 방지를 위한 UI활성화 플래그 활성화
                    theGM.UIFlag = true;
                }
                // 상대방의 땅이라면
                else
                {
                    // 거대화 카드 사용시 상대방의 건물 파괴
                    if (biggerFlag)
                    {
                        nowTile.ownPlayer = -1;
                        nowTile.building = null;
                        this.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                        biggerFlag = false;
                    }

                    // 거대화 카드 사용 없이 방문했다면
                    else
                    {
                        // 상대방의 건물이 있다면
                        if (nowTile.building != null)
                        {
                            // 어떤 건물인지 체크
                            switch (nowTile.building.type)
                            {
                                // 농장
                                case 0:
                                    break;
                                // 제단
                                case 1:
                                    break;
                                // 상점
                                case 2:
                                    break;
                                // 랜드마크
                                case 3:
                                    if (nowTile.building.visitCount < 5)
                                        nowTile.building.visitCount += 1;
                                    playerMoney -= nowTile.building.visitCount * 100;
                                    againstPlayer.playerMoney += nowTile.building.visitCount * 100;
                                    break;
                                default:
                                    playerMoney -= 100;
                                    againstPlayer.playerMoney += 100;
                                    break;
                            }

                        }
                        // 없다면 기본 통행료 50만큼만 차감
                        else
                        {
                            playerMoney -= 50;
                        }
                    }

                    theGM.turnCount += 1;//턴넘김
                    theGM.nextTurn = true;

                }
            }

            //특수 타일이라면
            else
            {
                print("specialTile" + nowTile.specialTileType);

                // 특수 타일 타입 체크
                switch (nowTile.specialTileType)
                {
                    // 양계장 타일
                    case 0:
                        print("card Tile");
                        break;
                    // 카드 지급 타일
                    case 1:
                        print("card Tile");
                        // 소지 카드 갯수가 7개 이하라면 
                        if (cardParent.childCount < 8)
                        {
                            // 랜덤한 카드를 하나 가져와서 cardPrefab에 적용시킨 후 복제해서 player의 card 리스트에 넣어줌
                            Card newCard = theGM.cards[UnityEngine.Random.Range(0, theGM.cards.Length)];
                            print(newCard.card_name);
                            cardPrefab.GetComponent<CardManager>().cardInfo = newCard; //theGM.cards[UnityEngine.Random.Range(0,theGM.cards.Length)]; //카드 속성 랜덤으로 설정해주고
                            cardPrefab.GetComponent<SpriteRenderer>().sprite = newCard.cardImg; //cardPrefab.GetComponent<CardManager>().cardInfo.cardImg; //카드 이미지 변경(여기서 오류 한번 생길듯)
                            var _card = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity, cardParent);//카드 프리펩 생성해주고
                            _card.transform.localPosition = new Vector3(0f, 0f, 0f);
                            cards.Add(newCard); //플레이어 리스트에 카드 추가
                        }
                        break;
                    // 텔레포트 타일
                    case 2:
                        //teleportFlag활성화
                        tpSelectFlag = true;
                        StartCoroutine("TeleportCoroutine");
                        break;
                    // 세금 징수 타일
                    case 3:
                        //땅 갯수 *5 + 건물 갯수 * 10
                        playerMoney -= (groundCount * 5) + (buildingCount * 10);
                        break;
                    // 강탈 타일
                    case 4:
                        // 상대방의 카드를 랜덤하게 하나 훔치고, 자신의 카드에 추가함
                        // (상대방 카드 목록에서 삭제, 내 카드 목록 업데이트 필요)
                        GameObject dCard = againstPlayer.cardParent.GetChild(UnityEngine.Random.Range(0, againstPlayer.cardParent.childCount)).gameObject;
                        dCard.transform.parent = cardParent;
                        break;
                }

                // 텔레포트 타일에 도착하지 않았다면 턴을 종료
                // 텔레포트 타일에 도착했다면 이 부분은 넘어감
                if (!tpSelectFlag)
                { //tp중일땐 일단 타일이 선택되기 전까지는 기다려야하기 때문에 탈출할 수 없음...
                    //특수 행동 후 턴을 넘김
                    theGM.turnCount += 1;
                    theGM.nextTurn = true;
                    invisibleFlag = false;
                    this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                }
            }
            VirtualCamera.SetActive(false);
        }

        else
        { //tpFlag일 경우 텔레포트함.

        }
    }

    // 투시 Coroutine
    IEnumerator TooSiCoroutine()
    {
        // 상대방의 카드들의 Cover를 비활성화 시켜서 카드 내용을 보여줌
        for (int i = 0; i < againstPlayer.cards.Count; i++)
        {
            againstPlayer.cardParent.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(3f);

        // 3초 대기 이후 Cover를 다시 씌워서 카드 내용을 가림
        for (int i = 0; i < againstPlayer.cards.Count; i++)
        {
            againstPlayer.cardParent.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }
    }

    // 순간이동 Coroutine
    IEnumerator TeleportCoroutine()
    {
        // 텔레포트 할 타일 선택 시, 검게 변하는 Background 이미지 활성화
        tpBack.SetActive(true);

        // 모든 타일을 클릭 가능하게 변경
        for (int i = 0; i < theTM.tiles.Length; i++)
        {
            theTM.tiles[i].cardActive = true;
        }

        // 타일을 클릭하기 전까지 대기
        yield return new WaitUntil(() => theGM.tpTile != null);

        // 모든 타일을 클릭 불가능하게 변경
        for (int i = 0; i < theTM.tiles.Length; i++)
        {
            theTM.tiles[i].cardActive = false;
        }

        // tpTile에 GameManager의 tpTile을 대입시키고 GameManager의 tpTile은 null로 초기화
        // GM.tpTile을 null로 초기화 시켜주지 않으면, 상대방이 뒤따라 도착했을때 위치를 정하지 못하고 같은 곳으로 이동하게됨
        tpTile = theGM.tpTile;
        theGM.tpTile = null;

        // 텔레포트 Background를 꺼주고 tpFlag를 true로 바꿔서 다음 턴에 텔레포트 가능하도록 함
        tpBack.SetActive(false);
        tpFlag = true;

        // 텔레포트할 위치를 정했으면 턴 종료
        myTurn = false;
        theGM.turnCount += 1;
        theGM.nextTurn = true;
    }
}
