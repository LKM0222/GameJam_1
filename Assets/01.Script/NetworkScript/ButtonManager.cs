using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using BackEnd;
using UnityEngine.SceneManagement;
using System;
using BackEnd.Tcp;
using System.Text.RegularExpressions;

public class ButtonManager : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] InputField loginIdInput;
    [SerializeField] InputField loginPwInput;

    [Header("SignUp")]
    [SerializeField] InputField signIdInput;
    [SerializeField] InputField signPwInput;
    [SerializeField] InputField signNickNameInput;
    [SerializeField] InputField signEmailInput;

    [Header("Test")]
    [SerializeField] Text testNickname;
    [SerializeField] bool winBool;

    [Header("InviteUserBtn")]
    [SerializeField] InputField inviteUserNicknameInput;
    [SerializeField] GameObject inviteResultPopup;
    [SerializeField] Text inviteResultText;

    // List<>

    public async void SignUpBtn(){
        await Task.Run(() => {
            BackendLogin.Instance.CustomSignUp(signIdInput.text, signPwInput.text, signNickNameInput.text, signEmailInput.text);
        });
    }

    public async void LoginBtn(){
        await Task.Run(() => {
            BackendLogin.Instance.CustomLogin(loginIdInput.text, loginPwInput.text);
        });

        if(Backend.IsLogin){
            print("로그인 완료");
            MatchTestManager.Instance.Join(); //로그인 후 매칭서버 접속 
            Backend.Match.OnJoinMatchMakingServer = (JoinChannelEventArgs args) => {
                Debug.Log("Login Result : " + args.ErrInfo); 
                SceneManager.LoadScene("MenuScene"); //매칭서버에 접속 완료했을 때 MenuScene으로 넘어가야함.
            };
        }
        else {
            print("로그인 실패");
        }
    }

    public async void UpScore(){
        await Task.Run(() => {
            if(winBool){
                BackendGameData.Instance.UpWinScore();
                BackendGameData.Instance.GameDataUpdate();
                // MenuSceneManager.Instance.SetWinText()//winText를 수정해야함...loseText도 마찬가지.
                print("승리 포인트 1 추가");
            } else {
                BackendGameData.Instance.UpLoseScore();
                BackendGameData.Instance.GameDataUpdate();
                print("패배 포인트 1 추가");
            }
        });
        
    }

    public void BackTitleBtn(){
        SceneManager.LoadScene("TitleScene");
    }

    public void NicknameTest(){
        testNickname.text = "gg \n" + DateTime.Now;
    }
    
    // public void MatchingBtn(){
    //     MatchManager.Instance.JoinMatchMakingServer();
    //     //방 생성 완료한거 같은데 방 설정 정보 불러와서 진짜 방 생성이 된건지 확인해보자
    //     print("방 정보는 ? : ");
    //     SceneManager.LoadScene("MatchingRoom");
        
    // }

    //대기방 생성 매칭서버 접속은 여기서 하지 않고, 로그인 시 자동으로 이뤄져야함.
    public void MatchingBtn(){
        MatchTestManager.Instance.CreateMatchingRoom();
        Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) => { //매칭룸을 생성했을때 호출되는 이벤트
            Debug.Log("MatchRoom Create1");
            // if(MatchingRoomScript.Instance.userListText != null){
            //     MatchingRoomScript.Instance.userListText.text += "내가 생성함!";
            // } else {
            //     Debug.Log("MatchingRoomScript.Instance.userListText is null");
            // }
            // if(MatchingRoomScript.Instance.MatchingRoomLogText != null){
            //     MatchingRoomScript.Instance.MatchingRoomLogText.text += "방을 생성하였습니다.";
            // } else {
            //     Debug.Log("MatchingRoomScript.Instance.MatchingRoomLogText is null");
            // }
        };
    }

    //대기방 참여
    public void VisitMatchingRoomBtn(){

    }

    //대기방에 유저 초대
    public void InviteUserBtn(){
        if(inviteUserNicknameInput.text == ""){
            Debug.Log("닉네임이 비었습니다.");
        }
        else{
            Backend.Match.InviteUser(inviteUserNicknameInput.text); //유저 초대 함수
            inviteResultPopup.SetActive(true);
            Backend.Match.OnMatchMakingRoomInvite = (MatchMakingInteractionEventArgs args) => { //유저 초대 시 발생하는 이벤트처리.
                switch(args.ErrInfo){//에러코드 잘 읽어보면 무슨 케이스인지 나옴!
                    case ErrorCode.Match_Making_NotJoinedRoom:
                        inviteResultText.text = "대기방을 만들지 않고는 초대 할 수 없습니다.\n";
                        break;
                    case ErrorCode.Match_Making_NotFoundGamer:
                        inviteResultText.text = "초대할 유저가 서버에 접속해있지 않습니다.\n";
                        break;
                    case ErrorCode.Match_Making_AlreadyJoinedRoom:
                        inviteResultText.text = "초대할 유저가 이미 다른방에 들어가있습니다.\n";
                        break;
                    default:
                        inviteResultText.text = inviteUserNicknameInput.text + "를 초대하였습니다.";
                        //초대했으니 핸들러 호출. 여기서 집중해야할것은, 핸들러가 호출되더라도, 초대받은 사람만 팝업이 떠야 함. 안됨 ㅋㅋ
                        // MenuSceneManager.Instance.InviteReceve();
                        break;

                }
            };
            
        }
    }

    //유저 초대 수락
    public void AcceptInviteBtn(){
        Backend.Match.AcceptInvitation(EventManager.Instance.roomId, EventManager.Instance.roomToken);
        //여기서 씬을 불러와야될까?
        SceneManager.LoadScene("MatchingRoom");
        EventManager.Instance.acceptFlag = true;
        Backend.Match.OnMatchMakingRoomUserList = (MatchMakingGamerInfoListInRoomEventArgs args) => { //초대받은 유저가 대기방에 입장했을 때, 호출되는 이벤트
            Debug.Log("유저가 참여하였습니다!" + args.UserInfos.Count); //여긴 정상적으로 잘 불러와짐. 초대를 받았을때, 인원이 몇명인지 일단 체크
            //씬 불러온 뒤, 
            
            //굳이 함수를 호출해서 바꿀 필요가 있나?
            for(int i=0; i < args.UserInfos.Count; i++){ //현재 입장 유저 리스트 업데이트
                Debug.Log("add userList : " + args.UserInfos[i].m_nickName);
                // MatchingRoomScript.Instance.userListText.text += args.UserInfos[i].m_nickName;
                MatchingRoomScript.Instance.matchingRoomUserList += args.UserInfos[i].m_nickName;
            }
            MatchingRoomScript.Instance.matchingRoomLogStr += //유저가 들어오면서 로그 생성
                args.UserInfos[args.UserInfos.Count - 1].m_nickName + "님이 입장하였습니다. \n";

            //여기까지하면, 초대받은 맴버는 매칭룸에 입장했을 때, 로그와 매칭룸의 인원이 표시될것임... 아마도...
        };
    }
    //유저 초대 거절
    public void DenyInviteBtn(){
        Backend.Match.DeclineInvitation(EventManager.Instance.roomId, EventManager.Instance.roomToken);
    }
    

    // IEnumerator MatcingServerCoroutine(){
    //     //근데 일단, 초당 10kb 만큼만 처리된다 했음
    //     yield return new WaitForSeconds(0.1f); //0.1f씩 호출함.
    //     print("코루틴 실행중");
    //     var poll = 0; //지속적으로 호출됨. poll함수는 처리된 이벤트의 갯수를 반환함.
    //     while(poll < 1){
    //         print("poll 수집중");
            
    //     }
    //     print("poll 가져왔고, var poll 에 전달. var poll : " + poll.ToString());
    //     if(poll > 0){//처리될 이벤트가 존재함
    //         print("처리될 이벤트 존재!");
    //         Backend.Match.OnJoinMatchMakingServer = (JoinChannelEventArgs args) => {
    //             print("매칭서버 이벤트 존재! errorInfo : " + args.ErrInfo.ToString() + ", Session Info : " + args.Session.ToString());
    //         };
    //     } else{
    //         print("이벤트가 존재하지 않음");
    //     }
        
    // }
}
