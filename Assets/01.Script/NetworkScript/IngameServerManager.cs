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
        if(Backend.Match.JoinGameServer(_serverAdrr, _serverPort, isReconnect, out errorInfo) == false){ //인게임 서버에 접속 시도.
            //리턴이 true라면 서버 연결이 된것이 아닌, 소켓 연결만 된것.
            //인게임 서버 접속 성공확인은 OnSessionJoinInServer 이벤트 핸들러에서 확인. (핸들러가 정상적으로 호출되어야 서버에 접속한것.)
            //에러정보
            Debug.Log(errorInfo);
            return;
        }

    }
}
