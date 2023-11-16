using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBuyScript : MonoBehaviour
{
    [SerializeField] PlayerManager thePlayer;
    public bool groundBuyFlag;//땅 사는 플래그 활성화 되면 구매창 열기
    public GameObject purchaseUI;
    GameManager theGM;
    // Start is called before the first frame update
    void Start()
    {
        theGM = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(groundBuyFlag){ //구매 버튼을 눌렀다면 땅의 소유주 변경 후, 땅에 건물 올리기 위한 UI활성화
            purchaseUI.SetActive(true); //UI활성화
            theGM.nowPlayer.nowTile.ownPlayer = theGM.nowPlayer.playerId; //땅을 샀으니, 소유주 변경
            theGM.buyedTiles.Add(theGM.nowPlayer.nowTile.gameObject); //구매한 타일을 저장
            groundBuyFlag = false; //재반복 막기 위해 false;
            this.transform.parent.gameObject.SetActive(false);//현재 UI 닫기
        }
    }
}
