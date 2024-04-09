using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

#region ParsingClass
public class ParsingData
{
    public ParsingType type;
    public string data;

    public ParsingData(ParsingType _type, string _data)
    {
        type = _type;
        data = _data;
    }
}

public class TurnCardSet
{
    public int randomNum;

    public TurnCardSet(int _randomNum)
    {
        randomNum = _randomNum;
    }
}

public class TurnCard
{
    public int turncardIdx;

    public TurnCard(int _turnCardIdx)
    {
        turncardIdx = _turnCardIdx;
    }
}

public class DiceData
{
    public int diceNum;
    public int playerNum;

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
    TileSelect,
    Extortion,
    Visit,
    ArriveTile,
    Olympic,
    Laser,
    Disconnect
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
        //Backend.Match.SendDataToinGameRoom에 전달할 데이터 파싱
        ParsingData data = new(_type, _jsonData);
        string jsonData = JsonUtility.ToJson(data);
        return Encoding.UTF8.GetBytes(jsonData);
    }
}