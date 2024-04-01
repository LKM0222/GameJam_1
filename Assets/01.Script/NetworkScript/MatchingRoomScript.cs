using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using BackEnd.Tcp;
using System.Linq;

public class MatchingRoomScript : MonoBehaviour
{
    [SerializeField] Text roomName;
    [SerializeField] Text userListText;
    [SerializeField] Text MatchingRoomLogText;

    public List<string> userList = new List<string>();
    public string matchingRoomLogStr = "";
    public string matchingRoomUserList = "참여한 유저 : ";

    private static MatchingRoomScript _instance;


    public static MatchingRoomScript Instance{
        get {
            if(_instance == null){
                _instance = FindObjectOfType(typeof(MatchingRoomScript)) as MatchingRoomScript;
            }
            return _instance;
        }
    }
    private void Awake() {
        Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) => { //매칭룸을 생성했을때 호출되는 이벤트
            Debug.Log("MatchRoom Create1");
            if(matchingRoomUserList != null){
                userList.Add(BackendGameData.userData.nickName);
                matchingRoomUserList += BackendGameData.userData.nickName + "\n"; //여기다가 방을 생성한 사람의 이름을 넣어야됨...
            } else {
                Debug.Log("matchingRoomUserList is null");
            }
            if(matchingRoomLogStr != null){
                matchingRoomLogStr += "방을 생성하였습니다. \n";
            } else {
                Debug.Log("matchingRoomLogStr is null");
            }
        };
    }
    
    private void Update() {
        Backend.Match.OnMatchMakingRoomJoin = (MatchMakingGamerInfoInRoomEventArgs args) => {// 초대받은 유저가 방에 입장했을때, 방에 있는 플레이어들에게 호출되는 이벤트
            Debug.Log(args.UserInfo.m_nickName + "님이 입장하였습니다.");
            matchingRoomLogStr += args.UserInfo.m_nickName + "님이 입장하였습니다. \n";
            //userlist를 다시 작성하기 위한 코드 (사람이 변동될 때, 작성해야함.)
            userList.Add(args.UserInfo.m_nickName);
            matchingRoomUserList = "참여한 유저 : ";
            for(int i = 0;i < userList.Count; i++){
                matchingRoomUserList += userList[i] + ",";
            }
        };
        MatchingRoomLogText.text = matchingRoomLogStr;
        userListText.text = matchingRoomUserList;
    }

    public void UserListRemove(string reaveName){ //유저가 퇴장했을때, 현재 방에 남아있는 유저 리스트 업데이트하는 함수
        userList.Remove(userList.Single(s => s == reaveName));
        matchingRoomUserList = "참여한 유저 : ";
        for(int i = 0;i <userList.Count; i++){
            matchingRoomUserList += userList[i] + ",";
        }
    }
}
