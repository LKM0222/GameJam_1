using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building63 : MonoBehaviour
{
    // 빌딩의 이름
    public string buildingName;

    // 플레이어가 건설한 건물 ID, 우측 하단을 기준으로 시계 방향으로 0번부터 ID를 부여함
    public int buildingID;

    // 빌딩의 타입(제단, 초능력상점, 농장, 랜드마크)
    const int ALTAR = 1, SHOP = 2, FARM = 3, LANDMARK = 4;

    // 건물의 통행료
    public int toll = 50;

    // 건물의 통행료 배수(2이면 통행료 2배)
    public int multiple = 1;

    PlayerStat thePlayerStat;


    // Start is called before the first frame update
    void Start()
    {
        thePlayerStat = FindObjectOfType<PlayerStat>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 빌딩을 살 수 있는지 체크
    public bool IsBuyBuilding(int _buildingID)
    {
        // 살 수 있다면 true 반환(이후 PlayerStat.buildings에 추가해야함)
        if (thePlayerStat.gold >= 100)
            return true;
        else
            return false;
    }

    // 토지를 살 수 있는지 체크
    public bool IsBuyGround(int _groundID)
    {
        // 살 수 있다면 true 반환(이후 PlayerStat.groundID에 추가해야함)
        if (thePlayerStat.gold >= 50)
            return true;
        else
            return false;
    }

    public void BuildingTypeCheck(int _type)
    {

        switch (_type)
        {
            case ALTAR:
                break;
            case SHOP:
                break;
            case FARM:
                break;
            case LANDMARK:
                break;
        }
    }
}