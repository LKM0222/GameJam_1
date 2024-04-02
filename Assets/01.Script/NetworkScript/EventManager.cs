#region using
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine.SceneManagement;
using System.Text;
#endregion
public class EventManager : MonoBehaviour
{
    public static EventManager Instance = null;

    [Header("Invite Info")]
    public MatchMakingUserInfo matchMakingUserInfo;
    public SessionId roomId;
    public string roomToken;
    public bool acceptFlag = false; //초대수락 플래그

    [Header("InGameServer")]
    MatchInGameRoomInfo _roomInfo; //인게임에서 방 정보를 전달하기위해 선언해둔 변수


    #region FindObjectArea
    GroundBuyScript _theGBS;
    CardManager theCM;

    TileManager theTM;

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
        _theGBS = FindObjectOfType<GroundBuyScript>();
        theCM = FindObjectOfType<CardManager>();
        theTM = FindObjectOfType<TileManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Backend.Match.OnMatchMakingRoomLeave = (MatchMakingGamerInfoInRoomEventArgs args) =>
        {
            // 대기방을 떠나면서 실행되는 핸들러
            // 대기방을 떠날때, 유저 리스트와 대기방을 나갔다는 로그가 필요함.
            // 유저 리스트는 좀만 있다가 수정하자. 대기방 나갔다는 로그부터
            if (args.ErrInfo == ErrorCode.Success)
            {//성공적으로 퇴장 성공
                print("매칭룸 퇴장");
            }
            if (args.ErrInfo == ErrorCode.InvalidOperation)
            {//매칭중이라 퇴장 실패
                Debug.Log("매칭중이라 방에서 나갈 수 없습니다.");
            }
            if (args.ErrInfo == ErrorCode.Match_Making_KickedByOwner)
            { //강퇴당했을경우

            }
        };

        //매칭신청(인게임서버접속 시작)
        Backend.Match.OnMatchMakingResponse = (MatchMakingResponseEventArgs args) =>
        {
            // 유저가 매칭을 신청, 취소 했을 때 그리고 매칭이 성사되었을 때 호출되는 이벤트
            switch (args.ErrInfo)
            {
                case ErrorCode.Match_InProgress: //매칭신청에 성공하였을때
                    Debug.Log("매칭신청 성공");
                    break;

                case ErrorCode.Success: //매칭이 성사되었을 떄 여기서 인게임 서버 접속시도
                    Debug.Log("매칭 성사 , 인게임 서버에 접속 시도합니다.");
                    _roomInfo = args.RoomInfo; //추후에 roomToken을 써야되기 때문에 따로 저장
                    if (Backend.Match.JoinGameServer(args.RoomInfo.m_inGameServerEndPoint.m_address,
                    args.RoomInfo.m_inGameServerEndPoint.m_port,
                    false, out ErrorInfo errorInfo) == false)
                    {
                        //true인 경우, OnSessionJoinInServer 호출.
                        Debug.Log("errorinfo is" + errorInfo.Reason);
                    }
                    else
                    {
                        Debug.Log("OnSessionJoinInServer Start");
                    }
                    break;

                case ErrorCode.Match_MatchMakingCanceled: //매칭 신청을 취소했을때
                    //매칭이 성사되었으나 서버에서 인게임 룸 생성에 실패했을 경우(다시 매칭을 신청해야 합니다.)
                    Debug.Log("매칭을 취소하였습니다. " + args.Reason);
                    break;

                case ErrorCode.Match_InvalidModeType: //잘못된 모드 타입으로 신청했을 때
                    Debug.Log("잘못된 모드타입 입니다.");
                    break;

                case ErrorCode.Match_Making_InvalidRoom: //대기방에 허용된 인원보다 많은 인원이 존재하는 경우
                    Debug.Log("대기방 안에 허용된 인원보다 많은 인원이 존재합니다." + args.Reason);
                    break;


                case ErrorCode.InvalidOperation:  // 잘못된 요청을 했을 때
                    // 잘못된 요청을 했을 때, 매치를 두번 이상 신청했을 때, 방장이 아닌 유저가 매칭 신청을 했을 때, 
                    // 방장이 아닌 유저가 매칭을 취소했을 때.
                    // 대기방에 2명 이상의 유저가 존재하는데 1:1, 개인전 매칭을 신청한 경우
                    Debug.Log("잘못된 요청입니다." + args.Reason);
                    break;
            }
        };

