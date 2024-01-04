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


    private static MatchingRoomScript _instance;
    List<string> userList = new List<string>();
    public static MatchingRoomScript Instance{
        get {
            if(_instance == null){
                _instance = new MatchingRoomScript();
            }
            return _instance;
        }
    }
    

    
    private void Update() {
        //플레이어가 들어올때마다 플레이어 목록을 추가시켜줘야함.
        Backend.Match.OnMatchMakingRoomUserList = (MatchMakingGamerInfoListInRoomEventArgs args) => {
            string u_str = "";
            for(int i = 0; i < args.UserInfos.Count; i++){
                u_str += args.UserInfos[i].m_nickName + " , ";
            }
            userListText.text = u_str;
        };
    }

    // IEnumerator WaitInfoCoroutine(){
    //     print("waitcoroutine start!");
    //     yield return new WaitUntil(() => MatchManager.Instance.listIsEmpty == true);
    //     roomMatchCard = MatchManager.Instance.matchCards[0];
    //     print("matchCard is " + roomMatchCard + "and MatchCards Count is " + MatchManager.Instance.matchCards.Count);
    //     roomName.text = "방 이름 : " + roomMatchCard.matchTitle + "\n " + roomMatchCard.matchType;
        
    // }
}
