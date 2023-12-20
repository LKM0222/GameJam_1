using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using BackEnd;
using UnityEngine.SceneManagement;

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
        }
        else {
            print("로그인 실패");
        }
    }
    
}