        Backend.Match.OnSessionJoinInServer += (args) =>
        { //인게임서버에 접속 성공했을 떄 호출되는 이벤트 이 이벤트가 호출되어야 서버에 접속성공한것.
            if (args.ErrInfo == ErrorInfo.Success)
            {
                Backend.Match.JoinGameRoom(this._roomInfo.m_inGameRoomToken); //OnMatchMakingResponse에서 전달받은 RoomToken을 여기로 전달.
                GameManager.Instance.nowPlayer.sessionId = args.Session.SessionId;
            }
            else
            {
                switch (args.ErrInfo.Category)
                {
                    case ErrorCode.Exception:
                        Debug.Log("인게임 서버 접속 실패사유 : " + args.ErrInfo.Reason);
                        break;

                    case ErrorCode.AuthenticationFailed:
                        Debug.Log("서버에 이전 세션 접속기록이 남아있습니다.");
                        break;

                    default:
                        Debug.Log("이외의 오류 : " + args.ErrInfo.Reason);
                        break;
                }
            }
        };

        Backend.Match.OnMatchInGameAccess = (MatchInGameSessionEventArgs args) =>
        { //유저가 게임방에 입장할 때마다 호출되는 이벤트입니다.
            // 입장한 유저(자기 자신)에게 호출됩니다.
            // 이미 게임방에 접속해있던 모든 유저에게 호출됩니다.
            if (args.ErrInfo == ErrorCode.Success)
            {
                Debug.Log(args.GameRecord.m_nickname + "접속 완료");
                BackendManager.Instance.mySessionId = args.GameRecord.m_sessionId;
                print("접속한 사람의 정보:" + args.GameRecord.m_sessionId + ", " + args.GameRecord.m_nickname);

                AudioManager.Instance.Stop("Title_Sound");

                SceneManager.LoadScene("MainScene");
                //방에 접속하면 누가 접속완료하였는지 닉네임이 표시된다.
                //이를 활용해 모두 접속 완료라면 씬을 옮겨서도 데이터를 주고받을 수 있을까?
            }
        };

        Backend.Match.OnMatchInGameStart = () =>
        { //게임 시작 이밴트
          // 모든 유저의 데이터를 브로드캐스팅 할 준비가 되었다는 이벤트
          // 모든 유저가 게임방에 접속한 이후 
          // 콘솔에서 설정한 매치 시작 대기시간이 지난 이후에 모든 유저에게 게임 시작 이벤트가 호출
          // 게임 시작 이벤트가 호출된 후 게임에서 필요한 데이터 동기화, 유저들 간 로딩 상황 공유 등 다양한 데이터 설정을 진행

            //데이터를 넘길 수는 있지만, 이제부터 누가 선 플레이어고, 게임 진행에 대하여 데이터 전송이 이뤄져야한다....
            Debug.Log("게임 시작! 이제부터 데이터가 모든 유저에게 브로드캐스팅 가능합니다!");
            //이후부터 게임 시작되었다는 뭔가가 필요할듯.
            //턴이 시작되었다는 뭔가가 필요...!
            //ParsingData의 클래스 인스턴스를 생성하여, 선언된 data를 json으로 파싱 후 string데이터를 다시 byte[]로 변환해서 전송.
            UIManager.Instance.SetUI();
        };

