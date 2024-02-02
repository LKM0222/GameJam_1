using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("PlayerInfo")]
    public int playerId;
    public int playerMoney;
    public Text playerMoneyText;
    public float moveSpeed;

    [Header("Moving")]
    public int diceNum;
    public bool isMoving;
    public bool canMove;

    [Header("Tile")]
    [SerializeField] int tileNum; //플레이어가 서있는 칸의 번호
    public Tile nowTile; //현재 서 있는 타일의 정보
    [SerializeField] List<GameObject> tileToGo = new List<GameObject>(); //플레이어가 가야될 타일

    [Header("Card")]
    public List<Card> cards = new List<Card>(); //플레이어가 가진 카드
    public GameObject cardPrefab; // 플레이어의 카드 뒷면 프리팹
    public Transform cardParent; // 플레이어의 카드 갯수만큼 복제하여 넣을 부모 오브젝트

    [Header("Building")]
    public int buildingCount = 0;
    public int groundCount = 0;

    [Header("Buy")]
    [SerializeField] GameObject groundBuyUi;
    [SerializeField] GameObject purchaseUi;

    [Header("InGameData")]
    public bool myTurn;
    public Text downInformationText;
    public GameObject VirtualCamera;
    public PlayerManager againstPlayer;

    [Header("SpecialEffect")]
    public GameObject tpTile; //다음 이동할곳 저장
    public GameObject blackBackground; //tp활성화 시 맵 이외의 주변이 어둡게 변함.
    public GameObject teleportEffect;
    public bool tpFlag;
    public bool isSelectingTeleport;
    public bool isExtortioning;

    [Header("CardFlag")]
    public bool highSpeedFlag;
    public bool invisibleFlag;
    public bool toosiFlag;
    public bool biggerFlag;
    public bool higherDiceFlag;
    public bool lowerDiceFlag;
    public bool exemptionFlag;
    public bool laserFlag;
    public bool isSetTransparent;
    public bool isSetScale;
    public bool isCheckingCard;

    [Header("ScriptReperence")]
    public CardManager theCM;
    GameManager theGM;
    TileManager theTM;
    TurnSignScript theTSI;
    TextManager theText;
    // Start is called before the first frame update
    void Start()
    {
        theTM = FindObjectOfType<TileManager>();
        theGM = FindObjectOfType<GameManager>();
        theTSI = FindObjectOfType<TurnSignScript>();
        theText = FindObjectOfType<TextManager>();
    }

    // Update is called once per frame
    void Update()
    {
        playerMoneyText.text = playerMoney.ToString();

        if (canMove)
        {
            StartCoroutine(ReadyToMovePlayerCoroutine());
        }

        if (toosiFlag && myTurn)
        {
            toosiFlag = false;
            StartCoroutine(theCM.PenetrateCoroutine());
        }

        if (laserFlag && myTurn)
        {
            laserFlag = false;
            StartCoroutine(theCM.LaserBeamCoroutine());
        }

        if (tpFlag && myTurn)
        {
            StartCoroutine(TeleportCoroutine(playerId));
        }
    }

    // 실질적인 이동 전, 카드 사전 작업 및 타일 추가 등을 수행
    IEnumerator ReadyToMovePlayerCoroutine()
    {
        canMove = false;

        // 주사위 수만큼 tileToGo 리스트에 추가
        for (int i = 0; i < diceNum; i++)
        {
            // 이동할 타일의 번호가 전체 타일의 길이를 넘어간다면 전체 타일 길이만큼 빼줌
            if (tileNum + i >= theTM.tiles.Length)
            {
                tileToGo.Add(theTM.tiles[tileNum + i - theTM.tiles.Length].gameObject);
            }
            else
            {
                tileToGo.Add(theTM.tiles[tileNum + i].gameObject);
            }
        }

        theTSI.cursorPos = 3;

        VirtualCamera.SetActive(true);
        yield return new WaitForSeconds(1f);

        StartCoroutine(CheckUsedCardCoroutine());
        yield return new WaitUntil(() => !isCheckingCard);

        while (tileToGo.Count != 0)
        {
            // Player 실제 이동 코루틴 실행
            isMoving = true;
            Vector3 targetPos = tileToGo[0].transform.Find("Pos").transform.position;
            StartCoroutine(MovingPlayerCoroutine(targetPos));
            yield return new WaitUntil(() => isMoving == false);

            nowTile = tileToGo[0].GetComponent<Tile>();
            tileToGo.RemoveAt(0);
            CheckPassTile();
        }
        StartCoroutine(EndMovePlayerCoroutine());
    }

    public IEnumerator CheckUsedCardCoroutine()
    {
        isCheckingCard = true;
        if (theGM.nowPlayer.highSpeedFlag)
        {
            theCM.HighSpeedMove();
        }

        if (theGM.nowPlayer.invisibleFlag)
        {
            StartCoroutine(SetPlayerTransparency("Invisible"));
            yield return new WaitUntil(() => !isSetTransparent);
        }

        if (theGM.nowPlayer.biggerFlag)
        {
            StartCoroutine(SetPlayerScale("Larger"));
            yield return new WaitUntil(() => !isSetScale);
        }
        isCheckingCard = false;
    }

    // 플레이어 이동시 지나치는 타일 체크
    public void CheckPassTile()
    {
        // 시작지점을 지나쳐간다면 월급 지급
        if (nowTile.transform.name == "0")
        {
            playerMoney += 200;
        }

        // 투명도둑을 사용하고 나와 상대방이 겹쳐질때, 상대방의 카드가 있을 때 투명도둑 효과 발동
        if (invisibleFlag)
        {
            if (againstPlayer.nowTile == nowTile && againstPlayer.cards.Count != 0)
            {
                theCM.InvisibleThief();
            }
        }
    }

    IEnumerator MovingPlayerCoroutine(Vector3 target)
    {

        this.gameObject.GetComponent<Animator>().SetInteger("Dir", nowTile.dir);
        this.gameObject.GetComponent<Animator>().SetBool("WalkFlag", true);

        if (theGM.nowPlayer.highSpeedFlag)
        {
            this.gameObject.GetComponent<Animator>().SetBool("FlyFlag", true);
            theCM.highMoveParticle.gameObject.transform.SetParent(theGM.nowPlayer.transform.GetChild(nowTile.dir));
            theCM.highMoveParticle.gameObject.transform.localPosition = new Vector3(0f, 0f, 1f);
        }

        while (isMoving)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, target, Time.deltaTime * moveSpeed);
            yield return new WaitForEndOfFrame();
            if (this.transform.position == target)
            {
                isMoving = false;
            }
        }
        yield return null;
    }

    IEnumerator EndMovePlayerCoroutine()
    {
        this.gameObject.GetComponent<Animator>().SetBool("FlyFlag", false);
        this.gameObject.GetComponent<Animator>().SetBool("WalkFlag", false);

        // 투명도둑을 사용했었다면 알파값 원상복구
        if (theGM.nowPlayer.invisibleFlag)
        {
            theCM.EndInvisibleThief();
            yield return new WaitUntil(() => !isSetTransparent);
        }

        // 고속이동이 끝났다면 스피드를 원상복구 시키고 플래그를 비활성화시킴
        if (theGM.nowPlayer.highSpeedFlag)
        {
            theCM.EndHighSpeedMove();
        }

        // 플레이어가 거대화 스킬을 사용하고 이동이 끝났다면 효과 발동
        if (theGM.nowPlayer.biggerFlag)
        {
            StartCoroutine(theCM.BiggerCoroutine());
            yield return new WaitUntil(() => theCM.biggerComplete);
            theCM.biggerComplete = false;

            StartCoroutine(SetPlayerScale("Smaller"));
            yield return new WaitUntil(() => !isSetScale);
        }

        tileNum = int.Parse(nowTile.gameObject.name);

        VirtualCamera.SetActive(false);

        StartCoroutine(CheckArriveTile());
    }

    // 도착한 땅의 타일을 체크하여 상호작용하는 기능
    public IEnumerator CheckArriveTile()
    {
        // 이동이 끝난 후, 일반 타일에 도착했다면
        if (!nowTile.specialTile)
        {
            // 일반 타일 중 자신이 구매한 타일이라면
            if (nowTile.ownPlayer == playerId)
            {
                // 건물이 없으면 건물 구매 UI 활성화
                if (nowTile.building == null)
                {
                    purchaseUi.SetActive(true);
                    theGM.UIFlag = true;
                }
                // 건물이 있으면 건물 방문 효과 활성화
                else
                {
                    switch (nowTile.building.type)
                    {
                        // 농장
                        case 0:
                            playerMoney += 200;
                            print(playerMoney);
                            break;
                        // 제단
                        case 1:
                            nowTile.price *= 2;
                            print(nowTile.price);
                            break;
                        // 특별상점
                        case 2:
                            for (int i = 0; i < 2; i++)
                            {
                                if (cards.Count < 8)
                                {
                                    StartCoroutine(theCM.CardProvideCoroutine());
                                    yield return new WaitUntil(() => theCM.isGetCard);
                                }
                            }
                            break;
                        // 랜드마크
                        case 3:
                            break;
                    }
                    theGM.NextTurnFunc();
                }
            }
            // 일반 타일 중 아무도 구매하지 않은 타일이라면 땅 구매 UI 활성화
            else if (nowTile.ownPlayer == -1)
            {
                if (playerMoney >= 50)
                {
                    groundBuyUi.SetActive(true);
                    theGM.UIFlag = true;
                }
                else
                {
                    theGM.NextTurnFunc();
                }
            }
            // 일반 타일 중 상대방이 구매한 타일이라면
            else
            {
                // 통행료 카드가 없는 경우 통행료 징수
                if (!exemptionFlag)
                {
                    playerMoney -= nowTile.price;
                    againstPlayer.playerMoney += nowTile.price;
                }
                // 통행료 면제 카드가 있다면 통행료 징수를 하지 않음
                else
                {
                    theCM.TollExemption();
                }
                theGM.NextTurnFunc();
            }
        }
        // 일반 타일이 아니라, 특수 타일일 경우
        else
        {
            switch (nowTile.specialTileType)
            {
                // 양계장
                case 0:
                    for (int i = 0; i < theTM.tiles.Length; i++)
                    {
                        if (theTM.tiles[i].ownPlayer == playerId && theTM.tiles[i].building.type == 0) playerMoney += 100;
                    }
                    break;

                // 카드지급
                case 1:
                    StartCoroutine(theCM.CardProvideCoroutine());
                    yield return new WaitUntil(() => theCM.isGetCard);
                    break;

                // 텔레포트
                case 2:
                    StartCoroutine(TeleportSetCoroutine());
                    yield return new WaitUntil(() => tpFlag);
                    break;

                // 올림픽
                case 3:
                    for (int i = 0; i < theTM.tiles.Length; i++)
                    {
                        if (theTM.tiles[i].ownPlayer == playerId) theTM.tiles[i].price *= 2;
                    }
                    break;

                // 건물강탈
                case 4:
                    blackBackground.SetActive(true);
                    isExtortioning = true;

                    for (int i = 0; i < theTM.tiles.Length; i++)
                    {
                        if (theTM.tiles[i].ownPlayer == againstPlayer.playerId) theTM.tiles[i].canTileSelect = true;
                    }

                    yield return new WaitUntil(() => theGM.seletedTile != null);

                    isExtortioning = false;
                    for (int i = 0; i < theTM.tiles.Length; i++)
                    {
                        theTM.tiles[i].canTileSelect = false;
                    }

                    blackBackground.SetActive(false);
                    theGM.seletedTile.GetComponent<Tile>().ownPlayer = playerId;
                    theGM.seletedTile = null;

                    break;
            }
            theGM.NextTurnFunc();
        }
    }

    IEnumerator TeleportSetCoroutine()
    {
        blackBackground.SetActive(true);
        isSelectingTeleport = true;
        for (int i = 0; i < theTM.tiles.Length; i++)
        {
            if (i != 6) theTM.tiles[i].canTileSelect = true;
        }

        yield return new WaitUntil(() => theGM.seletedTile != null);

        isSelectingTeleport = false;
        for (int i = 0; i < theTM.tiles.Length; i++)
        {
            theTM.tiles[i].canTileSelect = false;
        }

        blackBackground.SetActive(false);
        tpTile = theGM.seletedTile;
        theGM.seletedTile = null;
        myTurn = false;
        tpFlag = true;
        // theGM.NextTurnFunc();
    }

    IEnumerator TeleportCoroutine(int _playerId)
    {
        tpFlag = false;
        myTurn = false;

        // 턴을 알리는 텍스트가 사라질때까지 대기
        yield return new WaitUntil(() => !theGM.isActiveTurnImage);


        theGM.players[_playerId].downInformationText.gameObject.SetActive(false);
        VirtualCamera.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        Color alpha = new Color(1, 1, 1, 0);
        this.GetComponent<SpriteRenderer>().color = alpha;

        if (_playerId == 0)
        {
            theGM.player1TeleportEffect.transform.position = theTM.tiles[5].transform.GetChild(0).position;
            theGM.player1TeleportEffect.SetActive(true);
            yield return new WaitForSeconds(3f);
            theGM.player1TeleportEffect.SetActive(false);
        }
        else if (_playerId == 1)
        {
            theGM.player2TeleportEffect.transform.position = theTM.tiles[5].transform.GetChild(0).position;
            theGM.player2TeleportEffect.SetActive(true);
            yield return new WaitForSeconds(3f);
            theGM.player2TeleportEffect.SetActive(false);
        }

        // player가 서있는 타일의 정보를 갱신함
        this.tileNum = int.Parse(tpTile.gameObject.name);
        this.transform.position = tpTile.transform.Find("Pos").position;
        nowTile = tpTile.GetComponent<Tile>();
        this.GetComponent<Animator>().SetInteger("Dir", nowTile.dir);
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            alpha.a += 0.1f;
            this.GetComponent<SpriteRenderer>().color = alpha;
            yield return new WaitForSeconds(0.1f);
            if (alpha.a >= 1f) break;
        }

        VirtualCamera.SetActive(false);
        StartCoroutine(CheckArriveTile());
    }

    public IEnumerator SetPlayerTransparency(string _parameter)
    {
        isSetTransparent = true;
        if (_parameter == "Invisible")
        {
            Color alpha = new(1, 1, 1, 1);
            while (true)
            {
                alpha.a -= 0.1f;
                theGM.nowPlayer.GetComponent<SpriteRenderer>().color = alpha;
                yield return new WaitForSeconds(0.1f);

                if (alpha.a <= 0.5f)
                    break;
            }
        }
        else if (_parameter == "Visible")
        {
            Color alpha = new(1, 1, 1, 0.5f);
            while (true)
            {
                alpha.a += 0.1f;
                theGM.nowPlayer.GetComponent<SpriteRenderer>().color = alpha;
                yield return new WaitForSeconds(0.1f);

                if (alpha.a >= 1f)
                    break;
            }
        }
        isSetTransparent = false;
    }

    public IEnumerator SetPlayerScale(string _parameter)
    {
        isSetScale = true;
        if (_parameter == "Larger")
        {
            Vector3 scale = new Vector3(1.5f, 1.5f, 0);
            while (true)
            {
                scale += new Vector3(0.1f, 0.1f, 0);
                theGM.nowPlayer.gameObject.transform.localScale = scale;
                yield return new WaitForSeconds(0.1f);

                if (scale.x >= 2f)
                {
                    break;
                }
            }
        }
        else if (_parameter == "Smaller")
        {
            Vector3 scale = new Vector3(2f, 2f, 0);
            while (true)
            {
                scale -= new Vector3(0.1f, 0.1f, 0);
                theGM.nowPlayer.gameObject.transform.localScale = scale;
                yield return new WaitForSeconds(0.1f);

                if (scale.x <= 1.5f)
                {
                    break;
                }
            }
        }
        isSetScale = false;
    }
}
