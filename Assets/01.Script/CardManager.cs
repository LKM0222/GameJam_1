using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    GameManager theGM;
    TileManager theTile;
    TurnSignScript theTSI;
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
    public static CardManager Instance
    {
        get
        {
            if (_instance == null)
            {
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
        theDice = FindObjectOfType<DiceSystem>();
    }

    // 획득한 카드에 마우스를 올려놓았을 때
    private void OnMouseEnter()
    {
        if (theTSI.cursorPos == 1 && !theDice.isDrag)
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
        if (theTSI.cursorPos == 1 && !theDice.isDrag)
        {
            this.transform.localScale = new Vector3(7f, 7f, 1f);
            this.transform.position += Vector3.down * upPos;
            this.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
    }

    // 획득한 카드를 클릭해서 사용할 때
    private void OnMouseDown()
    {
        if (theTSI.cursorPos == 1 && GameManager.Instance.myCharactor.myTurn)
        {
            AudioManager.Instance.Play("UseCard_Sound");

            if (cardInfo.cardCode == 1 && !theGM.nowPlayer.highSpeedFlag)
            {
                CardClickData cData = new(1, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);

                DestroyCard();
            }

            else if (cardInfo.cardCode == 2 && !theGM.nowPlayer.invisibleFlag)
            {
                CardClickData cData = new(2, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);

                DestroyCard();
            }

            else if (cardInfo.cardCode == 3 && !theGM.nowPlayer.biggerFlag)
            {
                CardClickData cData = new(3, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);

                DestroyCard();
            }

            else if (cardInfo.cardCode == 4 && !theGM.nowPlayer.lowerDiceFlag)
            {
                CardClickData cData = new(4, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);

                theGM.nowPlayer.lowerDiceFlag = true;
                theGM.nowPlayer.higherDiceFlag = false;

                DestroyCard();
            }

            else if (cardInfo.cardCode == 5 && !theGM.nowPlayer.higherDiceFlag)
            {
                CardClickData cData = new(5, GameManager.Instance.nowPlayer.playerId);
                string jsonData = JsonUtility.ToJson(cData);
                byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.CardClick, jsonData);
                Backend.Match.SendDataToInGameRoom(data);

                theGM.nowPlayer.higherDiceFlag = true;
                theGM.nowPlayer.lowerDiceFlag = false;

                DestroyCard();
            }

            else if (cardInfo.cardCode == 7 && !theGM.nowPlayer.laserFlag && theGM.laserComplete)
            {
                for (int i = 0; i < theTile.tiles.Length; i++)
                {
                    // 상대방 소유의 타일 체크
                    if (theTile.tiles[i].ownPlayer == theGM.nowPlayer.againstPlayer.playerId)
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

    public IEnumerator InvisibleThief()
    {
        if (theGM.nowPlayer.cards.Count < 8)
        {
            byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.InvisibleThief, "");
            Backend.Match.SendDataToInGameRoom(sendData);

            // 랜덤 숫자를 서버에서 처리할 때까지 대기
            yield return new WaitUntil(() => GameManager.Instance.invisibleCardNum != -1);

            AudioManager.Instance.Play("InvisibleThief_Sound");

            // 천사꼬꼬 카드를 가져왔다면 
            if (theGM.nowPlayer.againstPlayer.cards[GameManager.Instance.invisibleCardNum] == theGM.cards[5])
            {
                theGM.nowPlayer.exemptionFlag = true;
                theGM.nowPlayer.againstPlayer.exemptionFlag = false;
            }

            // 파티클 실행
            InvisibleParticle.gameObject.SetActive(true);
            InvisibleParticle.transform.position = theGM.nowPlayer.transform.position;
            InvisibleParticle.Play();

            // 플레이어 카드 리스트 업데이트
            theGM.nowPlayer.cards.Add(theGM.nowPlayer.againstPlayer.cards[GameManager.Instance.invisibleCardNum]);
            theGM.nowPlayer.againstPlayer.cards.RemoveAt(GameManager.Instance.invisibleCardNum);
            theGM.CardListUpdate();

            GameObject dCard = theGM.nowPlayer.againstPlayer.cardParent.GetChild(0).gameObject;
            dCard.transform.SetParent(theGM.nowPlayer.cardParent);
            dCard.GetComponent<SpriteRenderer>().sprite = theGM.nowPlayer.cardPrefab.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void EndInvisibleThief()
    {
        theGM.nowPlayer.invisibleFlag = false;
        InvisibleParticle.Stop();
        InvisibleParticle.gameObject.SetActive(false);
        StartCoroutine(theGM.nowPlayer.SetPlayerTransparency("Visible"));
    }

    public IEnumerator TollExemption()
    {
        for (int i = 0; i < theGM.nowPlayer.cards.Count; i++)
        {
            // 천사꼬꼬 카드를 찾아서 삭제
            if (theGM.nowPlayer.cards[i] == theGM.cards[5])
            {
                theGM.nowPlayer.cards.RemoveAt(i);
                Destroy(theGM.nowPlayer.cardParent.GetChild(0).gameObject);
                break;
            }
        }

        AudioManager.Instance.Play("TollExemption_Sound");

        // 파티클 실행
        exemptionParticle.transform.position = theGM.nowPlayer.transform.position;
        exemptionParticle.gameObject.SetActive(true);
        exemptionParticle.Play();
        yield return new WaitForSeconds(1f);
        exemptionParticle.gameObject.SetActive(false);

        theGM.nowPlayer.exemptionFlag = false;
        yield return null;
    }

    public IEnumerator BiggerCoroutine()
    {
        theGM.nowPlayer.biggerFlag = false;

        // 상대방 소유의 타일이라면 거대화 꼬꼬 효과 발동
        if (theGM.nowPlayer.nowTile.ownPlayer != -1 && theGM.nowPlayer.playerId != theGM.nowPlayer.nowTile.ownPlayer)
        {
            Color buildingColor = theGM.nowPlayer.nowTile.buildingImg.GetComponent<SpriteRenderer>().color;
            Color tileColor = theGM.nowPlayer.nowTile.signImg.GetComponent<SpriteRenderer>().color;

            AudioManager.Instance.Play("BuildingDestroy_Sound");

            destroyParticle.gameObject.SetActive(true);
            destroyParticle.transform.position = theGM.nowPlayer.nowTile.transform.GetChild(0).position;
            destroyParticle.Play();

            // 타일과 건물의 색상을 점차 투명하게 변경
            while (buildingColor.a > 0f)
            {
                buildingColor.a -= 0.02f;
                tileColor.a -= 0.02f;

                theGM.nowPlayer.nowTile.buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
                theGM.nowPlayer.nowTile.signImg.GetComponent<SpriteRenderer>().color = tileColor;

                yield return new WaitForSeconds(0.02f);
            }

            destroyParticle.gameObject.SetActive(false);

            // 타일의 소유주 변경
            theGM.nowPlayer.nowTile.ownPlayer = -1;
            theGM.nowPlayer.nowTile.building = theGM.buildings[0];

            // 타일과 건물의 색상 원상복구
            buildingColor.a = 1f;
            tileColor.a = 1f;
            theGM.nowPlayer.nowTile.buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
            theGM.nowPlayer.nowTile.signImg.GetComponent<SpriteRenderer>().color = tileColor;
        }

        biggerComplete = true;
    }

    public IEnumerator SelectLaserCoroutine()
    {
        theGM.laserComplete = false;

        theGM.nowPlayer.blackBackground.SetActive(true);

        isSelectingLaser = true;

        // 상대방 소유의 타일을 클릭 가능하도록 변경
        for (int i = 0; i < theTile.tiles.Length; i++)
        {
            if (theGM.nowPlayer.playerId != theTile.tiles[i].ownPlayer && theTile.tiles[i].ownPlayer != -1)
                theTile.tiles[i].canTileSelect = true;
        }

        yield return new WaitUntil(() => theGM.seletedTile != null);

        isSelectingLaser = false;

        // 클릭 이후 다시 클릭이 불가능하도록 변경
        for (int i = 0; i < theTile.tiles.Length; i++)
        {
            theTile.tiles[i].canTileSelect = false;
        }

        theGM.nowPlayer.blackBackground.SetActive(false);

        LaserData laserData = new(true, theGM.seletedTile.name);
        string jsondata = JsonUtility.ToJson(laserData);
        byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.Laser, jsondata);
        Backend.Match.SendDataToInGameRoom(sendData);
    }

    public IEnumerator ShowGetCard(int _randomNum)
    {
        GameObject card = Instantiate(theGM.onlyCardImg, Vector3.zero, Quaternion.identity, theGM.showCardObject.transform);
        card.transform.localPosition = new Vector3(0f, 0f, 0f);
        card.transform.localScale = new Vector3(20f, 20f, 20f);

        card.GetComponent<SpriteRenderer>().sprite = theGM.cards[_randomNum].cardImg;

        yield return new WaitForSeconds(3f);

        Destroy(card);
        isShowCard = true;
    }

    public IEnumerator CardProvideCoroutine()
    {
        isGetCard = false;

        if (theGM.nowPlayer.cards.Count < 8)
        {
            int randomNum = Random.Range(0, theGM.cards.Length);
            Card newCard = theGM.cards[randomNum];

            CardData cardData = new(newCard);
            string jsonData = JsonUtility.ToJson(cardData);
            byte[] sendData = ParsingManager.Instance.ParsingSendData(ParsingType.CardListAdd, jsonData);
            Backend.Match.SendDataToInGameRoom(sendData);

            AudioManager.Instance.Play("GetCard_Sound");

            StartCoroutine(ShowGetCard(randomNum));

            yield return new WaitUntil(() => isShowCard);

            isShowCard = false;

            if (newCard == theGM.cards[5])
            {
                AudioManager.Instance.Play("TollExemption_Sound");

                exemptionParticle.transform.position = theGM.nowPlayer.transform.position;
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