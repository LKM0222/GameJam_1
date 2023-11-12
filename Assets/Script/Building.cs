using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Building
{
    //건물 이미지
    public BuildingType buildingImg;

    // 건물 이름
    public string buildingName;

    // 건물 생성, 파괴 애니메이션
    public Animator animator;

    // 건물의 타입(은행, 닭장, 부화장 등)
    public int type; //0부터 시작

    //건물방문횟수
    public int visitCount;
}