        Backend.Match.OnMatchRelay = (MatchRelayEventArgs args) =>
        { //데이터 수신
            //수신받은 Json데이터를 다시 ParsingData클래스로 변환 후 처리.
            //받는 함수(받는 데이터는 byte[]로 받음.
            //수신이벤트에서 각 클래스로 변환하는 함수.
            print("Recv!");
            byte[] data = args.BinaryUserData;
            ParsingData pData = JsonUtility.FromJson<ParsingData>(Encoding.Default.GetString(data));
            //pData.type : 데이터의 타입, pData.data : string데이터 (클래스별 데이터라 각 클래스에 맞는 파싱과정 필요)
            //데이터의 타입으로 스위치문 결정, 데이터를 다시 위와 같은 과정으로 알맞은 클래스로 변환 후 사용.
            switch (pData.type)
            {
                case ParsingType.TurnCardSet: //게임 시작 시 두 클라이언트 간 턴 선택하는 카드의 랜덤번호를 맞춰준다.
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

                case ParsingType.Session: //플레이어 인덱스에 맞게 세션 저장.
                    SessionData sessionData = JsonUtility.FromJson<SessionData>(pData.data);
                    GameManager.Instance.sessionArr[sessionData.turnNum] = sessionData.sessionId;
                    break;

                case ParsingType.Turn: //턴 선택 분기, 상대방이 어떤 카드를 뽑았는지 전달해주고, 상대방이 뽑은 카드를 비활성화 처리 하기 위해 사용
                    print("turn case");
                    TurnCard tData = JsonUtility.FromJson<TurnCard>(pData.data);
                    GameManager.Instance.playerCount.Add(1);
                    GameManager.Instance.turnCards[tData.turncardIdx].SetActive(false);
                    if (GameManager.Instance.playerCount.Count > 1)
                    {
                        GameManager.Instance.turnCardParent.SetActive(false);
                    }
                    break;

                case ParsingType.Dice: //주사위 데이터
                    print("dice type");
                    StartCoroutine(DiceSystem.Instance.RollDiceCoroutine());
                    DiceData dData = JsonUtility.FromJson<DiceData>(pData.data);
                    GameManager.Instance.diceNum = dData.diceNum;
                    DiceSystem.Instance.diceFlag = true;
                    break;

                case ParsingType.NextTurn: //다음턴으로 넘기기
                    GameManager.Instance.NextTurnFunc(); //이 함수로
                    GameManager.Instance.UIFlag = false;
                    break;

                case ParsingType.GroundBuy: //땅 구매
                    if (GameManager.Instance.myCharactor.myTurn)
                    {
                        GameManager.Instance.myCharactor.groundCount += 1;
                        GameManager.Instance.myCharactor.playerMoney -= 50;
                        GameManager.Instance.nowPlayer.nowTile.price = 50;
                    }
                    else
                    {
                        //상대방이 땅을 구매했을 때, 상대방 땅 색깔로 구매되었다는걸 알려줘야함.
                        GameManager.Instance.myCharactor.againstPlayer.nowTile.ownPlayer
                            = GameManager.Instance.myCharactor.againstPlayer.playerId;
                        GameManager.Instance.myCharactor.againstPlayer.groundCount += 1;
                        GameManager.Instance.myCharactor.againstPlayer.playerMoney -= 50;
                        GameManager.Instance.nowPlayer.nowTile.price = 50;
                    }
                    GameManager.Instance.SetFloatingText(GameManager.Instance.nowPlayer, 50, false);
                    break;

                case ParsingType.BuildingBuy: //건물건설
                    if (GameManager.Instance.myCharactor.myTurn)
                    {
                        // //모든 작업 완료 후 턴 넘기기
                        // 여기는 그냥 버튼 눌렀을때 처리해주면 됨. 자기턴이니깐.
                        // 상대방에게는 아래와 같이 전달해주면 될듯.
                        GameManager.Instance.NextTurnFunc();
                        GameManager.Instance.UIFlag = false;

                    }
                    else
                    {
                        BuildingData bdata = JsonUtility.FromJson<BuildingData>(pData.data);

                        GameManager.Instance.myCharactor.againstPlayer.nowTile.building =
                            GameManager.Instance.buildings[bdata.buildingNum];

                        GameManager.Instance.myCharactor.againstPlayer.buildingCount += 1;
                        GameManager.Instance.myCharactor.againstPlayer.playerMoney -= 50; //건물 건설비용
                        GameManager.Instance.myCharactor.againstPlayer.nowTile.price =
                        GameManager.Instance.buildings[bdata.buildingNum].toll;
                        GameManager.Instance.SetFloatingText(GameManager.Instance.nowPlayer, 50, false);
                        GameManager.Instance.NextTurnFunc();
                        GameManager.Instance.UIFlag = false;
                    }
                    break;

                case ParsingType.Teleport:
                    TeleportData tpData = JsonUtility.FromJson<TeleportData>(pData.data);

                    GameManager.Instance.nowPlayer.tpFlag = tpData.tpFlag; //전달받은 값을 현재 턴의 플레이어에 할당.
                    GameManager.Instance.nowPlayer.tpTile = GameObject.Find(tpData.tpTileNum); //땅이 계속 0으로만 들어감.

                    GameManager.Instance.seletedTile = null;

                    //이후 턴 넘기기.
                    GameManager.Instance.NextTurnFunc();
                    GameManager.Instance.UIFlag = false;
                    break;

                case ParsingType.Card:
                    CardData cardData = JsonUtility.FromJson<CardData>(pData.data);
                    GameManager.Instance.nowPlayer.cards.Add(cardData.card);
                    break;

                //건물파괴 타일 선택시(타일선택)
                case ParsingType.TileSelect:
                    TileSelectData tileSelectData = JsonUtility.FromJson<TileSelectData>(pData.data);
                    GameManager.Instance.seletedTile = GameObject.Find(tileSelectData.tilename); //missing오류...?
                    break;

                //건물파괴
                case ParsingType.Extortion:
                    print("recv ExtortionData!");
                    ExtortionData extortionData = JsonUtility.FromJson<ExtortionData>(pData.data);
                    Color tileColor = GameManager.Instance.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color;
                    StartCoroutine(ExtortionAlphaCoroutine(tileColor, extortionData.playerId));



                    break;

                case ParsingType.CardClick:
                    CardClickData cData = JsonUtility.FromJson<CardClickData>(pData.data);

                    switch (cData.cardNum)
                    {
                        case 1: //고속이동
                            GameManager.Instance.nowPlayer.highSpeedFlag = true;
                            break;

                        case 2://투명도둑
                            GameManager.Instance.nowPlayer.invisibleFlag = true;
                            break;

                        case 3://거대화 꼬꼬
                            GameManager.Instance.nowPlayer.biggerFlag = true;
                            break;

                        case 4: //소형알
                            GameManager.Instance.nowPlayer.lowerDiceFlag = true;
                            break;

                        case 5: //대형알
                            GameManager.Instance.nowPlayer.higherDiceFlag = true;
                            break;

                        // 6번 통행료 면제는 사용카드가 아니라 패시브 카드라서 패스.

                        case 7: //레이저빔
                            GameManager.Instance.nowPlayer.laserFlag = true;
                            break;
                    }
                    break;

                case ParsingType.CardListAdd:
                    CardData cardData1 = JsonUtility.FromJson<CardData>(pData.data);

                    var _card = Instantiate(GameManager.Instance.nowPlayer.cardPrefab,
                        Vector3.zero, Quaternion.identity, GameManager.Instance.nowPlayer.cardParent);
                    _card.transform.localPosition = new Vector3(0f, 0f, 0f);

                    GameManager.Instance.nowPlayer.cards.Add(cardData1.card); //카드를 뽑았다면 현재 플레이어 카드리스트에 값 추가.
                    break;

                case ParsingType.CardDestory:
                    print("Destroy");
                    CardDestroyData destroyData = JsonUtility.FromJson<CardDestroyData>(pData.data);
                    print("Recv : " + destroyData.cardCode);
                    Destroy(destroyData.destoryCard);
                    Destroy(GameManager.Instance.nowPlayer.cardParent.GetChild(0).gameObject);
                    GameManager.Instance.nowPlayer.cards.Remove(GameManager.Instance.nowPlayer.cards.Find(card => card.cardCode == destroyData.cardCode));
                    break;

                case ParsingType.InvisibleThief: //카드 투명도둑
                    GameManager.Instance.invisibleCardNum = UnityEngine.Random.Range(0,
                        GameManager.Instance.nowPlayer.againstPlayer.cards.Count);
                    //랜덤으로 뽑았으니 함수 계속.
                    break;

                case ParsingType.ExemptionFlag: //상대방 땅에 걸린경우
                    StartCoroutine(ExemptionCoroutine());
                    break;

                case ParsingType.ExemptionFlagSet:
                    print("exemptionFalg true");
                    GameManager.Instance.nowPlayer.exemptionFlag = true;
                    break;

                case ParsingType.Visit:
                    VisitData visitData = JsonUtility.FromJson<VisitData>(pData.data);
                    switch (visitData.caseNum)
                    {
                        case 0: //농장
                            GameManager.Instance.nowPlayer.playerMoney += visitData.money; //money = 200
                            GameManager.Instance.SetFloatingText(GameManager.Instance.nowPlayer, visitData.money, true);
                            GameManager.Instance.NextTurnFunc();
                            break;

                        case 1: //재단
                            StartCoroutine(TempleCoroutine()); //재단 활성화 되었을때 파티클 재생위해 코루틴으로 수정
                            break;

                    }

                    break;

                case ParsingType.ArriveTile: //양계장에 도착할 경우
                    StartCoroutine(ArriveCoroutine(pData)); //플로팅 텍스트 때문에 코루틴으로 뺌
                    break;

                case ParsingType.Olympic:
                    StartCoroutine(GameManager.Instance.OlympicMethod(GameManager.Instance.nowPlayer.playerId, GameManager.Instance.nowPlayer.VirtualCamera));
                    GameManager.Instance.NextTurnFunc();
                    break;

                case ParsingType.Laser:
                    LaserData laserData = JsonUtility.FromJson<LaserData>(pData.data);
                    GameManager.Instance.seletedTile = GameObject.Find(laserData.laserTileNum);
                    StartCoroutine(GameManager.Instance.LaserCoroutine());

                    break;
            }
        };

