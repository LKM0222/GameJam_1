using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Building
{
    // 건물 이름
    public string buildingName;

    // 건물 프리팹
    public GameObject buildingPrefab;

    // 건물 생성, 파괴 애니메이션
    public Animator animator;

    // 건물의 타입(은행, 닭장, 부화장 등)
    public int type; //0부터 시작

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
