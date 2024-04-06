using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using BackEnd.Tcp;

public class MenuSceneManager : MonoBehaviour
{
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
    [Header("MenuText")]
    [SerializeField] Text nickname;
    
    [Header("Invite Popup")]
    [SerializeField] GameObject invitePopup; //초대시 띄울 팝업오브젝트
    [SerializeField] Text inviteInfoText; //초대 팝업 오브젝트에서 누가 초대했는지 알려주는 텍스트

    public bool coroFlag = false; //수정 필요


    //log Text
    [SerializeField] Text matchingLogText;
    public string matchingLogStr = "";

    //Timer
    public Text timerText;

    private void Awake()
    { 
        //닉네임 받아오기
        UserData user = BackendGameData.Instance.GameDataGet();
        nickname.text = user.nickName;
    }

    public IEnumerator TimerCoroutine()
    {
        float sec = 0, min = 0;
        while (coroFlag)
        {
            sec += 0.01f;
            if (sec > 59f){ min += 1f; sec = 0f; }
            timerText.text = min.ToString("00") + ":" + sec.ToString("00");
            yield return new WaitForSeconds(0.01f);
        }
    }
}
