using System.Collections;
using System.Collections.Generic;
using System.Text;
using BackEnd.Tcp;
using UnityEngine;

#region ParsingClass
public class ParsingData
{
    public ParsingType type;
    public string data;
    //data는 type에 맞춰서 각 클래스를 data로 파싱.

    public ParsingData(ParsingType _type, string _data)
    {
        type = _type;
        data = _data;
    }
}

public class TurnCardSet
{
    public int randomNum; //턴 카드에 저장되는 턴 정보를 저장할 변수(랜덤된 값이 들어감.)

    public TurnCardSet(int _randomNum)
    {
        randomNum = _randomNum;
    }
}

public class TurnCard
{
    public int turncardIdx; //현재 선택한 턴카드가 어느 위치에 있는지 알려주는 변수 상대방이 어떤 카드를 눌렀는지 전달하고, 내 클라이언트에서도 상대방이 누른 카드의 active를 false로 하기 위한 변수임.

    public TurnCard(int _turnCardIdx)
    {
        turncardIdx = _turnCardIdx;
    }
}

public class DiceData
{
    public int diceNum; //어느 플레이어든 던진 주사위 값
    public int playerNum; //어떤 플레이어가 주사위를 던진건지

    public DiceData(int _diceNum, int _playerNum)
    {
        diceNum = _diceNum;
        playerNum = _playerNum;
    }

}

public class BuildingData
{
    public int buildingNum;
    public BuildingData(int _buildingNum)
    {
        buildingNum = _buildingNum;
    }
}

public class TeleportData
{
    public bool tpFlag;
    public string tpTileNum;

    public TeleportData(bool _tpFlag, string _tpTileNum)
    {
        tpFlag = _tpFlag;
        tpTileNum = _tpTileNum;
    }
}

public class CardClickData
{
    public int cardNum;
    public int playerId;

    public CardClickData(int _cardNum, int _playerId)
    {
        cardNum = _cardNum;
        playerId = _playerId;
    }
}

public class CardData
{
    public Card card;
    public CardData(Card _card)
    {
        card = _card;
    }
}

public class CardDestroyData
{
    public GameObject destoryCard;
    public int cardCode;

    public CardDestroyData(GameObject _destroyCard, int _cardCode)
    {
        destoryCard = _destroyCard;
        cardCode = _cardCode;
    }

}

public class InvisibleData
{
    public int cardNum;

    public InvisibleData(int _cardNum)
    {
        cardNum = _cardNum;
    }
}

public class SessionData
{
    public SessionId sessionId;
    public int turnNum;
    public SessionData(SessionId _id, int _turnNum)
    {
        sessionId = _id;
        turnNum = _turnNum;
    }
}

public class TileSelectData
{
    public string tilename;
    public TileSelectData(string _tilename)
    {
        tilename = _tilename;
    }
}

public class ExtortionData
{
    public int playerId;
    public ExtortionData(int _id)
    {
        playerId = _id;
    }
}

public class VisitData
{
    public int money;
    public int caseNum;

    public VisitData(int _money, int _caseNum)
    {
        money = _money;
        caseNum = _caseNum;
    }
}

public class ArriveTileData
{
    public int playerId;

    public ArriveTileData(int _playerId)
    {
        playerId = _playerId;
    }
}

public class LaserData
{
    public bool laserFlag;
    public string laserTileNum;

    public LaserData(bool _laserFlag, string _laserTileNum)
    {
        laserFlag = _laserFlag;
        laserTileNum = _laserTileNum;
    }
}

public class ExemptionData
{
    public int value;
}


#endregion

#region Enum_ParsingType
public enum ParsingType
{
    TurnCardSet,
    Turn,
    Dice,
    NextTurn,
    GroundBuy,
    BuildingBuy,
    Teleport,
    CardClick,
    Card,
    CardListAdd,
    CardDestory,
    InvisibleThief,
    ExemptionFlag,
    ExemptionFlagSet,
    Session,
    TileSelect,
    Extortion,
    Visit,
    ArriveTile,
    Olympic,
    Laser
}
#endregion


//데이터가 송신될때의 클래스 및 데이터 파싱형태를 지정합니다.

public class ParsingManager : MonoBehaviour
{
    #region Instance_Parsing
    private static ParsingManager _instance;

    public static ParsingManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ParsingManager();
            return _instance;
        }
    }
    #endregion

    public byte[] ParsingSendData(ParsingType _type, string _jsonData)
    {
        //전달하는 함수. 전달값은 byte[]로 전달.
        //클래스를 선언한 다음 이 함수 사용
        //string jsonData = JsonUtility.ToJson(data);로 바뀐 데이터를 전달.
        ParsingData data = new(_type, _jsonData);
        string jsonData = JsonUtility.ToJson(data);
        print("ParsingSendData is " + jsonData);
        return Encoding.UTF8.GetBytes(jsonData); //반환값을 Backend.Match.SendDataToinGameRoom으로 전달.
    }




}