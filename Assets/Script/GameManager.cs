using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Sprite[] signSprites = new Sprite[2];
    public BuildingType[] buildingSprite = new BuildingType[1];
    public Card[] cards = new Card[0];
    public Building[] buildings = new Building[4];

    public PlayerManager[] players = new PlayerManager[2]; //전체 플레이어 리스트
    public PlayerManager nowPlayer;

    public int turnCount;
    public bool nextTurn = false;

    public bool UIFlag = false; //UI가 활성화 되어있는지 확인하는 플래그
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if(nextTurn){
            if(turnCount % 2 == 1){//나머지가 1이면 1플레이어, 0이면 2플레이어
                players[0].myTurn = true;
                players[1].myTurn = false;
                nowPlayer = players[0];
            }
            else{
                players[1].myTurn = true;
                players[0].myTurn = false;
                nowPlayer = players[1];
            }
            nextTurn = false;
        }
        
    }
}