        //게임 종료(정상적: 게임에서 게임오버 함수 호출, 비정상적 : 플레이어가 나감) 결과 처리 후 호출이 된다면 분기를 나눌 수 있는데...
        Backend.Match.OnMatchResult = (MatchResultEventArgs args) =>
        {
            // TODO
            print("호출완료!");
            GameManager.Instance.gameOverUI.SetActive(true);
            print(GameManager.Instance.nowPlayer.againstPlayer.playerId + " 승리!");
            print("Game Over!");
            switch (args.ErrInfo)
            {
                case ErrorCode.Success:
                    print("결과 종합 성공");
                    break;

                case ErrorCode.Exception:
                    print("결과 종합 실패 - 서버에서 결과 종합을 실패한 경우 " + args.Reason);
                    break;

                case ErrorCode.Match_InGame_Timeout:
                    print("게임 시작 실패(룸 생성 후 모든 유저가 게임에 접속하지 않은 경우) " + args.Reason);
                    break;
            }
        };

        //게임 중, 플레이어가 연결 끊김.
        Backend.Match.OnSessionOffline = (MatchInGameSessionEventArgs args) =>
        {
            if (args.ErrInfo == ErrorCode.NetworkOffline)
            {
                print("플레이어가 연결을 끊어 연결이 끊어졌습니다. 남아있는 플레이어가 자동 우승이 됩니다. \n " + "끊어진 플레이어 정보 : "
                        + args.GameRecord);
                UIManager.Instance.SetErrorUI();
            }
            if (args.ErrInfo == ErrorCode.Exception)
            {
                print("서버가 끊어졌습니다. 게임 결과는 처리되지 않습니다.\n " + "끊어진 플레이어 정보 : "
                        + args.GameRecord);
            }

        };

