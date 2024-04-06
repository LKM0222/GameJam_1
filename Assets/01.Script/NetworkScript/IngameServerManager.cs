using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;

public class IngameServerManager : MonoBehaviour
{
    private static IngameServerManager _instance;

    public static IngameServerManager Instance{
        get{
            if(_instance == null){
                _instance = FindObjectOfType(typeof(IngameServerManager)) as IngameServerManager;
            }
            return _instance;
        }
    }
    
    public void JoinGameServer(string _serverAdrr, ushort _serverPort){ //게임서버에 접속하기 위한 함수.
        bool isReconnect = true;
        ErrorInfo errorInfo = null;
        //반환값은 true, false로 성공확인은 OnSessionJoinInServer 이벤트 핸들러에서 확인
        Backend.Match.JoinGameServer(_serverAdrr, _serverPort, isReconnect, out errorInfo); 
    }
}