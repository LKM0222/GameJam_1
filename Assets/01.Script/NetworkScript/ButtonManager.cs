using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using BackEnd;
using UnityEngine.SceneManagement;
using System;

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

    public void BackTitleBtn(){
        SceneManager.LoadScene("TitleScene");
    }

    public void NicknameTest(){
        testNickname.text = "gg \n" + DateTime.Now;
    }
    
    // public void MatchingBtn(){
    //     ErrorInfo errorInfo = null;
    //     Error
    //     if(Backend.Match.JoinMatchMakingServer(errorInfo)){

    //     }
    // }
    
}
