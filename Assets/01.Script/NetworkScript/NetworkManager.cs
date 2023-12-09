using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    
    [SerializeField] InputField roomCodeInput;
    [SerializeField] InputField playerNickNameInput;
    // Start is called before the first frame update
    void Start()
    {
        Connected();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ServerConnect
    public void Connected() => PhotonNetwork.ConnectUsingSettings();
    public override void OnConnected() => print("서버 연결 완료");
    public override void OnDisconnected(DisconnectCause cause) => print("연결 종료");

    //RoomCreate
    public void CreateRoom() => CheckCreateRoom();
    public override void OnCreatedRoom() => AfterCreateRoom();
    public override void OnCreateRoomFailed(short returnCode, string message) => print("방 생성 실패");
    void AfterCreateRoom(){
        print("방 생성완료");
        SceneManager.LoadScene("MenuScene");
    }
    void CheckCreateRoom(){
        if(playerNickNameInput.text == ""){
            print("닉네임을 입력해야합니다.");
        }
        else{
            print("닉네임이 있음.");
            PhotonNetwork.LocalPlayer.NickName = playerNickNameInput.text;
            PhotonNetwork.CreateRoom("100",new RoomOptions { MaxPlayers = 2 });
        }
    }

    //JoinRoom
    public void JoinRoom() => CheckJoinRoom();
    public override void OnJoinedRoom() => AfterJoinRoom();
    public override void OnJoinRoomFailed(short returnCode, string message) => print("방 참가 실패");
    void AfterJoinRoom(){
        print("방 참가완료" + PhotonNetwork.CurrentRoom.Name);
        SceneManager.LoadScene("MenuScene");
    }
    void CheckJoinRoom(){
        if(playerNickNameInput.text == "" && roomCodeInput.text == ""){
            print("InputField를 입력해야합니다.");
        }
        else{
            print("닉네임이 있음.");
            PhotonNetwork.LocalPlayer.NickName = playerNickNameInput.text;
            PhotonNetwork.JoinRoom(roomCodeInput.text);
        }
    }


}
