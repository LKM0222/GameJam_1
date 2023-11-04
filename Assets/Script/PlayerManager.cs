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
    {
        movingFlag = false;
        if (diceFlag)
        {//주사위를 굴렸다면
            for (int i = 0; i < diceNum; i++)
            { //주사위 눈금만큼 리스트에 넣어야됨.
                if(tileNum + i >= theTM.tiles.Length){
                    //넣어야하는 오브젝트의 길이가 전체 리스트의 길이를 넘어간다면 제대로 더해지지 않는거임.
                    tileToGo.Add(theTM.tiles[tileNum + i - theTM.tiles.Length].gameObject);
                }
                else{
                    //아니라면 그냥 추가시켜주면 됨.
                    tileToGo.Add(theTM.tiles[tileNum + i].gameObject);
                }
                
            }
            //주사위 굴리는거 기다려야됨
            yield return new WaitForSeconds(1f);
            print("주사위 완료");
            //플레이어 이동
            for (; tileToGo.Count != 0;)
            {   
                //player를 이동시킴
                this.transform.position = tileToGo[0].transform.TransformDirection(tileToGo[0].transform.position);
                //애니메이션 나오는 시간동안 기다린 뒤
                yield return new WaitForSeconds(0.5f);
                //리스트에서 첫번째 요소 삭제
                tileToGo.RemoveAt(0);
            }

            if(tileNum+ diceNum > theTM.tiles.Length){
                //만약 현재 위치를 업데이트 했을때, 총 타일의 길이를 넘어간다면 길이만큼 빼 줘야 정확한 위치에 있는것임.
                tileNum += diceNum;
                tileNum -= theTM.tiles.Length;
            }
            else{//아니라면 그대로 더하기 진행
                tileNum += diceNum;
            }
            
            diceFlag = false;//작업 완료 후 다이스 false
        }
    }
}
