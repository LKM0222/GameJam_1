using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    NetworkManager instance;
    [SerializeField] Text stateText;
    //CreateUI
    [SerializeField] InputField CreateNickname;
    //JoinUI
    [SerializeField] InputField joinNickName, roomCodeInput;


    private void Awake()
    {
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "TitleScene")
            stateText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    //Server Connect
    void Connect(){
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnected() => print("connect");
    public override void OnDisconnected(DisconnectCause cause) => print("disconnected");

    //Create Room
    public void CreateRoom(){
        if(CreateNickname.text == ""){
            print("닉네임을 입력해야 합니다.");
        }
        else{
            PhotonNetwork.LocalPlayer.NickName = CreateNickname.text;
            PhotonNetwork.CreateRoom("100", new RoomOptions{MaxPlayers = 2});
            print("방 생성 완료");
            SceneManager.LoadScene("MenuScene");
        }
    }
    public override void OnCreatedRoom(){
        print("OnCreatedRoom");

        var player = PhotonNetwork.Instantiate("Player",Vector3.zero, Quaternion.identity);
        print(player);
        DontDestroyOnLoad(player);
    }
    public override void OnCreateRoomFailed(short returnCode, string message) => print("CreateRoomFailed");


    //JoinRoom
    public void JoinRoom(){
        if(joinNickName.text == "" && roomCodeInput.text == ""){
            print("input이 비었습니다.");
        }
        else{
            PhotonNetwork.LocalPlayer.NickName = joinNickName.text;
            PhotonNetwork.JoinRoom(roomCodeInput.text);
            
        }
    }
    public override void OnJoinedRoom(){
        print("OnJoinedRoom");
        var player = PhotonNetwork.Instantiate("Player",Vector3.zero,Quaternion.identity);
        print(player);
        DontDestroyOnLoad(player);
        SceneManager.LoadScene("MenuScene");
    }
    public override void OnJoinRoomFailed(short returnCode, string message) => print("방 입장 실패");
}
