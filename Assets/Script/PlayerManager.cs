using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public int playerId; //몇번째 플레이어인지 정보
    [SerializeField] List<GameObject> tileToGo = new List<GameObject>(); //플레이어가 가야될 타일//최대 12칸.
    public Tile nowTile; //현재 서 있는 타일의 정보
    public int diceNum; //주사위의 눈금
    public bool diceFlag; // 주사위 굴렸는지 플래그
    public bool movingFlag; //코루틴 반복을 방지하는 플래그
    [SerializeField] GameObject[] cards = new GameObject[7]; //플레이어가 가진 카드
    [SerializeField] int tileNum; //플레이어가 서있는 칸의 번호
    TileManager theTM;//플레이어가 가야될 타일 정보 받아오기 위해 추가
    public bool myTurn;
    public Text downInformationText;
    GameManager theGM;


    [Header("Building")]
    [SerializeField] int henneryCount;

    [Header("Buy")]
    [SerializeField] GameObject groundBuyUi;
    [SerializeField] GameObject purchaseUi;

    // Start is called before the first frame update
    void Start()
    {
        theTM = FindObjectOfType<TileManager>();
        theGM = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {   
        if(myTurn){
            downInformationText.gameObject.SetActive(true);
        }
        else{
            downInformationText.gameObject.SetActive(false);
        }
        if (movingFlag)
        {
            StartCoroutine("DiceCoroutine");
        }
        // if(tileNum == 0){
        //     this.transform.GetComponent<SpriteRenderer>().color = new Color(1f,1f,0f,1f);
        // }
        // else{
        //     this.transform.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);
        // }
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
                if(tileToGo.Count == 1){//마지막 타일에 이동할 예정이라면
                    nowTile = tileToGo[0].GetComponent<Tile>(); //플레이어가 현재 서있는 타일임.
                }
                if(tileToGo[0].transform.name == "Tile (23)"){
                    this.transform.GetComponent<SpriteRenderer>().color = new Color(1f,1f,0f,1f);
                }
                else{
                    this.transform.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);
                }
                //player를 이동시킴 (애니메이션 필요)
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
            movingFlag = false; //무빙 플래그도 false
            //그다음 플래그 활성화 시켜야됨. 구매 플래그?
            if(!nowTile.specialTile){//일반 땅이라면
                if(nowTile.ownPlayer == playerId){//자기 땅이라면
                    //일단 건물이 있는 땅인지 없는 땅인지 체크
                    if(nowTile.building == null){ //건물이 없는 땅이라면
                        //건물 구매 UI활성화
                        purchaseUi.SetActive(true);
                        //카드 선택 방지를 위한 UI활성화 플래그 활성화
                        theGM.UIFlag = true;
                    }
                    else{ //건물이 있는 땅이라면
                        //효과를 활성화
                    }
                }
                else if(nowTile.ownPlayer == -1){ //주인없는 땅이라면
                    //땅 구매 UI를 활성화
                    groundBuyUi.SetActive(true);
                    //카드 선택 방지를 위한 UI활성화 플래그 활성화
                    theGM.UIFlag = true;
                }
                else{//둘 다 아니라면 상대방의 땅
                    //돈 빼는 코드 작성

                }
            }
            else{//특수 타일이라면 특수 타일의 행동을 함.

                //특수 행동 후 턴을 넘김
                theGM.turnCount += 1;
                theGM.nextTurn = true;
            }
           
            theTM.tiles[tileNum].players = this.GetComponent<PlayerManager>(); //전체 타일의 리스트에도 현재 서있는 정보 반영. 근데 왠지 필요없어보이긴 함....
            //왜냐면 플레이어의 스크립트에 이미 타일의 정보가 들어가있고, 어차피 각각의 플레이어가 각각의 턴을 갖고 움직이기 때문에 
            //굳이?라는 생각이 들기도 함.
            
        }
    }
}
