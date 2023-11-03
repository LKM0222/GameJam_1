using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject[] tileToGo = new GameObject[12]; //플레이어가 가야될 타일
    public int diceNum; //주사위의 눈금
    [SerializeField] GameObject[] cards = new GameObject[7]; //플레이어가 가진 카드

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
