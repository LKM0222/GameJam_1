using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class EventManager : MonoBehaviour
{
    public static EventManager Instance = null;

    [Header("Invite Info")]
    public MatchMakingUserInfo matchMakingUserInfo;
    public SessionId roomId;
    public string roomToken;
    
    private void Awake() {
        if(Instance == null){
            Instance = new EventManager();
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) => { //매칭룸을 생성했을때 호출되는 이벤트
            Debug.Log("MatchRoom Create");
        };

        //초대 수락, 거절 시 발생하는 이벤트
        Backend.Match.OnMatchMakingRoomInviteResponse = (MatchMakingInteractionEventArgs args) => {
            //수락 시 자동으로 대기방에 접속됨.
            //수락하면 이제 씬도 불러와야제?? 근데 수락했을때 씬을 불러오고, 거절했을땐 씬을 불러오지 않아야 함...
            //이 기준은 어디서..?
            Debug.Log("초대 수락/ 거절 결과 : " + args.ErrInfo);
        };

    }
}
