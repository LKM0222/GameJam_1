using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class PlayerManager : MonoBehaviour
{
    [Header("PlayerInfo")]
    public int playerId;
    public int playerMoney;
    public Text playerMoneyText;
    public float moveSpeed = 4f;
    // 자신이 몇 번째 플레이어인지 나타내는 오브젝트
    public GameObject myTurnImg;


    [Space(10), Header("Moving")]
    public int diceNum;
    public bool isMoving;
    public bool canMove;
    public bool finishMoving;
    float movingWaitTime;
    // 이동할 타일 위치
    [SerializeField] Vector3 targetPos;


    [Space(10), Header("Tile")]
    // 타일의 번호
    [SerializeField] int tileNum;
    // 현재 서있는 타일
    public Tile nowTile;
    // 지나쳐야 할 타일 리스트
    [SerializeField] List<GameObject> tileToGo = new List<GameObject>();
    // 마지막 도착할 타일
    int lastTile = 0;


    [Space(10), Header("Card")]
    // 플레이어의 카드
    public List<Card> cards = new List<Card>();
    // 카드 뒷면 프리팹
    public GameObject cardPrefab;
    // 보유한 카드의 수를 복제할 위치
    public Transform cardParent;


    [Space(10), Header("Buy")]
    [SerializeField] GameObject groundBuyUi;
    [SerializeField] GameObject purchaseUi;


    [Space(10), Header("InGameData")]
    public bool myTurn;
    public PlayerManager againstPlayer;
    public Text downInformationText;
    public GameObject VirtualCamera;


    [Space(10), Header("SpecialEffect")]
    // 텔레포트 할 타일
    public GameObject tpTile;
    public GameObject blackBackground;
    public bool tpFlag;
    public bool isSelectingTeleport;
    public bool isExtortioning;


    [Space(10), Header("CardFlag")]
    public bool highSpeedFlag;
    public bool invisibleFlag;
    public bool biggerFlag;
    public bool higherDiceFlag;
    public bool lowerDiceFlag;
    public bool exemptionFlag;
    public bool laserFlag;
    public bool isSetTransparent;
    public bool isSetScale;
    public bool isCheckingCard;


    [Space(10), Header("ScriptReperence")]
    public CardManager theCM;
    TurnSignScript theTSI;

    void Start()
    {
        theTSI = FindObjectOfType<TurnSignScript>();
    }

    void Update()
    {
        playerMoneyText.text = playerMoney.ToString();

        if (canMove)
        {
            StartCoroutine(ReadyToMovePlayerCoroutine());
        }

        if (laserFlag && myTurn)
        {
            laserFlag = false;
            StartCoroutine(theCM.SelectLaserCoroutine());
        }

        if (tpFlag && myTurn)
        {
            StartCoroutine(TeleportCoroutine(playerId));
            if (GameManager.Instance.turnCount % 2 != GameManager.Instance.turnIndex) myTurn = false;
        }
    }

    // 실질적인 이동 전, 카드 사전 작업 및 타일 추가 등을 수행
    IEnumerator ReadyToMovePlayerCoroutine()
    {
        canMove = false;

        theTSI.cursorPos = 3;

        VirtualCamera.SetActive(true);
        yield return new WaitForSeconds(1f);

        StartCoroutine(CheckUsedCardCoroutine());
        yield return new WaitUntil(() => !isCheckingCard);

        // 주사위 수만큼 밟아야 할 타일을 tileToGo 리스트에 추가
        for (int i = 0; i < diceNum; i++)
        {
            tileToGo.Add(TileManager.Instance.tiles[(tileNum + i) % TileManager.Instance.tiles.Length].gameObject);
        }

        // 도착 타일을 계산해서 이펙트를 띄워줌
        lastTile = (tileNum + diceNum) % TileManager.Instance.tiles.Length;
        if (lastTile == 0)
        {
            lastTile = TileManager.Instance.tiles.Length;
        }
        TileManager.Instance.tiles[lastTile - 1].gameObject.transform.Find("TileLine").gameObject.SetActive(true);

        movingWaitTime = 0f;

        finishMoving = false;

        while (tileToGo.Count != 0)
        {
            isMoving = true;
            targetPos = tileToGo[0].transform.Find("Pos").transform.position;

            // 플레이어 이동 코루틴 실행
            StartCoroutine(MovingPlayerCoroutine(targetPos));
            yield return new WaitUntil(() => isMoving == false);

            nowTile = tileToGo[0].GetComponent<Tile>();
            tileToGo.RemoveAt(0);

            CheckPassTile();
        }
        movingWaitTime = 0f;

        StartCoroutine(EndMovePlayerCoroutine());
    }

    // 이동 전 카드 사용 효과 발동
    public IEnumerator CheckUsedCardCoroutine()
    {
        isCheckingCard = true;
        if (GameManager.Instance.nowPlayer.highSpeedFlag)
        {
            theCM.HighSpeedMove();
            AudioManager.Instance.Play("HighSpeedMove_Sound");
        }

        if (GameManager.Instance.nowPlayer.invisibleFlag)
        {
            StartCoroutine(SetPlayerTransparency("Invisible"));
            yield return new WaitUntil(() => !isSetTransparent);
        }

        if (GameManager.Instance.nowPlayer.biggerFlag)
        {
            StartCoroutine(SetPlayerScale("Larger"));
            yield return new WaitUntil(() => !isSetScale);
        }
        isCheckingCard = false;
    }

    // 플레이어 이동시 지나치는 타일 체크
    public void CheckPassTile()
    {
        // 양계장 도착 시 월급 지급
        if (nowTile.transform.name == "0")
        {
            AudioManager.Instance.Play("Salary_Sound");
            playerMoney += 200;
            GameManager.Instance.SetFloatingText(GameManager.Instance.nowPlayer, 200, true);
        }

        // 투명도둑 사용 && 이동할 타일이 남음 && 상대방과 같은 타일 && 상대방의 카드가 있음
        if (invisibleFlag && tileToGo.Count != 0 && againstPlayer.nowTile == nowTile && againstPlayer.cards.Count != 0)
        {
            StartCoroutine(theCM.InvisibleThief());
        }
    }

    // 플레이어의 실질적 이동 코루틴
    IEnumerator MovingPlayerCoroutine(Vector3 target)
    {

        this.gameObject.GetComponent<Animator>().SetInteger("Dir", nowTile.dir);
        this.gameObject.GetComponent<Animator>().SetBool("WalkFlag", true);

        if (GameManager.Instance.nowPlayer.highSpeedFlag)
        {
            this.gameObject.GetComponent<Animator>().SetBool("FlyFlag", true);
            theCM.highMoveParticle.gameObject.transform.SetParent(GameManager.Instance.nowPlayer.transform.GetChild(nowTile.dir));
            theCM.highMoveParticle.gameObject.transform.localPosition = new Vector3(0f, 0f, 1f);
        }

        // 걷는 중일 때만 0.5초 간격으로 StepSound 재생
        while (isMoving)
        {
            if (!GameManager.Instance.nowPlayer.highSpeedFlag)
            {
                if (movingWaitTime >= 0.5f)
                {
                    AudioManager.Instance.Play("Step_Sound");
                    movingWaitTime = 0f;
                }
                else movingWaitTime += Time.deltaTime;
            }

            this.transform.position = Vector3.MoveTowards(this.transform.position, target, Time.deltaTime * moveSpeed);
            yield return new WaitForEndOfFrame();

            if (this.transform.position == target)
            {
                isMoving = false;
                break;
            }
        }
        yield return null;
    }

    // 플레이어 이동 종료 후처리
    IEnumerator EndMovePlayerCoroutine()
    {
        this.gameObject.GetComponent<Animator>().SetBool("FlyFlag", false);
        this.gameObject.GetComponent<Animator>().SetBool("WalkFlag", false);

        TileManager.Instance.tiles[lastTile - 1].gameObject.transform.Find("TileLine").gameObject.SetActive(false);
        lastTile = 0;

        // 투명도둑을 사용했었다면 알파값 원상복구
        if (GameManager.Instance.nowPlayer.invisibleFlag)
        {
            theCM.EndInvisibleThief();
            yield return new WaitUntil(() => !isSetTransparent);
        }

        // 고속이동이 끝났다면 스피드를 원상복구 시키고 플래그를 비활성화시킴
        if (GameManager.Instance.nowPlayer.highSpeedFlag)
        {
            SetCardFlagData setCardFlagData = new(1, playerId, false);
            string jsondata = JsonUtility.ToJson(setCardFlagData);
            byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.SetCardFlag, jsondata);
            Backend.Match.SendDataToInGameRoom(sendData);
        }

        // 플레이어가 거대화 스킬을 사용하고 이동이 끝났다면 효과 발동
        if (GameManager.Instance.nowPlayer.biggerFlag)
        {
            StartCoroutine(theCM.BiggerCoroutine());
            yield return new WaitUntil(() => theCM.biggerComplete);
            theCM.biggerComplete = false;

            StartCoroutine(SetPlayerScale("Smaller"));
            yield return new WaitUntil(() => !isSetScale);
        }

        tileNum = int.Parse(nowTile.gameObject.name);

        VirtualCamera.SetActive(false);

        finishMoving = true;

        // 내 턴일때만 UI상호작용
        if (GameManager.Instance.myCharactor.myTurn)
        {

            StartCoroutine(CheckArriveTile());
        }
    }

    // 도착한 땅의 타일을 체크하여 상호작용하는 기능
    public IEnumerator CheckArriveTile()
    {
        if (myTurn)
        {
            //일반 타일에 도착
            if (!nowTile.specialTile)
            {
                // 자신이 구매한 타일
                if (nowTile.ownPlayer == playerId)
                {
                    // 건물이 없으면 건물 구매 UI 활성화
                    if (nowTile.price == 50)
                    {
                        purchaseUi.SetActive(true);
                        GameManager.Instance.UIFlag = true;
                    }
                    // 건물이 있으면 건물 방문 효과 활성화
                    else
                    {
                        switch (nowTile.building.type)
                        {
                            // 농장
                            case 0:
                                VisitData visitData = new(200, 0, nowTile.name);
                                string jsondata = JsonUtility.ToJson(visitData);
                                byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.Visit, jsondata);
                                Backend.Match.SendDataToInGameRoom(sendData);
                                break;

                            // 제단
                            case 1:
                                VisitData visitData1 = new(0, 1, nowTile.name);
                                string jsondata1 = JsonUtility.ToJson(visitData1);
                                byte[] sendData1 = ParsingManager.Instance.ParsingSendData(ParsingType.Visit, jsondata1);
                                Backend.Match.SendDataToInGameRoom(sendData1);
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

                                byte[] data2 = ParsingManager.Instance.ParsingSendData(ParsingType.NextTurn, "");
                                Backend.Match.SendDataToInGameRoom(data2);
                                break;

                            // 랜드마크
                            case 3:
                                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.NextTurn, "");
                                Backend.Match.SendDataToInGameRoom(data);
                                break;
                        }
                    }
                }
                // 아무도 구매하지 않은 타일
                else if (nowTile.ownPlayer == -1)
                {
                    if (playerMoney >= 50)
                    {
                        groundBuyUi.SetActive(true);
                        GameManager.Instance.UIFlag = true;
                    }
                    else
                    {
                        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.NextTurn, "");
                        Backend.Match.SendDataToInGameRoom(data);
                    }
                }
                // 상대방이 구매한 타일이라면
                else
                {
                    byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.ExemptionFlag, "");
                    Backend.Match.SendDataToInGameRoom(sendData);
                }
            }

            // 특수 타일에 도착
            else
            {
                switch (nowTile.specialTileType)
                {
                    // 양계장
                    case 0:
                        ArriveTileData arriveTileData = new(this.playerId);
                        string arriveJsonData = JsonUtility.ToJson(arriveTileData);
                        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.ArriveTile, arriveJsonData);
                        Backend.Match.SendDataToInGameRoom(data);
                        break;

                    // 카드지급
                    case 1:
                        StartCoroutine(theCM.CardProvideCoroutine());
                        yield return new WaitUntil(() => theCM.isGetCard);

                        byte[] data1 = ParsingManager.Instance.ParsingSendData(ParsingType.NextTurn, "");
                        Backend.Match.SendDataToInGameRoom(data1);
                        break;

                    // 텔레포트
                    case 2:
                        StartCoroutine(TeleportSetCoroutine());
                        yield return new WaitUntil(() => tpFlag);
                        break;

                    // 부활절
                    case 3:
                        byte[] olympicData = ParsingManager.Instance.ParsingSendData(ParsingType.Olympic, "");
                        Backend.Match.SendDataToInGameRoom(olympicData);
                        break;

                    // 건물강탈
                    case 4:
                        bool canExtortion = false;

                        // 상대방 소유의 타일이 있는지 체크
                        for (int i = 0; i < TileManager.Instance.tiles.Length; i++)
                        {
                            if (TileManager.Instance.tiles[i].ownPlayer == againstPlayer.playerId)
                            {
                                canExtortion = true;
                                break;
                            }
                        }

                        // 상대방 소유의 타일이 있다면 강탈 가능
                        if (canExtortion)
                        {
                            blackBackground.SetActive(true);
                            isExtortioning = true;

                            for (int i = 0; i < TileManager.Instance.tiles.Length; i++)
                            {
                                if (TileManager.Instance.tiles[i].ownPlayer == againstPlayer.playerId) TileManager.Instance.tiles[i].canTileSelect = true;
                            }

                            yield return new WaitUntil(() => GameManager.Instance.seletedTile != null);

                            for (int i = 0; i < TileManager.Instance.tiles.Length; i++)
                            {
                                TileManager.Instance.tiles[i].canTileSelect = false;
                            }

                            isExtortioning = false;
                            blackBackground.SetActive(false);

                            ExtortionData extortionData = new(playerId);
                            string jsonData = JsonUtility.ToJson(extortionData);
                            byte[] data2 = ParsingManager.Instance.ParsingSendData(ParsingType.Extortion, jsonData);
                            Backend.Match.SendDataToInGameRoom(data2);
                        }
                        else
                        {
                            byte[] data3 = ParsingManager.Instance.ParsingSendData(ParsingType.NextTurn, "");
                            Backend.Match.SendDataToInGameRoom(data3);
                        }
                        break;
                }
            }
        }
    }

    // 텔레포트 타일 선택
    IEnumerator TeleportSetCoroutine()
    {
        blackBackground.SetActive(true);
        isSelectingTeleport = true;

        // 텔레포트 타일을 제외하고 타일을 선택 가능하게 세팅
        for (int i = 0; i < TileManager.Instance.tiles.Length; i++)
        {
            if (i != 5) TileManager.Instance.tiles[i].canTileSelect = true;
        }

        yield return new WaitUntil(() => GameManager.Instance.seletedTile != null);

        isSelectingTeleport = false;

        // 타일을 다시 선택 불가능하게 세팅
        for (int i = 0; i < TileManager.Instance.tiles.Length; i++)
        {
            TileManager.Instance.tiles[i].canTileSelect = false;
        }

        blackBackground.SetActive(false);
        tpTile = GameManager.Instance.seletedTile;

        myTurn = false;
        tpFlag = true;

        TeleportData tpData = new(tpFlag, tpTile.name);

        string jsonData = JsonUtility.ToJson(tpData);
        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.Teleport, jsonData);
        Backend.Match.SendDataToInGameRoom(data);
    }

    // 텔레포트로 이동
    IEnumerator TeleportCoroutine(int _playerId)
    {
        tpFlag = false;

        // 턴을 알리는 텍스트가 사라질때까지 대기
        yield return new WaitUntil(() => !GameManager.Instance.isActiveTurnImage);

        GameManager.Instance.players[_playerId].downInformationText.gameObject.SetActive(false);
        VirtualCamera.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        Color alpha = new Color(1, 1, 1, 0);
        this.GetComponent<SpriteRenderer>().color = alpha;

        AudioManager.Instance.Play("TeleportStart_Sound");

        // 텔레포트 이펙트 실행
        if (_playerId == 0)
        {
            GameManager.Instance.player1TeleportEffect.transform.position = TileManager.Instance.tiles[5].transform.GetChild(0).position;
            GameManager.Instance.player1TeleportEffect.SetActive(true);
            yield return new WaitForSeconds(3f);
            GameManager.Instance.player1TeleportEffect.SetActive(false);
        }
        else if (_playerId == 1)
        {
            GameManager.Instance.player2TeleportEffect.transform.position = TileManager.Instance.tiles[5].transform.GetChild(0).position;
            GameManager.Instance.player2TeleportEffect.SetActive(true);
            yield return new WaitForSeconds(3f);
            GameManager.Instance.player2TeleportEffect.SetActive(false);
        }

        // 플레이어가 서있는 타일의 정보를 갱신함
        this.tileNum = int.Parse(tpTile.gameObject.name);
        this.transform.position = tpTile.transform.Find("Pos").position;
        nowTile = tpTile.GetComponent<Tile>();
        tpTile = null;

        this.GetComponent<Animator>().SetInteger("Dir", nowTile.dir);
        yield return new WaitForSeconds(0.5f);

        AudioManager.Instance.Play("TeleportEnd_Sound");

        while (true)
        {
            alpha.a += 0.1f;
            this.GetComponent<SpriteRenderer>().color = alpha;
            yield return new WaitForSeconds(0.1f);
            if (alpha.a >= 1f) break;
        }

        VirtualCamera.SetActive(false);
        yield return new WaitForSeconds(0.5f);

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
                GameManager.Instance.nowPlayer.GetComponent<SpriteRenderer>().color = alpha;
                yield return new WaitForSeconds(0.1f);

                if (alpha.a <= 0.5f) break;
            }
        }
        else if (_parameter == "Visible")
        {
            Color alpha = new(1, 1, 1, 0.5f);
            while (true)
            {
                alpha.a += 0.1f;
                GameManager.Instance.nowPlayer.GetComponent<SpriteRenderer>().color = alpha;
                yield return new WaitForSeconds(0.1f);

                if (alpha.a >= 1f) break;
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
                GameManager.Instance.nowPlayer.gameObject.transform.localScale = scale;
                yield return new WaitForSeconds(0.1f);

                if (scale.x >= 2f) break;
            }
        }
        else if (_parameter == "Smaller")
        {
            Vector3 scale = new Vector3(2f, 2f, 0);
            while (true)
            {
                scale -= new Vector3(0.1f, 0.1f, 0);
                GameManager.Instance.nowPlayer.gameObject.transform.localScale = scale;
                yield return new WaitForSeconds(0.1f);

                if (scale.x <= 1.5f) break;
            }
        }
        isSetScale = false;
    }
}