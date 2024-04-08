using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    TurnSignScript theTurnSign;
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
    public ParticleSystem laserParticle;

    void Start()
    {
        theTurnSign = FindObjectOfType<TurnSignScript>();
        theDice = FindObjectOfType<DiceSystem>();
    }

    // 획득한 카드에 마우스를 올려놓았을 때
    private void OnMouseEnter()
    {
        if (theTurnSign.cursorPos == 1 && !theDice.isDrag)
        {
            AudioManager.Instance.Play("SelectCard_Sound");

            this.transform.localScale = new Vector3(14f, 14f, 1f);
            this.transform.position += Vector3.up * upPos;
            this.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }

    // 획득한 카드에 마우스를 올렸다가 떨어트렸을 때
    private void OnMouseExit()
    {
        if (theTurnSign.cursorPos == 1 && !theDice.isDrag)
        {
            this.transform.localScale = new Vector3(7f, 7f, 1f);
            this.transform.position += Vector3.down * upPos;
            this.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
    }

    // 획득한 카드를 클릭해서 사용할 때
    private void OnMouseDown()
    {
        if (theTurnSign.cursorPos == 1 && GameManager.Instance.myCharactor.myTurn)
        {
            AudioManager.Instance.Play("UseCard_Sound");

            if (cardInfo.cardCode == 1 && !GameManager.Instance.nowPlayer.highSpeedFlag)
            {
                CardClickData cData = new(1, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);

                DestroyCard();
            }

            else if (cardInfo.cardCode == 2 && !GameManager.Instance.nowPlayer.invisibleFlag)
            {
                CardClickData cData = new(2, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);

                DestroyCard();
            }

            else if (cardInfo.cardCode == 3 && !GameManager.Instance.nowPlayer.biggerFlag)
            {
                CardClickData cData = new(3, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);

                DestroyCard();
            }

            else if (cardInfo.cardCode == 4 && !GameManager.Instance.nowPlayer.lowerDiceFlag)
            {
                CardClickData cData = new(4, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);

                GameManager.Instance.nowPlayer.lowerDiceFlag = true;
                GameManager.Instance.nowPlayer.higherDiceFlag = false;

                DestroyCard();
            }

            else if (cardInfo.cardCode == 5 && !GameManager.Instance.nowPlayer.higherDiceFlag)
            {
                CardClickData cData = new(5, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);

                GameManager.Instance.nowPlayer.higherDiceFlag = true;
                GameManager.Instance.nowPlayer.lowerDiceFlag = false;

                DestroyCard();
            }

            else if (cardInfo.cardCode == 7 && !GameManager.Instance.nowPlayer.laserFlag && GameManager.Instance.laserComplete)
            {
                for (int i = 0; i < TileManager.Instance.tiles.Length; i++)
                {
                    // 상대방 소유의 타일 체크
                    if (TileManager.Instance.tiles[i].ownPlayer == GameManager.Instance.nowPlayer.againstPlayer.playerId)
                    {
                        CardClickData cData = new(7, GameManager.Instance.nowPlayer.playerId);
                        string jsonData = JsonUtility.ToJson(cData);
                        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                        Backend.Match.SendDataToInGameRoom(data);

                        DestroyCard();
                        break;
                    }
                }
            }
        }
    }

    public void DestroyCard()
    {
        CardDestroyData destroyData = new(this.gameObject, cardInfo.cardCode);
        string jsonData = JsonUtility.ToJson(destroyData);
        byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.CardDestory, jsonData);
        Backend.Match.SendDataToInGameRoom(sendData);
    }

    public void HighSpeedMove()
    {
        GameManager.Instance.nowPlayer.diceNum = GameManager.Instance.nowPlayer.diceNum * 2;
        GameManager.Instance.nowPlayer.moveSpeed = 6f;
        highMoveParticle.gameObject.SetActive(true);
        highMoveParticle.Play();
    }

    public void EndHighSpeedMove()
    {
        GameManager.Instance.nowPlayer.highSpeedFlag = false;
        GameManager.Instance.nowPlayer.moveSpeed = 4f;
        highMoveParticle.Stop();
        highMoveParticle.gameObject.SetActive(false);
    }

    public IEnumerator InvisibleThief()
    {
        if (GameManager.Instance.nowPlayer.cards.Count < 8)
        {
            byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.InvisibleThief, "");
            Backend.Match.SendDataToInGameRoom(sendData);

            // 랜덤 숫자를 서버에서 처리할 때까지 대기
            yield return new WaitUntil(() => GameManager.Instance.invisibleCardNum != -1);

            AudioManager.Instance.Play("InvisibleThief_Sound");

            // 천사꼬꼬 카드를 가져왔다면 
            if (GameManager.Instance.nowPlayer.againstPlayer.cards[GameManager.Instance.invisibleCardNum] == GameManager.Instance.cards[5])
            {
                GameManager.Instance.nowPlayer.exemptionFlag = true;
                GameManager.Instance.nowPlayer.againstPlayer.exemptionFlag = false;
            }

            // 파티클 실행
            InvisibleParticle.gameObject.SetActive(true);
            InvisibleParticle.transform.position = GameManager.Instance.nowPlayer.transform.position;
            InvisibleParticle.Play();

            // 플레이어 카드 리스트 업데이트
            GameManager.Instance.nowPlayer.cards.Add(GameManager.Instance.nowPlayer.againstPlayer.cards[GameManager.Instance.invisibleCardNum]);
            GameManager.Instance.nowPlayer.againstPlayer.cards.RemoveAt(GameManager.Instance.invisibleCardNum);
            GameManager.Instance.CardListUpdate();

            GameObject dCard = GameManager.Instance.nowPlayer.againstPlayer.cardParent.GetChild(0).gameObject;
            dCard.transform.SetParent(GameManager.Instance.nowPlayer.cardParent);
            dCard.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.nowPlayer.cardPrefab.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void EndInvisibleThief()
    {
        GameManager.Instance.nowPlayer.invisibleFlag = false;
        InvisibleParticle.Stop();
        InvisibleParticle.gameObject.SetActive(false);
        StartCoroutine(GameManager.Instance.nowPlayer.SetPlayerTransparency("Visible"));
    }

    public IEnumerator TollExemption()
    {
        for (int i = 0; i < GameManager.Instance.nowPlayer.cards.Count; i++)
        {
            // 천사꼬꼬 카드를 찾아서 삭제
            if (GameManager.Instance.nowPlayer.cards[i] == GameManager.Instance.cards[5])
            {
                GameManager.Instance.nowPlayer.cards.RemoveAt(i);
                Destroy(GameManager.Instance.nowPlayer.cardParent.GetChild(0).gameObject);
                break;
            }
        }

        AudioManager.Instance.Play("TollExemption_Sound");

        // 파티클 실행
        exemptionParticle.transform.position = GameManager.Instance.nowPlayer.transform.position;
        exemptionParticle.gameObject.SetActive(true);
        exemptionParticle.Play();
        yield return new WaitForSeconds(1f);
        exemptionParticle.gameObject.SetActive(false);

        GameManager.Instance.nowPlayer.exemptionFlag = false;
        yield return null;
    }

    public IEnumerator BiggerCoroutine()
    {
        GameManager.Instance.nowPlayer.biggerFlag = false;

        // 상대방 소유의 타일이라면 거대화 꼬꼬 효과 발동
        if (GameManager.Instance.nowPlayer.nowTile.ownPlayer != -1 && GameManager.Instance.nowPlayer.playerId != GameManager.Instance.nowPlayer.nowTile.ownPlayer)
        {
            Color buildingColor = GameManager.Instance.nowPlayer.nowTile.buildingImg.GetComponent<SpriteRenderer>().color;
            Color tileColor = GameManager.Instance.nowPlayer.nowTile.signImg.GetComponent<SpriteRenderer>().color;

            AudioManager.Instance.Play("BuildingDestroy_Sound");

            destroyParticle.gameObject.SetActive(true);
            destroyParticle.transform.position = GameManager.Instance.nowPlayer.nowTile.transform.GetChild(0).position;
            destroyParticle.Play();

            // 타일과 건물의 색상을 점차 투명하게 변경
            while (buildingColor.a > 0f)
            {
                buildingColor.a -= 0.02f;
                tileColor.a -= 0.02f;

                GameManager.Instance.nowPlayer.nowTile.buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
                GameManager.Instance.nowPlayer.nowTile.signImg.GetComponent<SpriteRenderer>().color = tileColor;

                yield return new WaitForSeconds(0.02f);
            }

            destroyParticle.gameObject.SetActive(false);

            // 타일의 소유주 변경
            GameManager.Instance.nowPlayer.nowTile.ownPlayer = -1;
            GameManager.Instance.nowPlayer.nowTile.building = GameManager.Instance.buildings[0];

            // 타일과 건물의 색상 원상복구
            buildingColor.a = 1f;
            tileColor.a = 1f;
            GameManager.Instance.nowPlayer.nowTile.buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
            GameManager.Instance.nowPlayer.nowTile.signImg.GetComponent<SpriteRenderer>().color = tileColor;
        }

        biggerComplete = true;
    }

    public IEnumerator SelectLaserCoroutine()
    {
        GameManager.Instance.laserComplete = false;

        GameManager.Instance.nowPlayer.blackBackground.SetActive(true);

        isSelectingLaser = true;

        // 상대방 소유의 타일을 클릭 가능하도록 변경
        for (int i = 0; i < TileManager.Instance.tiles.Length; i++)
        {
            if (GameManager.Instance.nowPlayer.playerId != TileManager.Instance.tiles[i].ownPlayer && TileManager.Instance.tiles[i].ownPlayer != -1)
                TileManager.Instance.tiles[i].canTileSelect = true;
        }

        yield return new WaitUntil(() => GameManager.Instance.seletedTile != null);

        isSelectingLaser = false;

        // 클릭 이후 다시 클릭이 불가능하도록 변경
        for (int i = 0; i < TileManager.Instance.tiles.Length; i++)
        {
            TileManager.Instance.tiles[i].canTileSelect = false;
        }

        GameManager.Instance.nowPlayer.blackBackground.SetActive(false);

        LaserData laserData = new(true, GameManager.Instance.seletedTile.name);
        string jsondata = JsonUtility.ToJson(laserData);
        byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.Laser, jsondata);
        Backend.Match.SendDataToInGameRoom(sendData);
    }

    public IEnumerator LaserCoroutine()
    {
        GameManager.Instance.nowPlayer.laserFlag = false;

        AudioManager.Instance.Play("Laser_Sound");

        laserParticle.gameObject.SetActive(true);
        laserParticle.transform.position = GameManager.Instance.seletedTile.transform.GetChild(0).position;
        laserParticle.Play();

        // 건물과 타일의 색상을 가져옴
        Color buildingColor = GameManager.Instance.seletedTile.GetComponent<Tile>().buildingImg.GetComponent<SpriteRenderer>().color;
        Color tileColor = GameManager.Instance.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color;

        // Alpha값을 줄여서 서서히 사라지는 듯한 연출
        while (buildingColor.a > 0f)
        {
            buildingColor.a -= 0.02f;
            tileColor.a -= 0.02f;

            GameManager.Instance.seletedTile.GetComponent<Tile>().buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
            GameManager.Instance.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;

            yield return new WaitForSeconds(0.02f);
        }

        laserParticle.gameObject.SetActive(false);

        // 타일의 소유주 초기화
        GameManager.Instance.seletedTile.GetComponent<Tile>().ownPlayer = -1;
        GameManager.Instance.seletedTile.GetComponent<Tile>().building = GameManager.Instance.buildings[0];
        GameManager.Instance.seletedTile.GetComponent<Tile>().price = 0;

        yield return new WaitForEndOfFrame();

        // Alpha값 원상복구
        buildingColor.a = 1f;
        tileColor.a = 1f;
        GameManager.Instance.seletedTile.GetComponent<Tile>().buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
        GameManager.Instance.seletedTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;

        GameManager.Instance.seletedTile = null;
        GameManager.Instance.laserComplete = true;
    }

    public IEnumerator ShowGetCard(int _randomNum)
    {
        GameObject card = Instantiate(GameManager.Instance.onlyCardImg, Vector3.zero, Quaternion.identity, GameManager.Instance.showCardObject.transform);
        card.transform.localPosition = new Vector3(0f, 0f, 0f);
        card.transform.localScale = new Vector3(20f, 20f, 20f);

        card.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.cards[_randomNum].cardImg;

        yield return new WaitForSeconds(3f);

        Destroy(card);
        isShowCard = true;
    }

    public IEnumerator CardProvideCoroutine()
    {
        isGetCard = false;

        if (GameManager.Instance.nowPlayer.cards.Count < 8)
        {
            int randomNum = Random.Range(0, GameManager.Instance.cards.Length);
            Card newCard = GameManager.Instance.cards[randomNum];

            CardData cardData = new(newCard);
            string jsonData = JsonUtility.ToJson(cardData);
            byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.CardListAdd, jsonData);
            Backend.Match.SendDataToInGameRoom(sendData);

            AudioManager.Instance.Play("GetCard_Sound");

            StartCoroutine(ShowGetCard(randomNum));

            yield return new WaitUntil(() => isShowCard);

            isShowCard = false;

            // 통행료 면제 카드를 획득했을 때
            if (newCard == GameManager.Instance.cards[5])
            {
                AudioManager.Instance.Play("TollExemption_Sound");

                exemptionParticle.transform.position = GameManager.Instance.nowPlayer.transform.position;
                exemptionParticle.gameObject.SetActive(true);
                exemptionParticle.Play();

                yield return new WaitForSeconds(1f);

                exemptionParticle.gameObject.SetActive(false);

                byte[] senddata = ParsingManager.Instance.ParsingSendData(ParsingType.ExemptionFlagSet, "");
                Backend.Match.SendDataToInGameRoom(senddata);
            }
        }
        isGetCard = true;
    }
}