using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using UnityEditor;
using BackEnd.Tcp;

public class MenuSceneManager : MonoBehaviour
{
    [SerializeField] Text nickname;
    // [SerializeField] Text winText;
    // [SerializeField] Text loseText;
    // [SerializeField] Text ratingText;

    [Header("Invite Popup")]
    [SerializeField] GameObject invitePopup; //초대시 띄울 팝업오브젝트
    [SerializeField] Text inviteInfoText; //초대 팝업 오브젝트에서 누가 초대했는지 알려주는 텍스트

    public bool coroFlag = false;

    #region Instance
    private static MenuSceneManager _instance = null;
    public static MenuSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(MenuSceneManager)) as MenuSceneManager;
            }
            return _instance;
        }
    }
    #endregion

    //log Text
    [SerializeField] Text matchingLogText;
    public string matchingLogStr = "";

    //Timer
    public Text timerText;

    private void Awake()
    { //승률 체크하는곳.
        UserData user = BackendGameData.Instance.GameDataGet();
        print("user nickname : " + user.nickname);
        nickname.text = user.nickname;
        // winText.text = user.winscore.ToString();
        // loseText.text = user.losescore.ToString();
        // if(user.winscore + user.losescore == 0){
        //     ratingText.text = + 0 + "%";
        // }else{
        //     ratingText.text = ( user.winscore / (user.winscore + user.losescore)) + "%";
        // }

    }

    void Update()
    {
        // 만약에 초대 핸들러를 함수에서 처리하고, 초대 시 함수를 실행하게 하면 초대가 정상적으로 이뤄지는가?
        // 안됨.. 무조건 Update에서 해야되는지도 잘 모르겠음... 뭔가 더 깔끔한 방향이 있을것 같음...
        //EventManager에서 가져옴.
        Backend.Match.OnMatchMakingRoomSomeoneInvited += (MatchMakingInvitedRoomEventArgs args) =>
        { //다른 유저가 나를 초대했을때 호출되는 이벤트
            //초대한 유저의 정보를 EventManager에 저장. (나중에 다른 곳에서 써야되기 때문에_ButtonManager의 초대 수락 함수)
            Debug.Log("someone Invite me ");
            EventManager.Instance.matchMakingUserInfo = args.InviteUserInfo;
            EventManager.Instance.roomId = args.RoomId;
            EventManager.Instance.roomToken = args.RoomToken;

            //팝업 띄우고, 텍스트 변환
            invitePopup.SetActive(true);
            inviteInfoText.text = EventManager.Instance.matchMakingUserInfo.m_nickName + "님이 게임에 초대하였습니다!";
        };
        // matchingLogText.text = matchingLogStr;
    }



    // public void SetWinText(string _winText){
    //     winText.text = _winText;
    // }
    // public void SetLoseText(string _loseText){
    //     loseText.text = _loseText;
    // }
    // public void SetRatingText(){ //이 함수는 수정 필요함...
    //     UserData user = BackendGameData.Instance.GameDataGet();
    //     if(user.winscore + user.losescore == 0){
    //         ratingText.text = "승률 : " + 0 + "%";
    //     }else{
    //         ratingText.text = "승률 : " + ( user.winscore / (user.winscore + user.losescore)) + "%";
    //     }
    // }

    public IEnumerator TimerCoroutine()
    {
        int sec = 0, min = 0;
        while (true && coroFlag)
        {
            sec += 1;

            if (sec > 59)
            {
                min += 1;
                sec = 0;
            }
            timerText.text = min.ToString("00") + ":" + sec.ToString("00");

            yield return new WaitForSeconds(1f);
        }
    }
}
