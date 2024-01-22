using System.Collections;
using System.Collections.Generic;
using System.Text;
using BackEnd;
using UnityEngine;

[System.Serializable]
public class TurnCard : MonoBehaviour
{
    [SerializeField] public int trunNum;
    [SerializeField] public int turncardIdx;

    public void TurnCardClick(){
        GameManager.Instance.turnIndex = trunNum; //나의 턴을 저장.
        this.gameObject.SetActive(false); 
        ParsingData data = new ParsingData(ParsingType.Turn,turncardIdx.ToString()); //선택 시 데이터 전송(비활성화 위해)
        string jsonData = JsonUtility.ToJson(data);
        Backend.Match.SendDataToInGameRoom(Encoding.UTF8.GetBytes(jsonData));
    }
}
