using System.Collections;
using System.Collections.Generic;
using System.Text;
using BackEnd;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class TurnCard : MonoBehaviour
{
    [SerializeField] public int trunNum;
    [SerializeField] public int turncardIdx;

    public void TurnCardClick(){
        if(GameManager.Instance.turnIndex == 0) //턴 인덱스가 0이면 아직 내 턴카드를 선택하지 않은것.
            GameManager.Instance.turnIndex = trunNum; //나의 턴을 저장.
        this.gameObject.SetActive(false); 
        ParsingData data = new ParsingData(ParsingType.Turn,turncardIdx.ToString()); //선택 시 데이터 전송(비활성화 위해)
        string jsonData = JsonUtility.ToJson(data);
        Backend.Match.SendDataToInGameRoom(Encoding.UTF8.GetBytes(jsonData));
    }

    private void Update() {
        if(GameManager.Instance.playerCount.Count > 1){
            //둘 다 카드를 뽑았다면 부모 객체를 안보이게 설정한다.
            this.transform.parent.gameObject.SetActive(false); 
        }
    }
}
