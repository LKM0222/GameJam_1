using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using BackEnd.Tcp;
using Unity.VisualScripting;
using System;
 
public class MatchingRoomScript : MonoBehaviour
{
    [SerializeField] Text roomName;
    public Text userListText;
    public Text MatchingRoomLogText;

    string matchingRoomLogStr = "";

    private static MatchingRoomScript _instance;


    public static MatchingRoomScript Instance{
        get {
            if(_instance == null){
                _instance = new MatchingRoomScript();
            }
            return _instance;
        }
    }
    
    // public void SetUserListText(List<MatchMakingUserInfo> userInfos){
    //     Debug.Log("SetUserListText Start" + userInfos);
    //     string u_str = "참여한 유저 : ";
    //     for(int i = 0; i < userInfos.Count; i++){
    //         u_str += userInfos[i].m_nickName + " , ";
    //     }
    //     print("SetUserListText" + userListText.text);
    //     userListText.text = u_str;
    // }

    // public void SetMatchingRoomLog(string user_name){ //닉네임이 전달되면 로그에 추가.
    //     Debug.Log("SetMatchingRoomLog Start");
    //     matchingRoomLogStr += user_name + " 님이 입장하였습니다.\n";
    //     MatchingRoomLogText.text = matchingRoomLogStr;
    // }
    
    private void Update() {
        Backend.Match.OnMatchMakingRoomJoin = (MatchMakingGamerInfoInRoomEventArgs args) => {
            //유저가 입장했을때, 대기방에 있는 모든 유저들에게 호출되는 이벤트 입장한 유저에게도 호출됨.
            //대기방을 생성했을땐 호출 안됨.
            matchingRoomLogStr += args.UserInfo.m_nickName + " 님이 입장하였습니다.\n";
            MatchingRoomLogText.text = matchingRoomLogStr;
            userListText.text += args.UserInfo.m_nickName;
        };
        // Backend.Match.OnMatchMakingRoomUserList = (MatchMakingGamerInfoListInRoomEventArgs args) => {
        //     //유저가 초대를 수락하면 호출되는 이벤트
        //     //입장하는 유저에게만 호출됨. args는 현재 대기방 유저 정보 args. UserInfos
        //     Debug.Log("OnMatchMakingRoomUserList"+ args.UserInfos);
        //     for(int i = 0;i< args.UserInfos.Count; i++){
        //         userListText.text += args.UserInfos[i].m_nickName;
        //     }
        // };
    }

    // IEnumerator WaitInfoCoroutine(){
    //     print("waitcoroutine start!");
    //     yield return new WaitUntil(() => MatchManager.Instance.listIsEmpty == true);
    //     roomMatchCard = MatchManager.Instance.matchCards[0];
    //     print("matchCard is " + roomMatchCard + "and MatchCards Count is " + MatchManager.Instance.matchCards.Count);
    //     roomName.text = "방 이름 : " + roomMatchCard.matchTitle + "\n " + roomMatchCard.matchType;
        
    // }
}