        //인게임 서버 종료
        Backend.Match.OnLeaveInGameServer = (MatchInGameSessionEventArgs args) =>
        {
            switch (args.ErrInfo)
            {
                case ErrorCode.Success:
                    print("정상적으로 종료됨");
                    UIManager.Instance.SetErrorUI();
                    break;

                case ErrorCode.Exception:
                    print("에러로 인한 종료 : " + args.Reason);
                    break;

                case ErrorCode.AuthenticationFailed:
                    print("재접속 오류");
                    break;

            }
        };
        //매치메이킹 서버 종료
        Backend.Match.OnLeaveMatchMakingServer = (LeaveChannelEventArgs args) =>
        {
            switch (args.ErrInfo.Category)
            {
                case ErrorCode.Success:
                    print("정상적으로 매치메이킹 서버 종료됨");
                    break;

                case ErrorCode.Exception:
                    print("비정상적으로 종료됨 : " + args.ErrInfo.Detail);
                    break;

                case ErrorCode.DisconnectFromRemote:
                    print("콘솔에서 생성하지 않은 매치 타입 & 매치 유형으로 매칭을 신청 오류");
                    break;

                case ErrorCode.NetworkTimeout:
                    print("매치 서버와 클라이언트가 30초 이상 연결이 끊어진 경우");
                    break;
            }
        };

    }


    //건물강탈 코루틴 빼놓은것.
    IEnumerator ExtortionAlphaCoroutine(Color tileColor, int playerId)
    {
        AudioManager.Instance.Play("Extortion_Sound");
        //여기부터 둘 다 처리되어야 하는 부분.

        // 타일의 Alpha 값을 서서히 0으로 줄임
        while (tileColor.a > 0f)
        {
            tileColor.a -= 0.02f;
            GameManager.Instance.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;
            yield return new WaitForSeconds(0.02f);
        }

        // ownPlayer를 바꿔서 땅의 소유주를 바꿔주고, signImg도 동시에 변하게함
        GameManager.Instance.seletedTile.GetComponent<Tile>().ownPlayer = playerId;

        // 타일의 Alpha 값을 서서히 1로 올림
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
        print("도착 코루틴 시작");
        ArriveTileData arriveTileData = JsonUtility.FromJson<ArriveTileData>(pData.data);
        int totalMoney = 0;
        print("파싱완료");
        //타일 체크
        for (int i = 0; i < TileManager.Instance.tiles.Length; i++)
        {
            print("TileCheck");
            if (TileManager.Instance.tiles[i].ownPlayer == arriveTileData.playerId && TileManager.Instance.tiles[i].building.type == 0) totalMoney += 100;
        }
        GameManager.Instance.nowPlayer.playerMoney += totalMoney;
        print("돈 추가 완료");
        yield return new WaitForSeconds(0.5f);
        if (totalMoney > 0)
        {
            GameManager.Instance.SetFloatingText(GameManager.Instance.nowPlayer, totalMoney, true);
            print("플로팅 텍스트 완료");
        }
        GameManager.Instance.NextTurnFunc();
        GameManager.Instance.UIFlag = false;
        print("nextturn Finish");
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
    //왠지 통신 넘어가면서 nowplayer가 상대방으로 바꼈을때 꼬일거같긴한데....
    IEnumerator ExemptionCoroutine()
    {
        yield return new WaitUntil(() => GameManager.Instance.nowPlayer.finishMoving == true); //무빙이 끝났다면 통행료 징수

        if (!GameManager.Instance.nowPlayer.exemptionFlag)
        {
            GameManager.Instance.nowPlayer.playerMoney -= GameManager.Instance.nowPlayer.nowTile.price;
            GameManager.Instance.SetFloatingText(GameManager.Instance.nowPlayer, GameManager.Instance.nowPlayer.nowTile.price, false);
            GameManager.Instance.nowPlayer.againstPlayer.playerMoney += GameManager.Instance.nowPlayer.nowTile.price;
            GameManager.Instance.SetFloatingText(GameManager.Instance.nowPlayer.againstPlayer, GameManager.Instance.nowPlayer.nowTile.price, true);

            GameManager.Instance.NextTurnFunc(); //통신으로 처리하지 않는 이유는 통신을 거치면 돈이 빠져나가기전에 두 클라이언트 모두 턴이 넘어가기 때문에
                                                 //완벽히 처리해주고 각 클라이언트의 턴을 넘기기 위함.
        }
        // 통행료 면제 카드가 있다면 통행료 징수를 하지 않음
        else
        {
            print("exemptionFlag is true!");
            print("Func Start!");
            StartCoroutine(GameManager.Instance.ParticleFunc());
        }
    }
}
