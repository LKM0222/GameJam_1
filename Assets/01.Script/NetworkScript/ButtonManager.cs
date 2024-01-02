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
    [SerializeField] InputField loginIdInput;
    [SerializeField] InputField loginPwInput;
    
    [Header("SignUp")]
    [SerializeField] InputField signIdInput;
    [SerializeField] InputField signPwInput;
    [SerializeField] InputField signNickNameInput;
    [SerializeField] InputField signEmailInput;

    [Header("Test")]
    [SerializeField] Text testNickname;
    [SerializeField] bool winBool;

    // List<>

    public async void SignUpBtn(){
        await Task.Run(() => {
            BackendLogin.Instance.CustomSignUp(signIdInput.text, signPwInput.text, signNickNameInput.text, signEmailInput.text);
        });
    }

    public async void LoginBtn(){
        await Task.Run(() => {
            BackendLogin.Instance.CustomLogin(loginIdInput.text, loginPwInput.text);
        });

        if(Backend.IsLogin){
            print("로그인 완료");
            SceneManager.LoadScene("MenuScene");
            // BackendGameData.Instance.GameDataGet(); //게임 데이터를 조회해서 메뉴 씬에서 출력할꺼야 
        }
        else {
            print("로그인 실패");
        }
    }

    public async void UpScore(){
        await Task.Run(() => {
            if(winBool){
                BackendGameData.Instance.UpWinScore();
                BackendGameData.Instance.GameDataUpdate();
                // MenuSceneManager.Instance.SetWinText()//winText를 수정해야함...loseText도 마찬가지.
                print("승리 포인트 1 추가");
            } else {
                BackendGameData.Instance.UpLoseScore();
                BackendGameData.Instance.GameDataUpdate();
                print("패배 포인트 1 추가");
            }
        });
        
    }

    public void BackTitleBtn(){
        SceneManager.LoadScene("TitleScene");
    }

    public void NicknameTest(){
        testNickname.text = "gg \n" + DateTime.Now;
    }
    
    public void MatchingBtn(){
        MatchManager.Instance.JoinMatchMakingServer();
        //방 생성 완료한거 같은데 방 설정 정보 불러와서 진짜 방 생성이 된건지 확인해보자
        print("방 정보는 ? : ");
        SceneManager.LoadScene("MatchingRoom");
        
    }
    

    // IEnumerator MatcingServerCoroutine(){
    //     //근데 일단, 초당 10kb 만큼만 처리된다 했음
    //     yield return new WaitForSeconds(0.1f); //0.1f씩 호출함.
    //     print("코루틴 실행중");
    //     var poll = 0; //지속적으로 호출됨. poll함수는 처리된 이벤트의 갯수를 반환함.
    //     while(poll < 1){
    //         print("poll 수집중");
            
    //     }
    //     print("poll 가져왔고, var poll 에 전달. var poll : " + poll.ToString());
    //     if(poll > 0){//처리될 이벤트가 존재함
    //         print("처리될 이벤트 존재!");
    //         Backend.Match.OnJoinMatchMakingServer = (JoinChannelEventArgs args) => {
    //             print("매칭서버 이벤트 존재! errorInfo : " + args.ErrInfo.ToString() + ", Session Info : " + args.Session.ToString());
    //         };
    //     } else{
    //         print("이벤트가 존재하지 않음");
    //     }
        
    // }
}
