using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using BackEnd;
using UnityEngine.SceneManagement;
using System;
using BackEnd.Tcp;

public class ButtonManager : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] InputField _loginIdInput;
    [SerializeField] InputField loginPwInput;

    [Header("SignUp")]
    [SerializeField] InputField signIdInput;
    [SerializeField] InputField signPwInput;
    [SerializeField] InputField signNickNameInput;
    [SerializeField] InputField signEmailInput;

    [Header("Signup Success UI")]
    public GameObject SignupUI;
    public Text SignupText;
    public Text SignupHeader;
    int result;
    
    [Header("TurnCard")]
    public int turnNum, turncardIdx;

    #region Instance
    private static ButtonManager _instance;
    public static ButtonManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType(typeof(ButtonManager)) as ButtonManager;

            return _instance;
        }
    }
    #endregion

    public async void SignUpBtn() //회원가입 버튼
    {
        AudioManager.Instance.Play("MenuClick_Sound");

        await Task.Run(() =>
        {
            result = BackendLogin.Instance.CustomSignUp(signIdInput.text, signPwInput.text, signNickNameInput.text, signEmailInput.text);
        });
        SignupUI.SetActive(true);
        switch (result)
        {
            case 1:
                SignupHeader.text = "실패";
                SignupText.text = "입력값이 없는곳이 있습니다.";
                break;
            case 2:
                SignupHeader.text = "실패";
                SignupText.text = "이메일 변경 실패.";
                break;
            case 3:
                SignupHeader.text = "실패";
                SignupText.text = "닉네임 변경 실패.";
                break;
            case 4:
                SignupHeader.text = "성공";
                SignupText.text = "회원가입 성공";
                break;
            case 5:
                SignupHeader.text = "실패";
                SignupText.text = "회원가입 실패";
                break;
            case 6:
                SignupHeader.text = "실패";
                SignupText.text = "아이디에 특수문자, 공백이 들어가있습니다.";
                break;
        }
    }

    public async void LoginBtn()
    {
        AudioManager.Instance.Play("MenuClick_Sound");

        await Task.Run(() =>
        {
            BackendLogin.Instance.CustomLogin(_loginIdInput.text, loginPwInput.text);
        });

        if (Backend.IsLogin)
        {
            MatchManager.Instance.Join(); //로그인 후 매칭서버 접속 
            Backend.Match.OnJoinMatchMakingServer = (JoinChannelEventArgs args) =>
            {
                SceneManager.LoadScene("LobbyScene");
            };
        }
    }

    public void BackTitleBtn()
    {
        AudioManager.Instance.Play("MenuClick_Sound");
        Backend.BMember.Logout();
        Backend.Match.LeaveMatchMakingServer();
        SceneManager.LoadScene("TitleScene");

    }
    
    //대기방 생성 매칭서버 접속은 여기서 하지 않고, 로그인 시 자동으로 이뤄져야함.
    public void MatchingBtn()
    {
        AudioManager.Instance.Play("MenuClick_Sound");

        MenuSceneManager.Instance.coroFlag = true;
        StartCoroutine(MenuSceneManager.Instance.TimerCoroutine());

        MatchManager.Instance.CreateMatchingRoom();
        //매칭룸을 생성했을때 호출되는 이벤트
        Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) =>
        { 
            if (args.ErrInfo == ErrorCode.Success)
            {
                RequestMatchMaking();
            }
        };
    }

    //매칭취소 
    public void MatchingCancelBtn()
    {
        AudioManager.Instance.Play("MenuClick_Sound");

        Backend.Match.CancelMatchMaking();
        Backend.Match.LeaveMatchRoom();
        //타이머 코루틴 해제 후 텍스트 초기화
        MenuSceneManager.Instance.coroFlag = false;
        MenuSceneManager.Instance.timerText.text = "00:00";
    }

    //로비로 돌아가기 (인게임에서 타이틀로 가기)
    public void GoToLobby()
    {
        Backend.Match.LeaveMatchRoom(); //대기방을 떠나는 함수
    }

    //게임 시작
    public void RequestMatchMaking()
    {
        MatchManager.Instance.RequestMatchMaking();
    }

    //게임 종료
    public void GameExitBtn()
    {
        //게임 종료 시, 게임방에서 나가고, 게임 종료에 따른 결과 처리 후, 메뉴씬으로 나가야한다.
        Backend.Match.LeaveMatchRoom();
        Backend.Match.LeaveGameServer();

        AudioManager.Instance.Stop("MainGame_Sound");
        AudioManager.Instance.Play("Title_Sound");
        SceneManager.LoadScene("LobbyScene");
    }

    //게임시작때, 자기 턴을 클릭하는 버튼
    public void TurnCardClick()
    {
        AudioManager.Instance.Play("UseCard_Sound");

        if (GameManager.Instance.turnIndex == -1)
        { //묶어주지 않으면 계속 선택이 가능함...
            //턴 인덱스가 0이면 아직 내 턴카드를 선택하지 않은것.
            GameManager.Instance.turnIndex = turnNum; //나의 턴을 저장.

            if (GameManager.Instance.turnIndex == 1)
            {
                GameManager.Instance.myCharactor = GameObject.Find("Player1").GetComponent<PlayerManager>();
            }
            else
            {
                GameManager.Instance.myCharactor = GameObject.Find("Player2").GetComponent<PlayerManager>();
            }

            TurnCard tCard = new(turncardIdx);
            string jsonData = JsonUtility.ToJson(tCard);
            byte[] data;
            data = ParsingManager.Instance.ParsingSendData(ParsingType.Turn, jsonData);
            Backend.Match.SendDataToInGameRoom(data);

            this.gameObject.SetActive(false);
        }
        GameManager.Instance.myCharactor.myTurnImg.SetActive(true);
    }
}