#region using
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.UI;
#endregion
public class EventManager : MonoBehaviour
{
    public static EventManager Instance = null;

    [Header("InGameServer")]
    MatchInGameRoomInfo _roomInfo; //인게임에서 방 정보를 전달하기위해 선언해둔 변수


    #region FindObjectArea
    CardManager theCardManager;
    DiceSystem theDice;
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType(typeof(EventManager)) as EventManager;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        theDice = FindObjectOfType<DiceSystem>();
    }

    void Update()
    {
        // 대기방을 떠나면서 실행되는 핸들러
        Backend.Match.OnMatchMakingRoomLeave = (MatchMakingGamerInfoInRoomEventArgs args) => { };

        //매칭신청(인게임서버접속 시작)
        Backend.Match.OnMatchMakingResponse = (MatchMakingResponseEventArgs args) =>
        {
            if (args.ErrInfo == ErrorCode.Success)
            {
                _roomInfo = args.RoomInfo; //추후에 roomToken을 써야되기 때문에 따로 저장
                Backend.Match.JoinGameServer(args.RoomInfo.m_inGameServerEndPoint.m_address,
                args.RoomInfo.m_inGameServerEndPoint.m_port,
                false, out ErrorInfo errorInfo);
            }
        };

        //인게임서버에 접속 성공했을 떄 호출되는 이벤트
        Backend.Match.OnSessionJoinInServer += (args) =>
        {
            if (args.ErrInfo == ErrorInfo.Success)
            {
                Backend.Match.JoinGameRoom(this._roomInfo.m_inGameRoomToken); //OnMatchMakingResponse에서 전달받은 RoomToken을 여기로 전달.
            }
        };

        //유저가 입장 시 호출
        Backend.Match.OnMatchInGameAccess = (MatchInGameSessionEventArgs args) =>
        {
            if (args.ErrInfo == ErrorCode.Success)
            {
                AudioManager.Instance.Stop("Title_Sound");
                SceneManager.LoadScene("MainScene");
            }
        };

        //게임시작 이벤트 브로드캐스팅 준비 완료
        Backend.Match.OnMatchInGameStart = () =>
        {
            UIManager.Instance.SetUI();
        };

        Backend.Match.OnMatchRelay = (MatchRelayEventArgs args) =>
        {
            byte[] data = args.BinaryUserData;
            ParsingData pData = JsonUtility.FromJson<ParsingData>(Encoding.Default.GetString(data));
            switch (pData.type)
            {
                case ParsingType.TurnCardSet:
                    TurnCardSet tsData = JsonUtility.FromJson<TurnCardSet>(pData.data);
                    if (tsData.randomNum == 0)
                    {
                        GameManager.Instance.turnCards[0].GetComponent<ButtonManager>().turnNum = 1;
                        GameManager.Instance.turnCards[1].GetComponent<ButtonManager>().turnNum = 0;
                    }
                    else
                    {
                        GameManager.Instance.turnCards[0].GetComponent<ButtonManager>().turnNum = 0;
                        GameManager.Instance.turnCards[1].GetComponent<ButtonManager>().turnNum = 1;
                    }
                    break;

                case ParsingType.Turn:
                    TurnCard tData = JsonUtility.FromJson<TurnCard>(pData.data);
                    GameManager.Instance.playerCount.Add(1);
                    GameManager.Instance.turnCards[tData.turncardIdx].SetActive(false);
                    if (GameManager.Instance.playerCount.Count > 1)
                    {
                        GameManager.Instance.turnCardParent.SetActive(false);
                    }

                    if (tData.turnIndex == 1)
                    {
                        GameManager.Instance.players[0].nickname = tData.nickname;
                    }
                    else
                    {
                        GameManager.Instance.players[1].nickname = tData.nickname;
                    }
                    break;

                case ParsingType.Dice:
                    if (theDice == null) theDice = FindObjectOfType<DiceSystem>();

                    StartCoroutine(theDice.RollDiceCoroutine());
                    DiceData dData = JsonUtility.FromJson<DiceData>(pData.data);
                    GameManager.Instance.diceNum = dData.diceNum;
                    theDice.diceFlag = true;
                    break;

                case ParsingType.NextTurn:
                    GameManager.Instance.NextTurnFunc();
                    GameManager.Instance.UIFlag = false;
                    break;

                case ParsingType.GroundBuy:
                    if (GameManager.Instance.myCharactor.myTurn)
                    {
                        GameManager.Instance.myCharactor.playerMoney -= 50;
                        GameManager.Instance.nowPlayer.nowTile.price = 50;
                    }
                    else
                    {
                        GameManager.Instance.myCharactor.againstPlayer.nowTile.ownPlayer
                            = GameManager.Instance.myCharactor.againstPlayer.playerId;
                        GameManager.Instance.myCharactor.againstPlayer.playerMoney -= 50;
                        GameManager.Instance.nowPlayer.nowTile.price = 50;
                    }
                    GameManager.Instance.SetFloatingText(GameManager.Instance.nowPlayer, 50, false);
                    break;

                case ParsingType.BuildingBuy:
                    if (GameManager.Instance.myCharactor.myTurn)
                    {
                        GameManager.Instance.NextTurnFunc();
                        GameManager.Instance.UIFlag = false;
                    }
                    else
                    {
                        BuildingData bdata = JsonUtility.FromJson<BuildingData>(pData.data);

                        GameManager.Instance.myCharactor.againstPlayer.nowTile.building =
                            GameManager.Instance.buildings[bdata.buildingNum];

                        GameManager.Instance.myCharactor.againstPlayer.playerMoney -= 50;
                        GameManager.Instance.myCharactor.againstPlayer.nowTile.price =
                        GameManager.Instance.buildings[bdata.buildingNum].toll * GameManager.Instance.magnification;
                        GameManager.Instance.SetFloatingText(GameManager.Instance.nowPlayer, 50, false);
                        GameManager.Instance.NextTurnFunc();
                        GameManager.Instance.UIFlag = false;
                    }
                    break;

                case ParsingType.Teleport:
                    TeleportData tpData = JsonUtility.FromJson<TeleportData>(pData.data);

                    GameManager.Instance.nowPlayer.tpFlag = tpData.tpFlag;
                    GameManager.Instance.nowPlayer.tpTile = GameObject.Find(tpData.tpTileNum);

                    GameManager.Instance.seletedTile = null;

                    GameManager.Instance.NextTurnFunc();
                    GameManager.Instance.UIFlag = false;
                    break;

                case ParsingType.Card:
                    CardData cardData = JsonUtility.FromJson<CardData>(pData.data);
                    GameManager.Instance.nowPlayer.cards.Add(cardData.card);
                    break;


                case ParsingType.TileSelect:
                    TileSelectData tileSelectData = JsonUtility.FromJson<TileSelectData>(pData.data);
                    GameManager.Instance.seletedTile = GameObject.Find(tileSelectData.tilename);
                    break;

                case ParsingType.Extortion:
                    ExtortionData extortionData = JsonUtility.FromJson<ExtortionData>(pData.data);
                    Color tileColor = GameManager.Instance.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color;
                    StartCoroutine(ExtortionAlphaCoroutine(tileColor, extortionData.playerId));
                    break;

                case ParsingType.CardClick:
                    CardClickData cData = JsonUtility.FromJson<CardClickData>(pData.data);
                    switch (cData.cardNum)
                    {
                        case 1:
                            GameManager.Instance.nowPlayer.highSpeedFlag = true;
                            break;

                        case 2:
                            GameManager.Instance.nowPlayer.invisibleFlag = true;
                            break;

                        case 3:
                            GameManager.Instance.nowPlayer.biggerFlag = true;
                            break;

                        case 4:
                            GameManager.Instance.nowPlayer.lowerDiceFlag = true;
                            break;

                        case 5:
                            GameManager.Instance.nowPlayer.higherDiceFlag = true;
                            break;

                        case 7:
                            GameManager.Instance.nowPlayer.laserFlag = true;
                            break;
                    }
                    break;

                case ParsingType.CardListAdd:
                    CardData cardData1 = JsonUtility.FromJson<CardData>(pData.data);

                    var _card = Instantiate(GameManager.Instance.nowPlayer.cardPrefab,
                        Vector3.zero, Quaternion.identity, GameManager.Instance.nowPlayer.cardParent);
                    _card.transform.localPosition = new Vector3(0f, 0f, 0f);

                    GameManager.Instance.nowPlayer.cards.Add(cardData1.card);
                    break;

                case ParsingType.CardDestory:
                    CardDestroyData destroyData = JsonUtility.FromJson<CardDestroyData>(pData.data);
                    Destroy(destroyData.destoryCard);
                    Destroy(GameManager.Instance.nowPlayer.cardParent.GetChild(0).gameObject);
                    GameManager.Instance.nowPlayer.cards.Remove(GameManager.Instance.nowPlayer.cards.Find(card => card.cardCode == destroyData.cardCode));
                    break;

                case ParsingType.InvisibleThief:
                    GameManager.Instance.invisibleCardNum = UnityEngine.Random.Range(0,
                        GameManager.Instance.nowPlayer.againstPlayer.cards.Count);
                    break;

                case ParsingType.ExemptionFlag:
                    StartCoroutine(ExemptionCoroutine());
                    break;

                case ParsingType.ExemptionFlagSet:
                    GameManager.Instance.nowPlayer.exemptionFlag = true;
                    break;

                case ParsingType.Visit:
                    VisitData visitData = JsonUtility.FromJson<VisitData>(pData.data);
                    switch (visitData.caseNum)
                    {
                        case 0:
                            GameManager.Instance.nowPlayer.playerMoney += visitData.money;
                            GameManager.Instance.SetFloatingText(GameManager.Instance.nowPlayer, visitData.money, true);
                            GameManager.Instance.NextTurnFunc();
                            break;

                        case 1:
                            GameManager.Instance.nowPlayer.nowTile = GameObject.Find(visitData.tileNum).GetComponent<Tile>();
                            StartCoroutine(TempleCoroutine());
                            break;
                    }
                    break;

                case ParsingType.ArriveTile:
                    StartCoroutine(ArriveCoroutine(pData));
                    break;

                case ParsingType.Olympic:
                    StartCoroutine(OlympicMethod(GameManager.Instance.nowPlayer.playerId, GameManager.Instance.nowPlayer.VirtualCamera));
                    break;

                case ParsingType.Laser:
                    LaserData laserData = JsonUtility.FromJson<LaserData>(pData.data);
                    GameManager.Instance.seletedTile = GameObject.Find(laserData.laserTileNum);
                    if (theCardManager == null) theCardManager = GameObject.Find("CardManager").GetComponent<CardManager>();
                    StartCoroutine(theCardManager.LaserCoroutine());
                    break;

                case ParsingType.Disconnect:
                    GameManager.Instance.errorUI.SetActive(true);
                    break;

                case ParsingType.SetCardFlag:
                    SetCardFlagData setCardFlagData = JsonUtility.FromJson<SetCardFlagData>(pData.data);
                    switch (setCardFlagData.cardCode)
                    {
                        case 1:
                            AudioManager.Instance.Stop("HighSpeedMove_Sound");
                            if (theCardManager == null) theCardManager = GameObject.Find("CardManager").GetComponent<CardManager>();
                            theCardManager.EndHighSpeedMove();
                            break;
                    }
                    break;

                case ParsingType.Surrender:
                    SurrenderData surrenderData = JsonUtility.FromJson<SurrenderData>(pData.data);
                    GameManager.Instance.gameOverUI.SetActive(true);
                    UIManager.Instance.goImg.GetComponent<UnityEngine.UI.Image>().sprite = UIManager.Instance.surrend[surrenderData.playerId];
                    UIManager.Instance.goTitle.text = "항복!";
                    UIManager.Instance.goMoney.text = (surrenderData.playerId == 0 ? "1P" : "2P") + "가 항복했습니다!";
                    MatchGameResult matchGameResult = new MatchGameResult();
                    Backend.Match.MatchEnd(matchGameResult);
                    break;
            }
        };

        //게임 종료(정상적: 게임에서 게임오버 함수 호출, 비정상적 : 플레이어가 나감)
        Backend.Match.OnMatchResult = (MatchResultEventArgs args) =>
        {
            GameManager.Instance.gameOverUI.SetActive(true);
        };

        //게임 중, 플레이어가 연결 끊김.
        Backend.Match.OnSessionOffline = (MatchInGameSessionEventArgs args) =>
        {
            UIManager.Instance.errorUI.SetActive(true);
        };
    }


    //건물강탈 코루틴
    IEnumerator ExtortionAlphaCoroutine(Color tileColor, int playerId)
    {
        AudioManager.Instance.Play("Extortion_Sound");

        while (tileColor.a > 0f)
        {
            tileColor.a -= 0.02f;
            GameManager.Instance.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;
            yield return new WaitForSeconds(0.02f);
        }

        GameManager.Instance.seletedTile.GetComponent<Tile>().ownPlayer = playerId;

        while (tileColor.a < 1f)
        {
            tileColor.a += 0.02f;
            GameManager.Instance.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;
            yield return new WaitForSeconds(0.02f);
        }

        GameManager.Instance.seletedTile = null;

        GameManager.Instance.NextTurnFunc();
        GameManager.Instance.UIFlag = false;
    }

    //양계장 코루틴
    IEnumerator ArriveCoroutine(ParsingData pData)
    {
        ArriveTileData arriveTileData = JsonUtility.FromJson<ArriveTileData>(pData.data);
        int totalMoney = 0;

        //타일 체크
        for (int i = 0; i < TileManager.Instance.tiles.Length; i++)
        {
            if (TileManager.Instance.tiles[i].ownPlayer == arriveTileData.playerId && TileManager.Instance.tiles[i].building.type == 0) totalMoney += 100;
        }
        GameManager.Instance.nowPlayer.playerMoney += totalMoney;
        yield return new WaitForSeconds(0.5f);

        if (totalMoney > 0)
        {
            GameManager.Instance.SetFloatingText(GameManager.Instance.nowPlayer, totalMoney, true);
        }

        GameManager.Instance.NextTurnFunc();
        GameManager.Instance.UIFlag = false;
    }

    //재단 코루틴
    IEnumerator TempleCoroutine()
    {
        AudioManager.Instance.Play("Olympics_Sound");
        GameManager.Instance.nowPlayer.nowTile.price *= 2;
        GameManager.Instance.nowPlayer.nowTile.transform.Find("Pos").GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        GameManager.Instance.nowPlayer.nowTile.transform.Find("Pos").GetChild(0).gameObject.SetActive(false);
        GameManager.Instance.NextTurnFunc();
    }

    //통행료 지불 코루틴(내 움직임이 끝날때까지 기다렸다가 징수하기 위해 코루틴 사용)
    IEnumerator ExemptionCoroutine()
    {
        yield return new WaitUntil(() => GameManager.Instance.nowPlayer.finishMoving == true);

        if (!GameManager.Instance.nowPlayer.exemptionFlag)
        {
            GameManager.Instance.nowPlayer.playerMoney -= GameManager.Instance.nowPlayer.nowTile.price;
            GameManager.Instance.SetFloatingText(GameManager.Instance.nowPlayer, GameManager.Instance.nowPlayer.nowTile.price, false);
            GameManager.Instance.nowPlayer.againstPlayer.playerMoney += GameManager.Instance.nowPlayer.nowTile.price;
            GameManager.Instance.SetFloatingText(GameManager.Instance.nowPlayer.againstPlayer, GameManager.Instance.nowPlayer.nowTile.price, true);

            GameManager.Instance.NextTurnFunc();
        }
        else
        {
            StartCoroutine(RunExemptionParticle());
        }
    }

    public IEnumerator OlympicMethod(int playerId, GameObject VirtualCamera)
    {
        bool haveBuilding = false;

        UIManager.Instance.olympicText.SetActive(true);

        // 자신의 소유인 타일이 있다면 플래그를 활성화하고 사운드 재생
        for (int i = 0; i < TileManager.Instance.tiles.Length; i++)
        {
            if (TileManager.Instance.tiles[i].ownPlayer == playerId)
            {
                AudioManager.Instance.Play("Olympics_Sound");
                haveBuilding = true;
                break;
            }
        }

        if (haveBuilding)
        {
            // 캐릭터를 비추는 카메라를 비활성화하고 맵을 비출때까지 대기
            VirtualCamera.SetActive(false);
            yield return new WaitForSeconds(1f);

            // 자신이 소유중인 타일에 파티클을 활성화
            for (int i = 0; i < TileManager.Instance.tiles.Length; i++)
            {
                if (TileManager.Instance.tiles[i].ownPlayer == playerId)
                {
                    TileManager.Instance.tiles[i].price *= 2;
                    TileManager.Instance.tiles[i].transform.Find("Pos").GetChild(0).gameObject.SetActive(true);
                    yield return new WaitForSeconds(0.1f);
                }
            }

            yield return new WaitForSeconds(1f);

            // 활성화한 파티클을 다시 비활성화
            for (int i = 0; i < TileManager.Instance.tiles.Length; i++)
            {
                if (TileManager.Instance.tiles[i].ownPlayer == playerId)
                {
                    TileManager.Instance.tiles[i].transform.Find("Pos").GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        UIManager.Instance.olympicText.SetActive(false);
        GameManager.Instance.NextTurnFunc();
    }

    public IEnumerator RunExemptionParticle()
    {
        if (theCardManager == null) theCardManager = GameObject.Find("CardManager").GetComponent<CardManager>();

        // 천사꼬꼬 카드를 찾아서 삭제
        for (int i = 0; i < GameManager.Instance.nowPlayer.cards.Count; i++)
        {
            if (GameManager.Instance.nowPlayer.cards[i].cardCode == 6)
            {
                GameManager.Instance.nowPlayer.cards.RemoveAt(i);
                Destroy(GameManager.Instance.nowPlayer.cardParent.GetChild(0).gameObject);
                break;
            }
        }

        // 오디오 재생 및 파티클 실행
        AudioManager.Instance.Play("TollExemption_Sound");

        theCardManager.exemptionParticle.transform.position = GameManager.Instance.nowPlayer.transform.position;
        theCardManager.exemptionParticle.gameObject.SetActive(true);
        theCardManager.exemptionParticle.Play();

        yield return new WaitForSeconds(1f);

        theCardManager.exemptionParticle.gameObject.SetActive(false);

        GameManager.Instance.nowPlayer.exemptionFlag = false;

        // 천사꼬꼬 카드가 더 있다면 플래그를 다시 켜줌
        for (int i = 0; i < GameManager.Instance.nowPlayer.cards.Count; i++)
        {
            if (GameManager.Instance.nowPlayer.cards[i].cardCode == 6)
            {
                GameManager.Instance.nowPlayer.exemptionFlag = true;
                break;
            }
        }

        GameManager.Instance.NextTurnFunc();
    }
}