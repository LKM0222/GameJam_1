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

    [Header("Test")]
    [SerializeField] Text testNickname;
    [SerializeField] bool winBool;

    [Header("InviteUserBtn")]
    [SerializeField] InputField inviteUserNicknameInput;
    [SerializeField] GameObject inviteResultPopup;
    [SerializeField] Text inviteResultText;

    // UI

    [Header("Signup Success UI")]
    public GameObject SignupUI;
    public Text SignupText;
    public Text SignupHeader;

    int result;


    [Header("TurnCard")]
    public int turnNum, turncardIdx;

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


    public async void SignUpBtn()
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
            print("로그인 완료");
            MatchTestManager.Instance.Join(); //로그인 후 매칭서버 접속 
            Backend.Match.OnJoinMatchMakingServer = (JoinChannelEventArgs args) =>
            {
                Debug.Log("Login Result : " + args.ErrInfo);
                SceneManager.LoadScene("LobbyScene"); //매칭서버에 접속 완료했을 때 LobbyScene으로 넘어가야함.
            };
        }
        else
        {
            print("로그인 실패");
        }
    }

    public void UpScore(bool winFlag)
    {
        if (winBool)
        {
            BackendGameData.Instance.UpWinScore();
            BackendGameData.Instance.GameDataUpdate();
            // MenuSceneManager.Instance.SetWinText()//winText를 수정해야함...loseText도 마찬가지.
            print("승리 포인트 1 추가");
        }
        else
        {
            BackendGameData.Instance.UpLoseScore();
            BackendGameData.Instance.GameDataUpdate();
            print("패배 포인트 1 추가");
        }
    }

    public void BackTitleBtn()
    {
        AudioManager.Instance.Play("MenuClick_Sound");
        Backend.BMember.Logout();
        Backend.Match.LeaveMatchMakingServer();
        SceneManager.LoadScene("TitleScene");

    }

    public void NicknameTest()
    {
        testNickname.text = "gg \n" + DateTime.Now;
    }


    //대기방 생성 매칭서버 접속은 여기서 하지 않고, 로그인 시 자동으로 이뤄져야함.
    public void MatchingBtn()
    {
        AudioManager.Instance.Play("MenuClick_Sound");

        MenuSceneManager.Instance.coroFlag = true;
        StartCoroutine(MenuSceneManager.Instance.TimerCoroutine());

        MatchTestManager.Instance.CreateMatchingRoom();
        Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) =>
        { //매칭룸을 생성했을때 호출되는 이벤트
            Debug.Log("MatchRoom Create1");
            if (args.ErrInfo == ErrorCode.Success)
            {
                print("대기방 생성 성공");
                RequestMatchMaking();
            }
        };
    }

    //매칭취소 
    //대기방을 생성했던걸 취소해줘야됨.
    public void MatchingCancelBtn()
    {
        AudioManager.Instance.Play("MenuClick_Sound");

        Backend.Match.CancelMatchMaking();
        Backend.Match.LeaveMatchRoom();
        //타이머 코루틴 해제 후 텍스트 초기화
        MenuSceneManager.Instance.coroFlag = false;
        MenuSceneManager.Instance.timerText.text = "00:00";
    }

    //로비로 돌아가기 (매칭 취소)
    public void GoToLobby()
    {
        Backend.Match.LeaveMatchRoom(); //대기방을 떠나는 함수
    }

    //게임 시작
    public void RequestMatchMaking()
    {
        MatchTestManager.Instance.RequestMatchMaking();
    }

    //게임 종료
    public void GameExitBtn()
    {
        //게임 종료 시, 게임방에서 나가고, 게임 종료에 따른 결과 처리 후, 메뉴씬으로 나가야한다.
        Backend.Match.LeaveMatchRoom();
        Backend.Match.LeaveGameServer();
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
                // SessionData sessionData = new(GameManager.Instance.mySessionId, GameManager.Instance.turnIndex);
                // SessionData sessionData = new(BackendManager.Instance.mySessionId, GameManager.Instance.turnIndex);
                // string jsondata = JsonUtility.ToJson(sessionData);
                // ParsingManager.Instance.ParsingSendData(ParsingType.Session, jsondata);
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

            // this.gameObject.GetComponent<Image>().sprite = GameManager.Instance.turnCardImage[1];
        }
        GameManager.Instance.myCharactor.myTurnImg.SetActive(true);
    }

}