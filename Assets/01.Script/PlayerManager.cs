using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

//커밋준비
public class PlayerManager : MonoBehaviour
{
    public int playerId; //몇번째 플레이어인지 정보
    public int playerMoney; //플레이어의 자금
    public Text playerMoneyText; //플레이어 자금 플로팅
    [SerializeField] float speed; //이동속도
    [SerializeField] bool movingFlag; //이동 완료인지 이동중인지 체크하는 플래그
    [SerializeField] List<GameObject> tileToGo = new List<GameObject>(); //플레이어가 가야될 타일//최대 12칸.
    public Tile nowTile; //현재 서 있는 타일의 정보
    public int diceNum; //주사위의 눈금
    public bool diceFlag; // 주사위 굴렸는지 플래그
    public bool movingCoroutineFlag; //코루틴 반복을 방지하는 플래그
    public List<Card> cards = new List<Card>(); //플레이어가 가진 카드
    PlayerCard thePCard;
    [SerializeField] int tileNum; //플레이어가 서있는 칸의 번호
    TileManager theTM;//플레이어가 가야될 타일 정보 받아오기 위해 추가
    public bool myTurn;
    public Text downInformationText;
    GameManager theGM;

    public GameObject VirtualCamera;

    public GameObject cardPrefab;
    public Transform cardParent;
    public PlayerManager againstPlayer;
    public List<GameObject> againstPlayer_Tile = new List<GameObject>();

    [Header("Building")]
    public int buildingCount = 0;
    public int groundCount = 0;

    [Header("Buy")]
    [SerializeField] GameObject groundBuyUi;
    [SerializeField] GameObject purchaseUi;

    TurnSignScript theTSI;
    [Header("Effect")]
    public bool tpFlag;
    bool tpSelectFlag;
    public GameObject tpBack; //tp활성화 시 맵 이외의 주변이 어둡게 변함.
    public bool tpMovingFlag; //tp무빙 플래그 활성화 시 애니메이션 재생
    public GameObject tpTile; //다음 이동할곳 저장
    public bool highSpeedFlag;
    // public bool invisibleFlag; //투명화
    // public bool toosiFlag; //투시
    // public bool biggerFlag; //거대화

    // Start is called before the first frame update
    void Start()
    {
        theTM = FindObjectOfType<TileManager>();
        theGM = FindObjectOfType<GameManager>();
        theTSI = FindObjectOfType<TurnSignScript>();
        thePCard = FindObjectOfType<PlayerCard>();
    }

    // Update is called once per frame
    void Update()
    {
        playerMoneyText.text = playerMoney.ToString();
        if (myTurn)
        {
            downInformationText.gameObject.SetActive(true);
        }
        else
        {
            downInformationText.gameObject.SetActive(false);
        }
        if (!tpFlag && movingCoroutineFlag)
        {
            StartCoroutine("DiceCoroutine");
        }
        // if (toosiFlag)
        // {
        //     StartCoroutine("TooSiCoroutine");
        // }

        if(tpFlag && myTurn){//말끔하게 수정 필요
            tpMovingFlag = true;
            StartCoroutine(TeleportCoroutine());
            //waitcoroutine 추가로 자신의 턴이 끝나기 전에 상대방이 움직이는 버그 해결
            StartCoroutine(NextTrunWait());
        }
    }

