using System.Collections;
using System.Collections.Generic;
using System.Text;
using BackEnd;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TurnCard : MonoBehaviour
{
    public int turnNum;
    public int turncardIdx;

    public TurnCard(int _turnNum, int _turnCardIdx){
        turnNum = _turnNum;
        turncardIdx = _turnCardIdx;
    }

    public void TurnCardClick(){
        if(GameManager.Instance.turnIndex == 0) //턴 인덱스가 0이면 아직 내 턴카드를 선택하지 않은것.
            GameManager.Instance.turnIndex = turnNum; //나의 턴을 저장.
        this.gameObject.SetActive(false); 
        TurnCard tCard = new TurnCard(turnNum, turncardIdx);
        // string tJson = JsonUtility.ToJson(tCard);
        // ParsingData data = new ParsingData(ParsingType.Turn,tJson); //선택 시 데이터 전송(비활성화 위해)
        // string jsonData = JsonUtility.ToJson(data);
        string jsonData = JsonUtility.ToJson(tCard);
        Backend.Match.SendDataToInGameRoom(ParsingManager.Instance.ParsingSendData(ParsingType.Turn, jsonData));
    }
}
