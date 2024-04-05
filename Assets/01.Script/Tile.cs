using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;

[System.Serializable]
public class Tile : MonoBehaviour
{
    CardManager theCM;

    public Building building;
    public SpriteRenderer buildingImg;
    public SpriteRenderer signImg;

    // 건물의 방향
    public bool tileFrontFlag;
    public bool specialTile;
    public int specialTileType;
    public int dir;

    public int price;
    public int ownPlayer = -1;
    public bool canTileSelect;

    private void Start()
    {
        theCM = FindObjectOfType<CardManager>();

        ownPlayer = -1;

        if (!specialTile)
        {
            buildingImg = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
            signImg = this.transform.GetChild(1).GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (!specialTile)
        {
            // 타일 소유주가 변경되면 Sign 타일 수정
            if (ownPlayer == -1) signImg.sprite = null;
            else if (ownPlayer == 0) signImg.sprite = GameManager.Instance.signSprites[0];
            else signImg.sprite = GameManager.Instance.signSprites[1];

            // 건물이 있다면 건물의 이미지 수정
            if (building.type > -1)
            {
                if (tileFrontFlag) buildingImg.sprite = GameManager.Instance.buildings[building.type + 1].buildingImg.building_left;
                else buildingImg.sprite = GameManager.Instance.buildings[building.type + 1].buildingImg.building_front;
            }
            else buildingImg.sprite = null;
        }
    }

    private void OnMouseDown()
    {
        // 레이저 사용 시 OR 텔레포트 사용 시
        if (canTileSelect && theCM.isSelectingLaser || canTileSelect && GameManager.Instance.nowPlayer.isSelectingTeleport)
        {
            AudioManager.Instance.Play("SelectTile_Sound");
            GameManager.Instance.seletedTile = this.gameObject;
        }

        // 건물 강탈 사용시
        if (canTileSelect && GameManager.Instance.nowPlayer.isExtortioning)
        {
            AudioManager.Instance.Play("SelectTile_Sound");

            TileSelectData tileSelectData = new(this.gameObject.name);
            string jsondata = JsonUtility.ToJson(tileSelectData);
            byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.TileSelect, jsondata);
            Backend.Match.SendDataToInGameRoom(data);
        }
    }
}
