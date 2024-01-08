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
                matchingRoomUserList += "내가 생성함!";
            } else {
                Debug.Log("matchingRoomUserList is null");
            }
            if(matchingRoomLogStr != null){
                matchingRoomLogStr += "방을 생성하였습니다.";
            } else {
                Debug.Log("matchingRoomLogStr is null");
            }
        };
    }
    
    private void Update() {
        Backend.Match.OnMatchMakingRoomJoin = (MatchMakingGamerInfoInRoomEventArgs args) => {
            //유저가 입장했을때, 대기방에 있는 모든 유저들에게 호출되는 이벤트 입장한 유저에게도 호출됨.
            //대기방을 생성했을땐 호출 안됨.
            // matchingRoomLogStr += args.UserInfo.m_nickName + " 님이 입장하였습니다.\n";
            // MatchingRoomLogText.text = matchingRoomLogStr;
            // userListText.text += args.UserInfo.m_nickName;
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
