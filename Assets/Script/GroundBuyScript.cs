using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 토지 구매 관리 스크립트(건물 X)
public class GroundBuyScript : MonoBehaviour
{
    GameManager theGM;

    // [SerializeField] PlayerManager thePlayer;

    public GameObject purchaseUI;

    // 땅을 구매하면 건물 구매창을 띄우기 위한 플래그
    public bool groundBuyFlag;


    // Start is called before the first frame update
    void Start()
    {
        theGM = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //구매 버튼을 눌렀다면 땅의 소유주 변경 후, 땅에 건물 올리기 위한 UI활성화
        if (groundBuyFlag)
        {
            purchaseUI.SetActive(true); //UI활성화
            theGM.nowPlayer.nowTile.ownPlayer = theGM.nowPlayer.playerId; //땅을 샀으니, 소유주 변경
            theGM.buyedTiles.Add(theGM.nowPlayer.nowTile.gameObject); //구매한 타일을 저장
            groundBuyFlag = false; //재반복 막기 위해 false;
            this.transform.parent.gameObject.SetActive(false);//현재 UI 닫기
        }
    }
}
