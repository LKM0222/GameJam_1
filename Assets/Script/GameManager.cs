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

    public PlayerManager nowPlayer; //현재 턴인 플레이어 정보

    public bool UIFlag; //UI가 활성화 되어있는지 확인하는 플래그
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
