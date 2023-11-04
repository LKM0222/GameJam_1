using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Tile: MonoBehaviour
{
    public Player[] players;
    public Building building;
    public int price;
    public GameObject tilePrefab;
    public int ownPlayer = -1;
    public SpriteRenderer buildingImg;
    public SpriteRenderer signImg;
    public bool tileFrontFlag;
    public Vector3 worldPos;
    public bool specialTile;

    GameManager theGM;
    private void Start()
    {
        worldPos = this.transform.TransformDirection(this.transform.position);
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
                if(tileFrontFlag)//true : front, false : left
                    buildingImg.sprite = theGM.buildingSprite[building.type].building_front;
                else{
                    buildingImg.sprite = theGM.buildingSprite[building.type].building_left;
                }
            }
            else{
                buildingImg.sprite = null;
            }
        }
        
        
    }
}
