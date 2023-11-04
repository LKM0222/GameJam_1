using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Tile: MonoBehaviour
{
    public PlayerManager players; //땅에 도착한 플레이어
    public Building building; //땅에 올라가있는 건물의 정보
    public int price; //땅의 가격
    public int ownPlayer = -1; //몇번째 플레이어의 땅인지 표시
    public SpriteRenderer buildingImg; //땅에 올라가있는 건물 이미지
    public SpriteRenderer signImg; //땅이 누구 소유인지 보여주는 표식
    public bool tileFrontFlag; //타일의 위치 플래그 건물이 보여지는 위치를 결정
    public bool specialTile; //특수타일인지 체크하는 플래그
    public int specialTileType; //특수타일이라면 어떤 타일인지 구분하기 위한 변수

    GameManager theGM;
    private void Start()
    {
        theGM = FindObjectOfType<GameManager>();
        ownPlayer = -1;
        if(!specialTile){
            buildingImg = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
            signImg = this.transform.GetChild(1).GetComponent<SpriteRenderer>();
        }
        
    }

    private void Update() {
        if(!specialTile){
            //땅의 소유 색깔 변경
            if(ownPlayer > -1){
                signImg.sprite = theGM.signSprites[ownPlayer];
            }
            else{
                signImg.sprite = null;
            }
            // 건물의 이미지 변경
            if(building.type > -1){
                if(tileFrontFlag)//true : left, false : front
                    buildingImg.sprite = theGM.buildings[building.type].buildingImg.building_left;
                else{
                    buildingImg.sprite = theGM.buildings[building.type].buildingImg.building_front;
                }
            }
            else{
                buildingImg.sprite = null;
            }
        }
        
        
    }
}
