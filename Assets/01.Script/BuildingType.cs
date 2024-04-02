using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//건물의 스프라이트를 저장하는 클래스
//건물의 위치에 따라 다른 이미지 (가로줄일경우는 1번이미지, 세로줄일경우 2번이미지)
[System.Serializable]
public class BuildingType
{
    public Sprite building_front;
    public Sprite building_left;
}
