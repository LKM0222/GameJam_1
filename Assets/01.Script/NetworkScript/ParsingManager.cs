using System.Collections;
using System.Collections.Generic;
using System.Text;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine;
using UnityEngine.Tilemaps;

#region ParsingClass
public class ParsingData{
    public ParsingType type;
    public string data;
    //data는 type에 맞춰서 각 클래스를 data로 파싱.

    public ParsingData(ParsingType _type, string _data){
        type = _type;
        data = _data;
    }
}

public class TurnCardSet{
    public int randomNum;

    public TurnCardSet(int _randomNum){
        randomNum = _randomNum;
    }
}

public class TurnCard{
    // public int turnNum;
    public int turncardIdx;

    public TurnCard(int _turnCardIdx){
        turncardIdx = _turnCardIdx;
    }
}

public class DiceData{
    public int diceNum; //어느 플레이어든 던진 주사위 값
    public int playerNum; //어떤 플레이어가 주사위를 던진건지

    public DiceData(int _diceNum, int _playerNum){
        diceNum = _diceNum;
        playerNum = _playerNum;
    }
    
}

public class BuildingData{
    public int buildingNum;
    public BuildingData(int _buildingNum){
        buildingNum = _buildingNum;
    }
}

public class TeleportData{
    public bool tpFlag;
    public GameObject tpTile;
    
    public TeleportData(bool _tpFlag,GameObject _tpTile){
        tpFlag = _tpFlag;
        tpTile = _tpTile;
    }
}



#endregion

#region Enum_ParsingType
public enum ParsingType{
    TurnCardSet,
    Turn,
    Dice,
    NextTurn,
    GroundBuy,
    BuildingBuy,
    Teleport
}
#endregion


//데이터가 송신될때의 클래스 및 데이터 파싱형태를 지정합니다.

public class ParsingManager : MonoBehaviour
{
    #region Instance_Parsing
    private static ParsingManager _instance;

    public static ParsingManager Instance{
        get{
            if(_instance == null)
                _instance = new ParsingManager(); //FindObjectOfType(typeof(ParsingManager)) as ParsingManager;

            return _instance;
        }
    }
    #endregion

    public byte[] ParsingSendData(ParsingType _type, string _jsonData){
        //전달하는 함수. 전달값은 byte[]로 전달.
        //클래스를 선언한 다음 이 함수 사용
        //string jsonData = JsonUtility.ToJson(data);로 바뀐 데이터를 전달.
        ParsingData data = new(_type, _jsonData);
        string jsonData = JsonUtility.ToJson(data);
        print("ParsingSendData is " + jsonData);
        return Encoding.UTF8.GetBytes(jsonData); //반환값을 Backend.Match.SendDataToinGameRoom으로 전달.
    }




}


/*
public void ParisngRecvData(MatchRelayEventArgs args){
        //받는 함수(받는 데이터는 byte[]로 받음.
        //수신이벤트에서 각 클래스로 변환하는 함수.
        print("Recv!");
        byte[] data = args.BinaryUserData;
        ParsingData pData = JsonUtility.FromJson<ParsingData>(Encoding.Default.GetString(data));
        //pData.type : 데이터의 타입, pData.data : string데이터 (클래스별 데이터라 각 클래스에 맞는 파싱과정 필요)
        //데이터의 타입으로 스위치문 결정, 데이터를 다시 위와 같은 과정으로 알맞은 클래스로 변환 후 사용.
        switch(pData.type){
            case ParsingType.Turn:
                print("turn case");
                TurnCard tData = JsonUtility.FromJson<TurnCard>(pData.data);
                GameManager.Instance.playerCount.Add(1);
                GameManager.Instance.turnCards[tData.turncardIdx].SetActive(false); 
                if(GameManager.Instance.playerCount.Count > 1){
                    GameManager.Instance.turnCardParent.SetActive(false);
                }
            break;
        }
    }*/

