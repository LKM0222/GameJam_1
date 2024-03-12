using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    GameManager theGM;
    TileManager theTile;
    TurnSignScript theTSI;
    AudioManager theAudio;
    DiceSystem theDice;

    // 카드의 정보
    public Card cardInfo;

    // 카드를 위아래로 움직일 수치
    [SerializeField] float upPos;

    public bool biggerComplete;

    public bool isShowCard;
    public bool isGetCard;
    public bool isSelectingLaser;

    public ParticleSystem destroyParticle;
    
    public ParticleSystem highMoveParticle;
    public ParticleSystem InvisibleParticle;
    public ParticleSystem exemptionParticle;

    #region Instance
    private static CardManager _instance;
    public static CardManager Instance{
        get{
            if(_instance == null){
                _instance = FindObjectOfType(typeof(CardManager)) as CardManager;
            }
            return _instance;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        theGM = FindObjectOfType<GameManager>();
        theTile = FindObjectOfType<TileManager>();
        theTSI = FindObjectOfType<TurnSignScript>();
        theAudio = FindObjectOfType<AudioManager>();
        theDice = FindObjectOfType<DiceSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 해당 스크립트가 붙은 오브젝트(획득한 카드)에 마우스를 올려놓았을 때 호출
    private void OnMouseEnter()
    {
        if (theTSI.cursorPos == 1 && !theDice.isDrag)
        {
            theAudio.Play("SelectCard_Sound");

            // upPos만큼 Position을 올리고 Scale을 늘림
            this.transform.localScale = new Vector3(14f, 14f, 1f);
            this.transform.position += Vector3.up * upPos;
            this.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }

    // 해당 스크립트가 붙은 오브젝트(획득한 카드)에 마우스를 올렸다가 떨어트렸을 때 호출
    private void OnMouseExit()
    {
        if (theTSI.cursorPos == 1 && !theDice.isDrag)
        {
            // upPos만큼 Position을 내리고 Scale을 줄임
            this.transform.localScale = new Vector3(7f, 7f, 1f);
            this.transform.position += Vector3.down * upPos;
            this.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
    }

    // 해당 스크립트가 붙은 오브젝트(획득한 카드)에 마우스를 클릭할 때 호출
    private void OnMouseDown()
    {
        if (theTSI.cursorPos == 1 && GameManager.Instance.myCharactor.myTurn)
        {
            theAudio.Play("UseCard_Sound");

            // cardCode가 1이라면 고속이동(중복사용 방지를 위해 플래그가 꺼져있을때만)
            if (cardInfo.cardCode == 1 && !theGM.nowPlayer.highSpeedFlag)
            {
                CardClickData cData = new(1, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);

                // // theGM.nowPlayer.highSpeedFlag = true;
                DestroyCard();
            }
            // cardCode가 2라면 투명도둑
            else if (cardInfo.cardCode == 2 && !theGM.nowPlayer.invisibleFlag)
            {
                CardClickData cData = new(2, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);

                // theGM.nowPlayer.invisibleFlag = true;
                DestroyCard();
            }
            // cardCode가 3이라면 거대화꼬꼬
            else if (cardInfo.cardCode == 3 && !theGM.nowPlayer.biggerFlag)
            {
                CardClickData cData = new(3, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);
                // theGM.nowPlayer.biggerFlag = true;
                DestroyCard();
            }
            // cardCode가 4라면 투시(투시카드 사용이 완료되었고, 레이저빔도 완료되어야만 사용 가능 => 둘 중 하나라도 발동중이면 사용불가)
            else if (cardInfo.cardCode == 4 && !theGM.nowPlayer.toosiFlag && theGM.penetrateComplete && theGM.laserComplete)
            {
                CardClickData cData = new(4, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);
                // theGM.nowPlayer.toosiFlag = true;
                DestroyCard();
            }
            // cardCode가 5라면 주사위컨트롤(하)
            else if (cardInfo.cardCode == 5 && !theGM.nowPlayer.lowerDiceFlag)
            {
                CardClickData cData = new(5, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);
                // theGM.nowPlayer.lowerDiceFlag = true;
                theGM.nowPlayer.lowerDiceFlag = true;
                theGM.nowPlayer.higherDiceFlag = false;
                DestroyCard();
            }
            // cardCode가 6이라면 주사위컨트롤(상)
            else if (cardInfo.cardCode == 6 && !theGM.nowPlayer.higherDiceFlag)
            {
                CardClickData cData = new(6, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);
                // theGM.nowPlayer.higherDiceFlag = true;
                theGM.nowPlayer.higherDiceFlag = true;
                theGM.nowPlayer.lowerDiceFlag = false;
                DestroyCard();
            }
            // cardCode가 8이라면 레이저빔(레이저빔 사용이 완료되었고, 투시 완료되어야만 사용 가능 => 둘 중 하나라도 발동중이면 사용불가)
            else if (cardInfo.cardCode == 8 && !theGM.nowPlayer.laserFlag && theGM.laserComplete && theGM.penetrateComplete)
            {
                // 모든 타일을 돌면서 상대방 소유의 땅이 있는지 검사
                for (int i = 0; i < theTile.tiles.Length; i++)
                {
                    // 타일의 주인이 상대플레이어의 ID와 일치하면 카드 사용 가능
                    if (theTile.tiles[i].ownPlayer == theGM.nowPlayer.againstPlayer.playerId)
                    {
                        CardClickData cData = new(8, GameManager.Instance.nowPlayer.playerId);
                        string jsonData = JsonUtility.ToJson(cData);
                        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                        Backend.Match.SendDataToInGameRoom(data);
                        // theGM.nowPlayer.laserFlag = true;
                        DestroyCard();
                        break;
                    }
                }
            }
        }
    }

    public void DestroyCard()
    {
        print("send : card is " + cardInfo.cardCode);
        CardDestroyData destroyData = new(this.gameObject, cardInfo.cardCode);
        string jsonData = JsonUtility.ToJson(destroyData);
        byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.CardDestory, jsonData);
        Backend.Match.SendDataToInGameRoom(sendData);

        // 카드 오브젝트 삭제 및 플레이어가 가지고 있는 카드 리스트에서도 삭제
        // Destroy(this.gameObject);
        // Destroy(theGM.nowPlayer.cardParent.GetChild(0).gameObject);
        // theGM.nowPlayer.cards.Remove(this.cardInfo);
    }

    public void HighSpeedMove()
    {
        theGM.nowPlayer.diceNum = theGM.nowPlayer.diceNum * 2;
        theGM.nowPlayer.moveSpeed = 6f;
        highMoveParticle.gameObject.SetActive(true);
        highMoveParticle.Play();
    }

    public void EndHighSpeedMove()
    {
        theGM.nowPlayer.highSpeedFlag = false;
        theGM.nowPlayer.moveSpeed = 4f;
        highMoveParticle.Stop();
        highMoveParticle.gameObject.SetActive(false);
    }

    public IEnumerator InvisibleThief()//여기 수정해야됨.
    {
        print("invisible PlayCoroutine");
        if (theGM.nowPlayer.cards.Count < 8)
        {
            // 상대가 가진 카드를 랜덤으로 골라서 현재 플레이어 카드에 추가하고 상대 플레이어 카드에는 삭제
            //여기 통신 필요
            //랜덤값을 서로 전송해서 받아와야됨.
            // int randomCard = UnityEngine.Random.Range(0, theGM.nowPlayer.againstPlayer.cards.Count);
            //통신을 한다면 속도를 맞출 수 있을까?
            byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.InvisibleThief, "");
            Backend.Match.SendDataToInGameRoom(sendData);
            print("invisible sendServer");
            yield return new WaitUntil(() => GameManager.Instance.invisibleCardNum != -1);

            // 효과음 추가하기
            theAudio.Play("InvisibleThief_Sound");

            
            // if (theGM.nowPlayer.againstPlayer.cards[randomCard] == theGM.cards[6])
            // {
            //     theGM.nowPlayer.exemptionFlag = true;
            //     theGM.nowPlayer.againstPlayer.exemptionFlag = false;
            // }

            // theGM.nowPlayer.cards.Add(theGM.nowPlayer.againstPlayer.cards[randomCard]);
            // theGM.nowPlayer.againstPlayer.cards.RemoveAt(randomCard);
            // // 만약 뺏어온 카드가 통행료면제 카드라면 플래그를 서로 바꿔줌 GameManager.Instance.invisibleCardNum
            if (theGM.nowPlayer.againstPlayer.cards[GameManager.Instance.invisibleCardNum] == theGM.cards[6])
            {
                //이거도 뭔가 통신쪽으로 가야될듯 (나중에 수정)
                theGM.nowPlayer.exemptionFlag = true;
                theGM.nowPlayer.againstPlayer.exemptionFlag = false;
            }

            theGM.nowPlayer.cards.Add(theGM.nowPlayer.againstPlayer.cards[GameManager.Instance.invisibleCardNum]);
            theGM.nowPlayer.againstPlayer.cards.RemoveAt(GameManager.Instance.invisibleCardNum);

            //파티클
            InvisibleParticle.gameObject.SetActive(true);
            InvisibleParticle.transform.position = theGM.nowPlayer.transform.position;
            InvisibleParticle.Play();

            // 상대방 카드 UI를 내 카드 UI로 옮겨오고 스프라이트 이미지도 변경
            GameObject dCard = theGM.nowPlayer.againstPlayer.cardParent.GetChild(0).gameObject;
            dCard.transform.SetParent(theGM.nowPlayer.cardParent);
            dCard.GetComponent<SpriteRenderer>().sprite = theGM.nowPlayer.cardPrefab.GetComponent<SpriteRenderer>().sprite;

            theGM.CardListUpdate();
        }
    }

    public void EndInvisibleThief()
    {
        theGM.nowPlayer.invisibleFlag = false;
        InvisibleParticle.Stop();
        InvisibleParticle.gameObject.SetActive(false);
        StartCoroutine(theGM.nowPlayer.SetPlayerTransparency("Visible"));
    }

    public void LowerDiceControl()
    {
        // theGM.nowPlayer.diceNum = Random.Range(1, 5);
        DiceData dData = new(Random.Range(1, 5), GameManager.Instance.turnIndex); //서버로 전송하기 위해 데이터 클래스화
        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.Dice, JsonUtility.ToJson(dData));
        Backend.Match.SendDataToInGameRoom(data);
        theGM.nowPlayer.lowerDiceFlag = false;
    }

    public void HigherDiceControll()
    {
        // theGM.nowPlayer.diceNum = Random.Range(5, 9);
        DiceData dData = new(Random.Range(5, 9), GameManager.Instance.turnIndex); //서버로 전송하기 위해 데이터 클래스화
        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.Dice, JsonUtility.ToJson(dData));
        Backend.Match.SendDataToInGameRoom(data);
        theGM.nowPlayer.higherDiceFlag = false;
    }

    public IEnumerator TollExemption()
    {
        print("TollExemption coroutine start");
        // 현재 자신의 카드 중에서 통행료 면제 카드를 찾아서 파괴함
        for (int i = 0; i < theGM.nowPlayer.cards.Count; i++)
        {
            if (theGM.nowPlayer.cards[i] == theGM.cards[6])
            {
                theGM.nowPlayer.cards.RemoveAt(i);
                Destroy(theGM.nowPlayer.cardParent.GetChild(0).gameObject);
                print("cardFind!");
                break;
            }
        }
        theAudio.Play("TollExemption_Sound");
        
        //파티클
        exemptionParticle.transform.position = theGM.nowPlayer.transform.position;
        exemptionParticle.gameObject.SetActive(true);
        exemptionParticle.Play();
        yield return new WaitForSeconds(1f);
        exemptionParticle.gameObject.SetActive(false);

        // 카드 효과를 사용했으니 flag를 false로 바꿔줌
        theGM.nowPlayer.exemptionFlag = false;
        yield return null;
    }

    // 거대화 사용효과 및 애니메이션 코루틴
    public IEnumerator BiggerCoroutine()
    {
        theGM.nowPlayer.biggerFlag = false;
        // 현재 타일이 빈 땅도 아니고, 현재 플레이어 id와 ownPlayer가 다르다면 상대방의 땅
        if (theGM.nowPlayer.nowTile.ownPlayer != -1 && theGM.nowPlayer.playerId != theGM.nowPlayer.nowTile.ownPlayer)
        {
            // 건물과 타일의 컬러를 받아옴
            Color buildingColor = theGM.nowPlayer.nowTile.buildingImg.GetComponent<SpriteRenderer>().color;
            Color tileColor = theGM.nowPlayer.nowTile.signImg.GetComponent<SpriteRenderer>().color;

            theAudio.Play("BuildingDestroy_Sound");

            // 건물파괴 파티클을 활성화하고 위치를 현재 타일의 건물 위치로 옮긴 다음 파티클 실행
            destroyParticle.gameObject.SetActive(true);
            destroyParticle.transform.position = theGM.nowPlayer.nowTile.transform.GetChild(0).position;
            destroyParticle.Play();

            // 건물의 Alpha 값을 조절해서 서서히 사라지는 듯한 연출
            while (buildingColor.a > 0f)
            {
                buildingColor.a -= 0.02f;
                tileColor.a -= 0.02f;

                theGM.nowPlayer.nowTile.buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
                theGM.nowPlayer.nowTile.signImg.GetComponent<SpriteRenderer>().color = tileColor;

                yield return new WaitForSeconds(0.02f);
            }

            // 파티클 비활성화
            destroyParticle.gameObject.SetActive(false);

            // 현재 타일의 소유주와 건물을 없앰
            theGM.nowPlayer.nowTile.ownPlayer = -1;
            theGM.nowPlayer.nowTile.building = theGM.buildings[0];

            // 0으로 감소시켰던 건물과 타일의 Alpha 값을 원상복구
            buildingColor.a = 1f;
            tileColor.a = 1f;
            theGM.nowPlayer.nowTile.buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
            theGM.nowPlayer.nowTile.signImg.GetComponent<SpriteRenderer>().color = tileColor;
        }
        biggerComplete = true;
    }

    // 투시효과 코루틴
    public IEnumerator PenetrateCoroutine()
    {
        theGM.penetrateComplete = false;

        // 상대방의 카드가 1장 이상이라면
        if (theGM.nowPlayer.againstPlayer.cards.Count > 0)
        {
            // 상대방 카드의 갯수만큼 showCardListObject에 복제하고 이미지를 맞춰줌
            for (int i = 0; i < theGM.nowPlayer.againstPlayer.cards.Count; i++)
            {
                var _card = Instantiate(theGM.onlyCardImg, new Vector3(0, 0, 0), Quaternion.identity, theGM.showCardListObject.transform);
                _card.AddComponent<RectTransform>();
                _card.transform.localPosition = new Vector2(0, 0);
                _card.transform.localScale = new Vector2(10, 10);
                _card.GetComponent<SpriteRenderer>().sprite = theGM.nowPlayer.againstPlayer.cards[i].cardImg;
            }

            // 3초동안 보여주고 이후에 showCardListObject에 복제했던 오브젝트를 파괴함
            yield return new WaitForSeconds(3f);

            for (int i = 0; i < theGM.nowPlayer.againstPlayer.cards.Count; i++)
            {
                Destroy(theGM.showCardListObject.transform.GetChild(i).gameObject);
            }
        }
        // 상대방의 카드가 없을 경우 안내메시지 출력
        else
        {
            theGM.textManager.ShowText("상대방의 카드가 없습니다!");

            yield return new WaitForSeconds(3f);

            theGM.textManager.HideText();
        }


        if (theGM.nowPlayer.cardParent.childCount < 8)
        {
            // 랜덤하게 카드번호를 추출
            Card newCard = theGM.cards[UnityEngine.Random.Range(0, theGM.cards.Length)]; //UnityEngine.Random.Range(0, theGM.cards.Length)

            // 팻말 아래 카드리스트에 복제하고 플레이어의 카드 목록에 추가함
            var _card = Instantiate(theGM.nowPlayer.cardPrefab, Vector3.zero, Quaternion.identity, theGM.nowPlayer.cardParent);
            _card.transform.localPosition = new Vector3(0f, 0f, 0f);
            theGM.nowPlayer.cards.Add(newCard); //팻말 아래 카드리스트 추가하는곳. ? 여기에 이거 들어가는거 맞나?


            StartCoroutine(ShowGetCard());
            theAudio.Play("GetCard_Sound");

            // 상세 카드 창에 카드 리스트 업데이트
            theGM.CardListUpdate();

            yield return new WaitUntil(() => isShowCard);
            isShowCard = false;

            // 만약 통행료면제 카드라면 카드효과를 즉시 활성화.
            if (newCard == theGM.cards[6])
            {
                theAudio.Play("TollExemption_Sound");

                exemptionParticle.transform.position = theGM.nowPlayer.transform.position;
                exemptionParticle.gameObject.SetActive(true);
                exemptionParticle.Play();

                yield return new WaitForSeconds(1f);

                exemptionParticle.gameObject.SetActive(false);

                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.ExemptionFlagSet,"");
                Backend.Match.SendDataToInGameRoom(data);
            }

            theGM.penetrateComplete = true;
        }
    }

    public IEnumerator LaserBeamCoroutine()
    {
        theGM.laserComplete = false;

        theGM.nowPlayer.blackBackground.SetActive(true);

        isSelectingLaser = true;

        for (int i = 0; i < theTile.tiles.Length; i++)
        {
            if (theGM.nowPlayer.playerId != theTile.tiles[i].ownPlayer && theTile.tiles[i].ownPlayer != -1)
                theTile.tiles[i].canTileSelect = true; //모든 카드 클릭 가능하도록 미리 클릭하고 다음턴에 해당 위치로 이동.
        }

        yield return new WaitUntil(() => theGM.seletedTile != null);

        //여기를 좀 어떻게 해야됨....
        isSelectingLaser = false;

        for (int i = 0; i < theTile.tiles.Length; i++)
        {
            theTile.tiles[i].canTileSelect = false; //다시 클릭 못하도록 변경
        }
        theGM.nowPlayer.blackBackground.SetActive(false);

        LaserData laserData = new(true, theGM.seletedTile.name);
        string jsondata = JsonUtility.ToJson(laserData);
        byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.Laser, jsondata);
        Backend.Match.SendDataToInGameRoom(sendData);
        // theAudio.Play("Laser_Sound");
        // // 건물과 타일의 컬러를 받아옴
        // Color buildingColor = theGM.seletedTile.GetComponent<Tile>().buildingImg.GetComponent<SpriteRenderer>().color;
        // Color tileColor = theGM.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color;

        // // 건물파괴 파티클을 활성화하고 위치를 현재 타일의 건물 위치로 옮긴 다음 파티클 실행
        // laserParticle.gameObject.SetActive(true);
        // laserParticle.transform.position = theGM.seletedTile.transform.GetChild(0).position;
        // laserParticle.Play();

        // // 건물의 Alpha 값을 조절해서 서서히 사라지는 듯한 연출
        // while (buildingColor.a > 0f)
        // {
        //     buildingColor.a -= 0.02f;
        //     tileColor.a -= 0.02f;

        //     theGM.seletedTile.GetComponent<Tile>().buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
        //     theGM.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;

        //     yield return new WaitForSeconds(0.02f);
        // }

        // // 파티클 비활성화
        // laserParticle.gameObject.SetActive(false);

        // // 현재 타일의 소유주와 건물을 없앰
        // theGM.seletedTile.GetComponent<Tile>().ownPlayer = -1;
        // theGM.seletedTile.GetComponent<Tile>().building = theGM.buildings[0];
        // theGM.seletedTile.GetComponent<Tile>().price = 0;

        // yield return new WaitForEndOfFrame();

        // // 0으로 감소시켰던 건물과 타일의 Alpha 값을 원상복구
        // buildingColor.a = 1f;
        // tileColor.a = 1f;
        // theGM.seletedTile.GetComponent<Tile>().buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
        // theGM.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;

        // theGM.seletedTile = null;

        // theGM.laserComplete = true;
    }

    public IEnumerator ShowGetCard()
    {
        // GameManager에 만들어놓은 카드이미지 프리팹을 카드를 띄워줄 위치에 있는 오브젝트에 복제
        // 플레이 화면 좌측하단에 표시하는 함수
        // 이후 위치값, 스케일값, 스프라이트 이미지를 변경함
        GameObject _card = Instantiate(theGM.onlyCardImg, Vector3.zero, Quaternion.identity, theGM.showCardObject.transform);
        _card.transform.localPosition = new Vector3(0f, 0f, 0f);
        _card.transform.localScale = new Vector3(20f, 20f, 20f);
        yield return new WaitUntil(() => theGM.nowPlayer.cards.Count > 0);
        _card.GetComponent<SpriteRenderer>().sprite = theGM.nowPlayer.cards[theGM.nowPlayer.cards.Count - 1].cardImg;

        // 3초 대기 이후 보여줬던 카드를 파괴하고 코루틴 탈출
        yield return new WaitForSeconds(3f);

        Destroy(_card);
        isShowCard = true;
    }

    public IEnumerator CardProvideCoroutine()
    {
        isGetCard = false;
        if (theGM.nowPlayer.cards.Count < 8)
        {
            // 랜덤하게 카드번호를 추출
            Card newCard = theGM.cards[7];
            // Card newCard = theGM.cards[UnityEngine.Random.Range(0, theGM.cards.Length)];//Test

            // 팻말 아래 카드리스트에 복제하고 플레이어의 카드 목록에 추가함

            // var _card = Instantiate(theGM.nowPlayer.cardPrefab, Vector3.zero, Quaternion.identity, theGM.nowPlayer.cardParent); //EventManager로 이동.
            // _card.transform.localPosition = new Vector3(0f, 0f, 0f); //EventManager로 이동.

            CardData cardData = new(newCard);
            string jsonData = JsonUtility.ToJson(cardData);
            byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.CardListAdd, jsonData);
            Backend.Match.SendDataToInGameRoom(sendData);
            // theGM.nowPlayer.cards.Add(newCard); (EventManager로 이동.)

            StartCoroutine(ShowGetCard());
            theAudio.Play("GetCard_Sound");
            yield return new WaitUntil(() => isShowCard);
            isShowCard = false;

            // 만약 통행료면제 카드라면 카드효과를 즉시 활성화.
            if (newCard == theGM.cards[6])
            {
                theAudio.Play("TollExemption_Sound");

                exemptionParticle.transform.position = theGM.nowPlayer.transform.position;
                exemptionParticle.gameObject.SetActive(true);
                exemptionParticle.Play();

                yield return new WaitForSeconds(1f);

                exemptionParticle.gameObject.SetActive(false);
                // theGM.nowPlayer.exemptionFlag = true;
                byte[] senddata = ParsingManager.Instance.ParsingSendData(ParsingType.ExemptionFlagSet,"");
                Backend.Match.SendDataToInGameRoom(senddata);
            }
            //카드 받고나서 턴 넘기는 부분
            byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.NextTurn, "");
            Backend.Match.SendDataToInGameRoom(data);
        }
        isGetCard = true;
    }

    public IEnumerator LaserCoroutine(){
        theAudio.Play("Laser_Sound");
        // 건물과 타일의 컬러를 받아옴
        Color buildingColor = theGM.seletedTile.GetComponent<Tile>().buildingImg.GetComponent<SpriteRenderer>().color;
        Color tileColor = theGM.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color;

        // 건물파괴 파티클을 활성화하고 위치를 현재 타일의 건물 위치로 옮긴 다음 파티클 실행
        GameManager.Instance.laserParticle.gameObject.SetActive(true);
        GameManager.Instance.laserParticle.transform.position = theGM.seletedTile.transform.GetChild(0).position;
        GameManager.Instance.laserParticle.Play();

        // 건물의 Alpha 값을 조절해서 서서히 사라지는 듯한 연출
        while (buildingColor.a > 0f)
        {
            buildingColor.a -= 0.02f;
            tileColor.a -= 0.02f;

            theGM.seletedTile.GetComponent<Tile>().buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
            theGM.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;

            yield return new WaitForSeconds(0.02f);
        }

        // 파티클 비활성화
        GameManager.Instance.laserParticle.gameObject.SetActive(false);

        // 현재 타일의 소유주와 건물을 없앰
        theGM.seletedTile.GetComponent<Tile>().ownPlayer = -1;
        theGM.seletedTile.GetComponent<Tile>().building = theGM.buildings[0];
        theGM.seletedTile.GetComponent<Tile>().price = 0;

        yield return new WaitForEndOfFrame();

        // 0으로 감소시켰던 건물과 타일의 Alpha 값을 원상복구
        buildingColor.a = 1f;
        tileColor.a = 1f;
        theGM.seletedTile.GetComponent<Tile>().buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
        theGM.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;

        theGM.seletedTile = null;
        theGM.laserComplete = true;
    }
}