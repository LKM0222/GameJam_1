using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] List<GameObject> tileToGo = new List<GameObject>(); //플레이어가 가야될 타일//최대 12칸.
    public int diceNum; //주사위의 눈금
    [SerializeField] bool diceFlag; // 주사위 굴렸는지 플래그
    [SerializeField] bool movingFlag; //코루틴 반복을 방지하는 플래그
    [SerializeField] GameObject[] cards = new GameObject[7]; //플레이어가 가진 카드
    [SerializeField] int tileNum; //플레이어가 서있는 칸의 번호
    TileManager theTM;//플레이어가 가야될 타일 정보 받아오기 위해 추가

    // Start is called before the first frame update
    void Start()
    {
        theTM = FindObjectOfType<TileManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movingFlag)
        {
            StartCoroutine("DiceCoroutine");
        }
    }

    IEnumerator DiceCoroutine()
    {//코루틴 실행이 안됨...ㅠ  => ?? StartCourotine(DiceCourotine())으로 해야하는거 아님?
        movingFlag = false;
        if (diceFlag)
        {//주사위를 굴렸다면
            for (int i = 0; i < diceNum; i++)
            { //주사위 눈금만큼 리스트에 넣어야됨.
                tileToGo.Add(theTM.tiles[tileNum + 1 + i].gameObject);
            }
            //주사위 굴리는거 기다려야됨
            yield return new WaitForSeconds(1f);
            print("주사위 완료");
            //플레이어 이동
            for (; tileToGo.Count != 0;)
            {
                //리스트 제일 첫번째 요소로 플레이어를 이동시킨 뒤 
                //x = -1.313474 // y = -3.737147
                //x = -0.4295902 // y = -4.414241
                //x = 0.883972 // y = 0.677094
                /////
                //x = -5.556116 // y = 0.8670939
                //x = -4.84901 // y = 1.408769
                //x = 0.707106 // y = 0.5416751


                this.transform.position = tileToGo[0].transform.TransformDirection(tileToGo[0].transform.position);
                print(this.transform.position + " " + tileToGo[0].transform.localPosition + " " + tileToGo[0].transform.position);
                //애니메이션 나오는 시간동안 기다린 뒤
                yield return new WaitForSeconds(0.5f);
                //리스트에서 첫번째 요소 삭제
                tileToGo.RemoveAt(0);
            }


            diceFlag = false;//작업 완료 후 다이스 false
        }
    }
}
