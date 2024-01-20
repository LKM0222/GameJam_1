using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Unity.VisualScripting;
public class EventManager : MonoBehaviour
{
    public static EventManager Instance = null;

    [Header("Invite Info")]
    public MatchMakingUserInfo matchMakingUserInfo;
    public SessionId roomId;
    public string roomToken;
    public bool acceptFlag = false;

    [Header("InGameServer")]
    MatchInGameRoomInfo roomInfo; //인게임에서 방 정보를 전달하기위해 선언해둔 변수
    
    private void Awake() {
        if(Instance == null){
            Instance = FindObjectOfType(typeof(EventManager)) as EventManager;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Backend.Match.OnMatchMakingRoomLeave = (MatchMakingGamerInfoInRoomEventArgs args) => {
            // 대기방을 떠나면서 실행되는 핸들러
            // 대기방을 떠날때, 유저 리스트와 대기방을 나갔다는 로그가 필요함.
            // 유저 리스트는 좀만 있다가 수정하자. 대기방 나갔다는 로그부터
            if(args.ErrInfo == ErrorCode.Success){//성공적으로 퇴장 성공
                MatchingRoomScript.Instance.matchingRoomLogStr += args.UserInfo.m_nickName + 
                "님이 퇴장하였습니다. \n";
                MatchingRoomScript.Instance.UserListRemove(args.UserInfo.m_nickName);
            }
            if(args.ErrInfo == ErrorCode.InvalidOperation){//매칭중이라 퇴장 실패
                Debug.Log("매칭중이라 방에서 나갈 수 없습니다.");
            }
            if(args.ErrInfo == ErrorCode.Match_Making_KickedByOwner){ //강퇴당했을경우

            }
        };
        
        //매칭신청(인게임서버접속 시작)
        Backend.Match.OnMatchMakingResponse = (MatchMakingResponseEventArgs args) => {
            // 유저가 매칭을 신청, 취소 했을 때 그리고 매칭이 성사되었을 때 호출되는 이벤트
            switch(args.ErrInfo){
                case ErrorCode.Match_InProgress: //매칭신청에 성공하였을때
                    Debug.Log("매칭신청 성공");
                    MatchingRoomScript.Instance.matchingRoomLogStr += "매칭신청 성공\n";
                    break;

                case ErrorCode.Success: //매칭이 성사되었을 떄 여기서 인게임 서버 접속시도
                    Debug.Log("매칭 성사 , 인게임 서버에 접속 시도합니다.");
                    MatchingRoomScript.Instance.matchingRoomLogStr += "매칭 성사 , 인게임 서버에 접속 시도합니다.\n";
                    roomInfo = args.RoomInfo; //추후에 roomToken을 써야되기 때문에 따로 저장
                    if(Backend.Match.JoinGameServer(args.RoomInfo.m_inGameServerEndPoint.m_address,
                    args.RoomInfo.m_inGameServerEndPoint.m_port,
                    false, out ErrorInfo errorInfo)==false){
                        //true인 경우, OnSessionJoinInServer 호출.
                        Debug.Log("errorinfo is" + errorInfo.Reason);
                    }
                    else{
                        Debug.Log("OnSessionJoinInServer Start");
                    }
                    break;

                case ErrorCode.Match_MatchMakingCanceled: //매칭 신청을 취소했을때
                    //매칭이 성사되었으나 서버에서 인게임 룸 생성에 실패했을 경우(다시 매칭을 신청해야 합니다.)
                    Debug.Log("서버 인게임 룸 생성 실패 " + args.Reason);
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

        Backend.Match.OnSessionJoinInServer += (args) => { //인게임서버에 접속 성공했을 떄 호출되는 이벤트 이 이벤트가 호출되어야 서버에 접속성공한것.
            if(args.ErrInfo == ErrorInfo.Success){
                Debug.Log("인게임서버 접속 성공" );//+ this.roomInfo.m_inGameRoomToken);
                // MatchingRoomScript.Instance.matchingRoomLogStr += "인게임서버 접속 성공\n"; //주석처리 이유 : 이 구문이 왜 반복적으로 호출되는지 모르겠지만
                //일단 이 구문이 반복호출되면서 text가 200줄을 넘어가기 때문에 일단 비활성화
                Backend.Match.JoinGameRoom(this.roomInfo.m_inGameRoomToken); //OnMatchMakingResponse에서 전달받은 RoomToken을 여기로 전달.
                Debug.Log("게임방에 접속시도합니다.");
                // MatchingRoomScript.Instance.matchingRoomLogStr += "게임방에 접속 시도합니다.\n"; //여기도 마찬가지
                //다른 에러 케이스가 많지만 그건 추후에...
            }
            else{
                switch(args.ErrInfo.Category){
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

        Backend.Match.OnMatchInGameAccess = (MatchInGameSessionEventArgs args) => { //유저가 게임방에 입장할 때마다 호출되는 이벤트입니다.
            // 입장한 유저(자기 자신)에게 호출됩니다.
            // 이미 게임방에 접속해있던 모든 유저에게 호출됩니다.
            if(args.ErrInfo == ErrorCode.Success){
                Debug.Log(args.GameRecord.m_nickname + "접속 완료"); //여기까지 성공.
                MatchingRoomScript.Instance.matchingRoomLogStr += "접속 완료\n";
                SceneManager.LoadScene("TestScene");
                //방에 접속하면 누가 접속완료하였는지 닉네임이 표시된다.
                //이를 활용해 모두 접속 완료라면 씬을 옮겨서도 데이터를 주고받을 수 있을까?
            }
        };

        Backend.Match.OnMatchInGameStart = () => { //게임 시작 이밴트
            // 모든 유저의 데이터를 브로드캐스팅 할 준비가 되었다는 이벤트
            // 모든 유저가 게임방에 접속한 이후 
            // 콘솔에서 설정한 매치 시작 대기시간이 지난 이후에 모든 유저에게 게임 시작 이벤트가 호출
            // 게임 시작 이벤트가 호출된 후 게임에서 필요한 데이터 동기화, 유저들 간 로딩 상황 공유 등 다양한 데이터 설정을 진행
            
            //데이터를 넘길 수는 있지만, 이제부터 누가 선 플레이어고, 게임 진행에 대하여 데이터 전송이 이뤄져야한다....
            Debug.Log("게임 시작! 이제부터 데이터가 모든 유저에게 브로드캐스팅 가능합니다!");
            //이후부터 게임 시작되었다는 뭔가가 필요할듯.
        };

        Backend.Match.OnMatchRelay = (MatchRelayEventArgs args) => { //데이터 수신
            byte[] data = args.BinaryUserData;
            InGameScript.Instance.testStr += Encoding.Default.GetString(data);
        };
    }


}
