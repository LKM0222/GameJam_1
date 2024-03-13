using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;
using BackEnd;
using BackEnd.Tcp;
using UnityEditor;

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
    float movingWaitTime;


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
    AudioManager theAudio;

    [Header("Test")]
    [SerializeField] Vector3 targetPos;
    public bool specialTurn;
    public SessionId sessionId;

    // Start is called before the first frame update
    void Start()
    {
        theTM = FindObjectOfType<TileManager>();
        theGM = FindObjectOfType<GameManager>();
        theTSI = FindObjectOfType<TurnSignScript>();
        theAudio = FindObjectOfType<AudioManager>();
        moveSpeed = 4f;
    }

    // Update is called once per frame
    void Update()
    {
        
        //의외로 쉬울지도??
        //내 턴을 알 수만 있다면 내 턴에서 하는것과 상대방턴에서 하는것을 구분할 수 있다.
        //예를들어서 내 턴이면 내 캐릭터의 PlayerManager를 사용하면 되고,
        //상대방턴이면 상대방의 PlayerManager를 사용하면 된다.
        //즉, 턴 구분만 완벽하게 이뤄지면 스크립트의 수정이 거의 없을것.

        //단, 플레이어가 구매하는 행동, 건물을 짓는 행동은 구별해줄 필요가 있다.
        
        playerMoneyText.text = playerMoney.ToString();

        if (canMove)
        {
            StartCoroutine(ReadyToMovePlayerCoroutine());
        }

        if (toosiFlag && myTurn) //card
        {
            toosiFlag = false;
            StartCoroutine(theCM.PenetrateCoroutine());
        }

        if (laserFlag && myTurn) //card
        {
            laserFlag = false;
            StartCoroutine(theCM.LaserBeamCoroutine());
        }

        if(tpFlag && myTurn)
        //if (tpFlag && GameManager.Instance.turnCount % 2 == GameManager.Instance.turnIndex)
        {
            StartCoroutine(TeleportCoroutine(playerId));
            if(GameManager.Instance.turnCount % 2 != GameManager.Instance.turnIndex){
                myTurn = false;
            }
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

        movingWaitTime = 0f;

        while (tileToGo.Count != 0)
        {
            // Player 실제 이동 코루틴 실행
            isMoving = true;
            targetPos = tileToGo[0].transform.Find("Pos").transform.position;
            StartCoroutine(MovingPlayerCoroutine(targetPos));
            yield return new WaitUntil(() => isMoving == false);

            nowTile = tileToGo[0].GetComponent<Tile>();
            tileToGo.RemoveAt(0);
            CheckPassTile();
        }

        movingWaitTime = 0f;

        StartCoroutine(EndMovePlayerCoroutine());
    }

    public IEnumerator CheckUsedCardCoroutine()
    {
        isCheckingCard = true;
        if (theGM.nowPlayer.highSpeedFlag)
        {
            theCM.HighSpeedMove();
            theAudio.Play("HighSpeedMove_Sound");
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
            theAudio.Play("Salary_Sound");
            playerMoney += 200;
            theGM.SetFloatingText(theGM.nowPlayer, 200, true);
        }

        // 투명도둑을 사용하고 나와 상대방이 겹쳐질때, 상대방의 카드가 있을 때 투명도둑 효과 발동
        if (invisibleFlag)
        {
            print("invisible");
            if (againstPlayer.nowTile == nowTile && againstPlayer.cards.Count != 0)
            {
                print("invisibleTrue"); //이거 테스트 해야됨.
                StartCoroutine(theCM.InvisibleThief());
                print("invisibleFinish");
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
            // 걷는 중일 때만 0.5초 간격으로 StepSound 재생
            if (!theGM.nowPlayer.highSpeedFlag)
            {
                if (movingWaitTime >= 0.5f)
                {
                    theAudio.Play("Step_Sound");
                    movingWaitTime = 0f;
                }
                else
                {
                    movingWaitTime += Time.deltaTime;
                }
            }

            print("is moving Check");
            this.transform.position = Vector3.MoveTowards(this.transform.position, target, Time.deltaTime * moveSpeed);
            yield return new WaitForEndOfFrame();
            if (this.transform.position == target)
            {
                print("positioncheck");
                isMoving = false;
                break;
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
            theAudio.Stop("HighSpeedMove_Sound");
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
        

        //내 턴일때만 UI상호작용
        if(GameManager.Instance.myCharactor.myTurn){
            StartCoroutine(CheckArriveTile());
        }
        
    }

    // 도착한 땅의 타일을 체크하여 상호작용하는 기능
    public IEnumerator CheckArriveTile()
    {   
        if(myTurn) //이게 없으면 상대방이 특수타일 동작 후 나 자신도 같은 UI를 띄우게 됨...! 중요
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
                                // playerMoney += 200;
                                // theGM.SetFloatingText(theGM.nowPlayer, 200, true);
                                VisitData visitData = new(200,0);
                                string jsondata = JsonUtility.ToJson(visitData);
                                byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.Visit,jsondata);                                
                                Backend.Match.SendDataToInGameRoom(sendData);
                                // print(playerMoney);
                                break;
                            // 제단
                            case 1:
                                VisitData visitData1 = new(0,1);
                                string jsondata1 = JsonUtility.ToJson(visitData1);
                                byte[] sendData1 = ParsingManager.Instance.ParsingSendData(ParsingType.Visit,jsondata1);
                                Backend.Match.SendDataToInGameRoom(sendData1);
                                // nowTile.price *= 2;
                                // print(nowTile.price);
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
                        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.NextTurn,"");
                        Backend.Match.SendDataToInGameRoom(data);
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
                        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.NextTurn, "");
                        Backend.Match.SendDataToInGameRoom(data);
                    }
                }
                // 일반 타일 중 상대방이 구매한 타일이라면
                else
                {
                    byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.ExemptionFlag, "");
                    Backend.Match.SendDataToInGameRoom(sendData);
                }
            }
        
            // 일반 타일이 아니라, 특수 타일일 경우
            else
            {
                switch (nowTile.specialTileType)
                {
                    // 양계장
                    case 0:
                        // int totalMoney = 0;
                        // for (int i = 0; i < theTM.tiles.Length; i++)
                        // {
                        //     if (theTM.tiles[i].ownPlayer == playerId && theTM.tiles[i].building.type == 0) totalMoney += 100;
                        // }
                        // playerMoney += totalMoney;
                        // theGM.SetFloatingText(theGM.nowPlayer, totalMoney, true);
                        // theGM.NextTurnFunc(); //여기서 처리해도 되나...? 서버로 안넘겨도 됨?
                        ArriveTileData arriveTileData = new(this.playerId, 100);
                        string arriveJsonData = JsonUtility.ToJson(arriveTileData);
                        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.ArriveTile, arriveJsonData);
                        Backend.Match.SendDataToInGameRoom(data);
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
                        byte[] olympicData = ParsingManager.Instance.ParsingSendData(ParsingType.Olympic, "");
                        Backend.Match.SendDataToInGameRoom(olympicData);
                        // bool haveBuilding = false;

                        // // 자신의 소유인 타일이 있다면 플래그를 활성화하고 사운드 재생
                        // for (int i = 0; i < theTM.tiles.Length; i++)
                        // {
                        //     if (theTM.tiles[i].ownPlayer == playerId)
                        //     {
                        //         haveBuilding = true;
                        //         theAudio.Play("Olympics_Sound");
                        //         break;
                        //     }
                        // }

                        // if (haveBuilding)
                        // {
                        //     // 캐릭터를 비추는 카메라를 비활성화하고 맵을 비출때까지 대기
                        //     VirtualCamera.SetActive(false);
                        //     yield return new WaitForSeconds(0.5f);

                        //     // 자신이 소유중인 타일에 파티클을 활성화
                        //     for (int i = 0; i < theTM.tiles.Length; i++)
                        //     {
                        //         if (theTM.tiles[i].ownPlayer == playerId)
                        //         {
                        //             theTM.tiles[i].price *= 2;
                        //             theTM.tiles[i].transform.Find("Pos").GetChild(0).gameObject.SetActive(true);
                        //             yield return new WaitForSeconds(0.1f);
                        //         }
                        //     }

                        //     yield return new WaitForSeconds(1f);

                        //     // 활성화한 파티클을 다시 비활성화
                        //     for (int i = 0; i < theTM.tiles.Length; i++)
                        //     {
                                // if (theTM.tiles[i].ownPlayer == playerId)
                        //         {
                        //             theTM.tiles[i].transform.Find("Pos").GetChild(0).gameObject.SetActive(false);
                        //         }
                        //     }
                        // }
                        break;

                    // 건물강탈
                    case 4:
                        bool canExtortion = false;
                        print("건물강탈!");
                        // 상대방 소유의 타일이 있는지 체크
                        for (int i = 0; i < theTM.tiles.Length; i++)
                        {
                            if (theTM.tiles[i].ownPlayer == againstPlayer.playerId)
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

                            for (int i = 0; i < theTM.tiles.Length; i++)
                            {
                                if (theTM.tiles[i].ownPlayer == againstPlayer.playerId) theTM.tiles[i].canTileSelect = true;
                            }

                            yield return new WaitUntil(() => theGM.seletedTile != null); //이부분을 뭔가 분리시켜야될듯.

                            print("건물 체크 완료");
                            for (int i = 0; i < theTM.tiles.Length; i++)
                            {
                                theTM.tiles[i].canTileSelect = false;
                            }

                            isExtortioning = false;
                            blackBackground.SetActive(false);

                            // //서버로 데이터 전송 시켜야함.
                            // ExtortionData extortionData = new(playerId);
                            // string jsonData = JsonUtility.ToJson(extortionData);
                            // byte[] data2 = ParsingManager.Instance.ParsingSendData(ParsingType.Extortion, jsonData);
                            // Backend.Match.SendDataToInGameRoom(data2);
                            
                            //여기부터 둘 다 처리되어야 하는 부분.
                            Color tileColor = theGM.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color;

                            // 타일의 Alpha 값을 서서히 0으로 줄임
                            while (tileColor.a > 0f)
                            {
                                tileColor.a -= 0.02f;
                                theGM.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;
                                yield return new WaitForSeconds(0.02f);
                            }

                            // ownPlayer를 바꿔서 땅의 소유주를 바꿔주고, signImg도 동시에 변하게함
                            // theGM.seletedTile.GetComponent<Tile>().ownPlayer = playerId;
                            ExtortionData extortionData = new(playerId);
                            string jsonData = JsonUtility.ToJson(extortionData);
                            byte[] data2 = ParsingManager.Instance.ParsingSendData(ParsingType.Extortion, jsonData);
                            Backend.Match.SendDataToInGameRoom(data2);

                            // 타일의 Alpha 값을 서서히 1로 올림
                            while (tileColor.a < 1f)
                            {
                                tileColor.a += 0.02f;
                                theGM.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;
                                yield return new WaitForSeconds(0.02f);
                            }

                            byte[] data3 = ParsingManager.Instance.ParsingSendData(ParsingType.NextTurn,"");
                            Backend.Match.SendDataToInGameRoom(data3);

                            theGM.seletedTile = null;
                        }
                        else{ //강탈 할 땅이 없다면 그냥 턴 넘김
                            byte[] data3 = ParsingManager.Instance.ParsingSendData(ParsingType.NextTurn,"");
                            Backend.Match.SendDataToInGameRoom(data3);
                        }
                        
                        break;
                }
            }
        }
    }

    IEnumerator TeleportSetCoroutine()
    {
        blackBackground.SetActive(true);
        isSelectingTeleport = true;
        for (int i = 0; i < theTM.tiles.Length; i++)
        {
            if (i != 5) theTM.tiles[i].canTileSelect = true;
        }

        yield return new WaitUntil(() => theGM.seletedTile != null);

        isSelectingTeleport = false;
        for (int i = 0; i < theTM.tiles.Length; i++)
        {
            theTM.tiles[i].canTileSelect = false;
        }

        blackBackground.SetActive(false);
        tpTile = theGM.seletedTile;
        
        myTurn = false;
        tpFlag = true;
        
        print("tpTileName is " + tpTile.name);
        print("and, TPTile Find Name is "+ GameObject.Find(tpTile.name).name);
        TeleportData tpData = new(tpFlag, tpTile.name);

        string jsonData = JsonUtility.ToJson(tpData);
        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.Teleport, jsonData);
        Backend.Match.SendDataToInGameRoom(data);
        theGM.seletedTile = null;
        // theGM.NextTurnFunc();
    }

    IEnumerator TeleportCoroutine(int _playerId)
    {
        tpFlag = false;
        // myTurn = false;

        // 턴을 알리는 텍스트가 사라질때까지 대기
        yield return new WaitUntil(() => !theGM.isActiveTurnImage);


        theGM.players[_playerId].downInformationText.gameObject.SetActive(false);
        VirtualCamera.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        Color alpha = new Color(1, 1, 1, 0);
        this.GetComponent<SpriteRenderer>().color = alpha;

        theAudio.Play("TeleportStart_Sound");
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

        theAudio.Play("TeleportEnd_Sound");
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

    //건물강탈 코루틴
    public IEnumerator ExtortionCoroutine(){
        bool canExtortion = false;
        print("건물강탈!");
        // 상대방 소유의 타일이 있는지 체크
        for (int i = 0; i < theTM.tiles.Length; i++)
        {
            if (theTM.tiles[i].ownPlayer == againstPlayer.playerId)
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

            for (int i = 0; i < theTM.tiles.Length; i++)
            {
                if (theTM.tiles[i].ownPlayer == againstPlayer.playerId) theTM.tiles[i].canTileSelect = true;
            }

            yield return new WaitUntil(() => theGM.seletedTile != null); //이부분을 뭔가 분리시켜야될듯.

            print("건물 체크 완료");
            for (int i = 0; i < theTM.tiles.Length; i++)
            {
                theTM.tiles[i].canTileSelect = false;
            }

            isExtortioning = false;
            blackBackground.SetActive(false);

            theAudio.Play("Extortion_Sound");

            Color tileColor = theGM.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color;

            // 타일의 Alpha 값을 서서히 0으로 줄임
            while (tileColor.a > 0f)
            {
                tileColor.a -= 0.02f;
                theGM.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;
                yield return new WaitForSeconds(0.02f);
            }

            // ownPlayer를 바꿔서 땅의 소유주를 바꿔주고, signImg도 동시에 변하게함
            theGM.seletedTile.GetComponent<Tile>().ownPlayer = playerId;

            // 타일의 Alpha 값을 서서히 1로 올림
            while (tileColor.a < 1f)
            {
                tileColor.a += 0.02f;
                theGM.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;
                yield return new WaitForSeconds(0.02f);
            }

            theGM.seletedTile = null;
            theGM.NextTurnFunc();
        }
    }
}