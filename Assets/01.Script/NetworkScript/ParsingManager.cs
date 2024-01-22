using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//데이터가 송신될때의 클래스 및 데이터 파싱형태를 지정합니다.

public class ParsingManager : MonoBehaviour
{
    #region Instance_Parsing
    private static ParsingManager _instance;

    public static ParsingManager Instance{
        get{
            if(_instance == null)
                _instance = FindObjectOfType(typeof(ParsingManager)) as ParsingManager;

            return _instance;
        }
    }
    #endregion


}

public class ParsingData{
    public ParsingType type;
    public string data;

    public ParsingData(ParsingType _type, string _data){
        type = _type;
        data = _data;
    }
}

public enum ParsingType{
    Turn,
    Data
}