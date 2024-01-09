using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
public class EventManager : MonoBehaviour
{
    public static EventManager Instance = null;

    [Header("Invite Info")]
    public MatchMakingUserInfo matchMakingUserInfo;
    public SessionId roomId;
    public string roomToken;
    public bool acceptFlag = false;
    
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
                
            }
            if(args.ErrInfo == ErrorCode.InvalidOperation){//매칭중이라 퇴장 실패
                Debug.Log("매칭중이라 방에서 나갈 수 없습니다.");
            }
            if(args.ErrInfo == ErrorCode.Match_Making_KickedByOwner){ //강퇴당했을경우

            }
        };
        
        Backend.Match.OnMatchMakingResponse = (MatchMakingResponseEventArgs args) => {
            // 유저가 매칭을 신청, 취소 했을 때 그리고 매칭이 성사되었을 때 호출되는 이벤트
            switch(args.ErrInfo){
                case ErrorCode.Match_InProgress: //매칭신청에 성공하였을때
                    Debug.Log("매칭신청 성공");
                    break;

                case ErrorCode.Success: //매칭이 성사되었을 떄 여기서 인게임 서버 접속시도
                    Debug.Log("매칭 성사");
                    
                    break;

                case ErrorCode.Match_MatchMakingCanceled: 
                    //매칭 신청을 취소했을때
                    //매칭이 성사되었으나 서버에서 인게임 룸 생성에 실패했을 경우(다시 매칭을 신청해야 합니다.)
                    Debug.Log(args.Reason);
                    break;
                
                case ErrorCode.Match_InvalidModeType: //잘못된 모드 타입으로 신청했을 때
                    Debug.Log("잘못된 모드타입 입니다.");
                    break;

                case ErrorCode.Match_Making_InvalidRoom: //대기방에 허용된 인원보다 많은 인원이 존재하는 경우
                    Debug.Log("대기방 안에 허용된 인원보다 많은 인원이 존재합니다." + args.Reason);
                    break;


                case ErrorCode.InvalidOperation: 
                    // 잘못된 요청을 했을 때, 매치를 두번 이상 신청했을 때, 방장이 아닌 유저가 매칭 신청을 했을 때, 
                    // 방장이 아닌 유저가 매칭을 취소했을 때.
                    // 대기방에 2명 이상의 유저가 존재하는데 1:1, 개인전 매칭을 신청한 경우
                    Debug.Log("잘못된 요청입니다." + args.Reason);
                    break;
            }
        };

    }
}
