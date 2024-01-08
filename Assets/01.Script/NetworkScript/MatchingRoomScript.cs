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
    [SerializeField] Text userListText;
    [SerializeField] Text MatchingRoomLogText;

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
                matchingRoomUserList += BackendGameData.userData.nickname + "\n"; //여기다가 방을 생성한 사람의 이름을 넣어야됨...
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
            matchingRoomUserList += "," + args.UserInfo.m_nickName;
        };
        MatchingRoomLogText.text = matchingRoomLogStr;
        userListText.text = matchingRoomUserList;
    }

    // IEnumerator WaitInfoCoroutine(){
    //     print("waitcoroutine start!");
    //     yield return new WaitUntil(() => MatchManager.Instance.listIsEmpty == true);
    //     roomMatchCard = MatchManager.Instance.matchCards[0];
    //     print("matchCard is " + roomMatchCard + "and MatchCards Count is " + MatchManager.Instance.matchCards.Count);
    //     roomName.text = "방 이름 : " + roomMatchCard.matchTitle + "\n " + roomMatchCard.matchType;
        
    // }
}