    IEnumerator DiceCoroutine()
    {
        movingCoroutineFlag = false;
        // if (invisibleFlag && myTurn)
        // {//invisibleFalg
        //     this.gameObject.GetComponent<SpriteRenderer>().color =
        //         new Color(1, 1, 1, 0.5f);
        // }
        // if(biggerFlag && myTurn){
        //     this.gameObject.transform.localScale = new Vector3(2f,2f,0);
        // }
        if (diceFlag)
        {//주사위를 굴렸다면
            //highspeedflag는 diceSystem으로 넘어감. 거기가 더 깔끔해보여서. 테스트 후 아래의 주석 처리 코드는 삭제할것.
            // if (highSpeedFlag && myTurn)
            // { //고속이동을 활성화 했다면
            //     for (int i = 0; i < diceNum * 2; i++) //두배로 이동
            //     { //주사위 눈금만큼 리스트에 넣어야됨.
            //         if (tileNum + i >= theTM.tiles.Length)
            //         {
            //             //넣어야하는 오브젝트의 길이가 전체 리스트의 길이를 넘어간다면 제대로 더해지지 않는거임.
            //             tileToGo.Add(theTM.tiles[tileNum + i - theTM.tiles.Length].gameObject);
            //         }
            //         else
            //         {
            //             //아니라면 그냥 추가시켜주면 됨.
            //             tileToGo.Add(theTM.tiles[tileNum + i].gameObject);
            //         }

            //     }
            //     highSpeedFlag = false;
            // }
                for (int i = 0; i < diceNum; i++)
                { //주사위 눈금만큼 리스트에 넣어야됨.
                    if (tileNum + i >= theTM.tiles.Length)
                    {
                        //넣어야하는 오브젝트의 길이가 전체 리스트의 길이를 넘어간다면 제대로 더해지지 않는거임.
                        tileToGo.Add(theTM.tiles[tileNum + i - theTM.tiles.Length].gameObject);
                    }
                    else
                    {
                        //아니라면 그냥 추가시켜주면 됨.
                        tileToGo.Add(theTM.tiles[tileNum + i].gameObject);
                    }

                }
 


            VirtualCamera.SetActive(true);
            //주사위 굴리는거 기다려야됨
            yield return new WaitForSeconds(1f);
            print("주사위 완료");
            //플레이어 이동
            theTSI.cursorPos = 3;
            for (; tileToGo.Count != 0;)
            {
                if (tileToGo[0].transform.name == "0")
                {
                    this.transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 0f, 1f);
                    playerMoney += 100; //지나다닐때마다 100알씩 지급
                }
                else
                {
                    this.transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                }
                //player를 이동시킴 (애니메이션 필요) 메인 이동 코드
                movingFlag = true;
                //this.transform.position = tileToGo[0].transform.TransformDirection(tileToGo[0].transform.Find("Pos").transform.position);
                Vector3 targetPos = tileToGo[0].transform.Find("Pos").transform.position;
               
                StartCoroutine(MovingCoroutine(targetPos));
                yield return new WaitUntil(()=> movingFlag == false); //코루틴이 끝난지 체크
                nowTile = tileToGo[0].GetComponent<Tile>(); //현재 타일
                //AudioManager.instance.Play("moveSound");
            
                // if(invisibleFlag){//invisible플래그가 활성화되어있고
                //     if(againstPlayer.tileNum == tileNum){ //내가 상대방과 같은 타일을 지나간다면
                //         GameObject dCard = againstPlayer.cardParent.GetChild(UnityEngine.Random.Range(0,againstPlayer.cardParent.childCount)).gameObject;
                //         dCard.transform.parent = cardParent;
                //         //카드 한장을 훔침.
                //     }

                // }
                //애니메이션 나오는 시간동안 기다린 뒤
                yield return new WaitForSeconds(0.5f);

                //리스트에서 첫번째 요소 삭제
                tileToGo.RemoveAt(0);
            }

            if (tileNum + diceNum > theTM.tiles.Length)
            {
                //만약 현재 위치를 업데이트 했을때, 총 타일의 길이를 넘어간다면 길이만큼 빼 줘야 정확한 위치에 있는것임.
                tileNum += diceNum;
                tileNum -= theTM.tiles.Length;
            }
            else
            {//아니라면 그대로 더하기 진행
                tileNum += diceNum;
            }
            diceFlag = false;//작업 완료 후 다이스 false
            this.gameObject.GetComponent<Animator>().SetBool("WalkFlag", false);
            movingCoroutineFlag = false; //무빙 플래그도 false


            
            //그다음 플래그 활성화 시켜야됨. 구매 플래그?
            if (!nowTile.specialTile)
            {//일반 땅이라면
                if (nowTile.ownPlayer == playerId)
                {//자기 땅이라면
                    //일단 건물이 있는 땅인지 없는 땅인지 체크
                    if (nowTile.building == null)
                    { //건물이 없는 땅이라면
                        //건물 구매 UI활성화
                        purchaseUi.SetActive(true);
                        //카드 선택 방지를 위한 UI활성화 플래그 활성화
                        theGM.UIFlag = true;
                    }
                    else
                    { //건물이 있는 땅이라면
                        //효과를 활성화
                    }
                }
                else if (nowTile.ownPlayer == -1)
                { //주인없는 땅이라면
                    //땅 구매 UI를 활성화
                    groundBuyUi.SetActive(true);
                    //카드 선택 방지를 위한 UI활성화 플래그 활성화
                    theGM.UIFlag = true;
                }
                else
                {//둘 다 아니라면 상대방의 땅
                    //돈 빼는 코드 작성
                    // if(biggerFlag){
                    //     nowTile.ownPlayer = -1;
                    //     nowTile.building = null;
                    //     this.gameObject.transform.localScale = new Vector3(1f,1f,1f);
                    //     biggerFlag = false;
                    // }
                    //else{

                        if(nowTile.building != null){ //건물이 있는경우
                            switch(nowTile.building.type){ //빌딩 타입 검사
                                case 1:

                                break;
                                case 3:
                                    if(nowTile.building.visitCount < 5)
                                        nowTile.building.visitCount += 1;
                                    playerMoney -= nowTile.building.visitCount * 100;
                                    againstPlayer.playerMoney += nowTile.building.visitCount * 100;
                                    break;
                                default:
                                    playerMoney -= 100;
                                    againstPlayer.playerMoney += 100;
                                break;
                            }
                            
                        }
                        
                        else{
                            playerMoney -= 50;
                        }
                    //}

                    theGM.NextTurnFunc();

                }
            }
            else
            {//특수 타일이라면 특수 타일의 행동을 함.
                print("specialTile" + nowTile.specialTileType);
                switch (nowTile.specialTileType)
                {//(0 : 양계장, 1 : 카드, 2 : 워프, 3 : 세금, 4 : 강탈)
                    case 0: //양계장 

                        break;
                    case 1: //카드 지급 
                        print("card Tile");
                        // 카드 구현해야됨.
                        if (cardParent.childCount < 8)//카드는 최대 7장
                        {
                            Card newCard = theGM.cards[UnityEngine.Random.Range(0, theGM.cards.Length)];
                            print(newCard.card_name);
                            var _card = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity, cardParent);//카드 프리펩 생성해주고
                            _card.transform.localPosition = new Vector3(0f, 0f, 0f);
                            cards.Add(newCard); //플레이어 리스트에 카드 추가
                        }

                        break;
                    case 2: //teleport
                        //teleportFlag활성화
                        tpSelectFlag = true;
                        StartCoroutine("TeleportSetCoroutine");


                        break;
                    case 3: //세금
                        //상대방 땅 갯수 *5 + 건물 갯수 * 10
                        //세금 징수하는 애니메이션이나, 영수증 띄워주면 좋을듯
                        playerMoney -= (groundCount * 5) + (buildingCount * 10);

                        break;
                    case 4: //강탈
                        //상대 플레이어 카드 무작위 한장 뺏어옴.
                        GameObject dCard = againstPlayer.cardParent.GetChild(UnityEngine.Random.Range(0, againstPlayer.cardParent.childCount)).gameObject;
                        dCard.transform.parent = cardParent;
                        break;

                }
                if (!tpSelectFlag)
                { //tp중일땐 일단 타일이 선택되기 전까지는 기다려야하기 때문에 탈출할 수 없음...
                    //특수 행동 후 턴을 넘김
                    theGM.NextTurnFunc();
                    //invisibleFlag = false;
                    this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                }
            }
            VirtualCamera.SetActive(false);
        }
        else
        { //tpFlag일 경우 텔레포트함.

        }
    }

    IEnumerator TooSiCoroutine()
    {
        for (int i = 0; i < againstPlayer.cards.Count; i++)
        {
            againstPlayer.cardParent.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(3f);
        for (int i = 0; i < againstPlayer.cards.Count; i++)
        {
            againstPlayer.cardParent.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }
    }

    IEnumerator TeleportSetCoroutine()
    {
        tpBack.SetActive(true);
        theGM.tpImg.SetActive(true);
        for (int i = 0; i < theTM.tiles.Length; i++)
        {
            theTM.tiles[i].cardActive = true; //모든 카드 클릭 가능하도록 미리 클릭하고 다음턴에 해당 위치로 이동.
        }


        yield return new WaitUntil(() => theGM.tpTile != null);
        for (int i = 0; i < theTM.tiles.Length; i++)
        {
            theTM.tiles[i].cardActive = false; //다시 클릭 못하도록 변경
        }
        tpTile = theGM.tpTile;
        theGM.tpTile = null;
        tpBack.SetActive(false);
        theGM.tpImg.SetActive(false);
        tpFlag = true;
        myTurn = false;
        theGM.NextTurnFunc();
    }

    IEnumerator TeleportCoroutine(){
        this.tileNum = int.Parse(tpTile.gameObject.name);
        this.tileToGo.Add(tpTile);
        //주석 처리된 코드는 그 자리로 순간이동하는 코드
        //만약, 순간이동처럼 보이고 싶다면 애니매이션 추가한 후 아래의 코드를 써야될듯.
        //this.transform.position = tileToGo[0].transform.TransformDirection(tileToGo[0].transform.Find("Pos").transform.position);
        //새로 작성한 코드는 그 자리로 moving하는것처럼 이동하는 코드
        Vector3 target = tileToGo[0].transform.Find("Pos").transform.position;
        while(tpMovingFlag){
            this.transform.position = Vector3.MoveTowards(this.transform.position, target, Time.deltaTime * speed);
            // 여기서 뭔가 텔레포트하는 애니메이션이 필요할듯!
            if(this.transform.position == target){
                tpMovingFlag = false;
            }
        }
        nowTile = tileToGo[0].GetComponent<Tile>();
        this.GetComponent<Animator>().SetInteger("Dir", nowTile.dir);
        this.tileToGo.RemoveAt(0);
        theGM.NextTurnFunc();
        tpFlag = false;
        yield return null;
    }
    IEnumerator NextTrunWait(){ //현재 플레이어의 텔레포트가 완료되기 전에 상대 플레이어가 이동하는걸 방지하는 코드.
        yield return new WaitUntil(()=> tpMovingFlag == false);
    }

    IEnumerator MovingCoroutine(Vector3 target){
        this.gameObject.GetComponent<Animator>().SetInteger("Dir",nowTile.dir);
        this.gameObject.GetComponent<Animator>().SetBool("WalkFlag",true);
        while(movingFlag){
            this.transform.position = Vector3.MoveTowards(this.transform.position, target, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
            if(this.transform.position == target){
                movingFlag = false;
            }
        }
        this.gameObject.GetComponent<Animator>().SetBool("WalkFlag",false);
        yield return null;
    }
    
}
