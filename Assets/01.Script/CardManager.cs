using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CardManager : MonoBehaviour
{
    GameManager theGM;
    TileManager theTile;

    // 카드의 정보
    public Card cardInfo;

    // 카드를 위아래로 움직일 수치
    [SerializeField] float upPos;

    public bool completeFlag;
    public ParticleSystem destroyParticle;
    public ParticleSystem laserParticle;
    public ParticleSystem highMoveParticle;


    // Start is called before the first frame update
    void Start()
    {
        theGM = FindObjectOfType<GameManager>();
        theTile = FindObjectOfType<TileManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 해당 스크립트가 붙은 오브젝트(획득한 카드)에 마우스를 올려놓았을 때 호출
    private void OnMouseEnter()
    {
        // 땅, 건물 구매 UI가 꺼져있을때
        if (!theGM.UIFlag)
        {
            // upPos만큼 Position을 올리고 Scale을 늘림
            this.transform.localScale = new Vector3(14f, 14f, 1f);
            this.transform.position += Vector3.up * upPos;
            this.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }

        // 카드를 쓰는 턴일때만 작동하게(주사위를 굴리는 도중, 이동하는 도중은 작동하지 않게 해야함)
    }

    // 해당 스크립트가 붙은 오브젝트(획득한 카드)에 마우스를 올렸다가 떨어트렸을 때 호출
    private void OnMouseExit()
    {
        // 땅, 건물 구매 UI가 꺼져있을때
        if (!theGM.UIFlag)
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
        // 땅, 건물 구매 UI가 꺼져있을때
        if (!theGM.UIFlag)
        {
            // cardCode가 1이라면 고속이동
            if (cardInfo.cardCode == 1)
            {
                theGM.nowPlayer.highSpeedFlag = true;
            }
            // cardCode가 2라면 투명도둑
            else if (cardInfo.cardCode == 2)
            {
                theGM.nowPlayer.invisibleFlag = true;
            }
            // cardCode가 3이라면 거대화꼬꼬
            else if (cardInfo.cardCode == 3)
            {
                theGM.nowPlayer.biggerFlag = true;
            }
            // cardCode가 4라면 투시
            else if (cardInfo.cardCode == 4)
            {
                theGM.nowPlayer.toosiFlag = true;
            }
            // cardCode가 5라면 주사위컨트롤(하)
            else if (cardInfo.cardCode == 5)
            {
                theGM.nowPlayer.lowerDiceFlag = true;
            }
            // cardCode가 6이라면 주사위컨트롤(상)
            else if (cardInfo.cardCode == 6)
            {
                theGM.nowPlayer.higherDiceFlag = true;
            }
            // // cardCode가 7이라면 통행료면제
            // else if (cardInfo.cardCode == 7)
            // {
            //     theGM.nowPlayer.exemptionFlag = true;
            // }
            // cardCode가 8이라면 레이저빔
            else if (cardInfo.cardCode == 8)
            {
                theGM.nowPlayer.laserFlag = true;
            }

            // 카드 오브젝트 삭제 및 플레이어가 가지고 있는 카드 리스트에서도 삭제
            Destroy(this.gameObject);
            Destroy(theGM.nowPlayer.cardParent.GetChild(0).gameObject);
            theGM.nowPlayer.cards.Remove(this.cardInfo);
            print("효과 발동" + transform.name);
        }
    }

    public void HighSpeedMove()
    {
        print("고속이동 사용");
        theGM.nowPlayer.diceNum = theGM.nowPlayer.diceNum * 2;
        theGM.nowPlayer.speed = 6f;
    }

    public void InvisibleThief()
    {
        print("투명도둑 사용");
        // 상대가 가진 카드를 랜덤으로 골라서 현재 플레이어 카드에 추가하고 상대 플레이어 카드에는 삭제
        int randomCard = UnityEngine.Random.Range(0, theGM.nowPlayer.againstPlayer.cards.Count);
        theGM.nowPlayer.cards.Add(theGM.nowPlayer.againstPlayer.cards[randomCard]);
        theGM.nowPlayer.againstPlayer.cards.RemoveAt(randomCard);

        // 상대방 카드 UI를 내 카드 UI로 옮겨오고 스프라이트 이미지도 변경
        GameObject dCard = theGM.nowPlayer.againstPlayer.cardParent.GetChild(randomCard).gameObject;
        dCard.transform.parent = theGM.nowPlayer.cardParent;
        dCard.GetComponent<SpriteRenderer>().sprite = theGM.nowPlayer.cardPrefab.GetComponent<SpriteRenderer>().sprite;

        // 만약 뺏어온 카드가 통행료면제 카드라면 플래그를 서로 바꿔줌
        if (theGM.nowPlayer.againstPlayer.cards[randomCard] == theGM.cards[6])
        {
            theGM.nowPlayer.exemptionFlag = true;
            theGM.nowPlayer.againstPlayer.exemptionFlag = false;
        }

        // invisibleFlag를 False로 변환
        theGM.nowPlayer.invisibleFlag = false;
    }

    public void BiggerChicken()
    {
        print("거대화꼬꼬 사용");
        // 만약 현재 타일의 소유주가 자신이 아니라면 코루틴을 실행하고 자신의 소유라면 completeFlag를 true로 주어 기존 코드를 실행
        if (theGM.nowPlayer.nowTile.ownPlayer != theGM.nowPlayer.playerId)
        {
            StartCoroutine(BiggerCoroutine());
        }
        else
        {
            completeFlag = true;
        }
    }

    public void Penetrate()
    {
        print("투시 사용");
        StartCoroutine(PenetrateCoroutine());
    }

    public void LowerDiceControl()
    {
        print("주사위컨트롤(하) 사용");
        theGM.nowPlayer.diceNum = Random.Range(1, 5);
        theGM.nowPlayer.lowerDiceFlag = false;
    }

    public void HigherDiceControll()
    {
        print("주사위컨트롤(상) 사용");
        theGM.nowPlayer.diceNum = Random.Range(5, 9);
        theGM.nowPlayer.higherDiceFlag = false;
    }


    public void TollExemption()
    {
        print("통행료 면제 카드 발동!");

        // 현재 자신의 카드 중에서 통행료 면제 카드를 찾아서 파괴함
        for (int i = 0; i < theGM.nowPlayer.cards.Count; i++)
        {
            if (theGM.nowPlayer.cards[i] == theGM.cards[6])
            {
                theGM.nowPlayer.cards.RemoveAt(i);
                Destroy(theGM.nowPlayer.cardParent.GetChild(0).gameObject);
            }
        }
        // 카드 효과를 사용했으니 flag를 false로 바꿔줌
        theGM.nowPlayer.exemptionFlag = false;
    }

    public void LaserBeam()
    {
        print("레이저 빔 사용");
        StartCoroutine(LaserBeamCoroutine());
    }

    // 거대화 사용효과 및 애니메이션 코루틴
    IEnumerator BiggerCoroutine()
    {
        // 현재 타일이 빈 땅도 아니고, 현재 플레이어 id와 ownPlayer가 다르다면 상대방의 땅
        if (theGM.nowPlayer.nowTile.ownPlayer != -1 && theGM.nowPlayer.playerId != theGM.nowPlayer.nowTile.ownPlayer)
        {
            // 건물과 타일의 컬러를 받아옴
            Color buildingColor = theGM.nowPlayer.nowTile.buildingImg.GetComponent<SpriteRenderer>().color;
            Color tileColor = theGM.nowPlayer.nowTile.signImg.GetComponent<SpriteRenderer>().color;

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
            // theGM.nowPlayer.nowTile.buildingImg = null;

            yield return new WaitForEndOfFrame();

            // 0으로 감소시켰던 건물과 타일의 Alpha 값을 원상복구
            buildingColor.a = 1f;
            tileColor.a = 1f;
            theGM.nowPlayer.nowTile.buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
            theGM.nowPlayer.nowTile.signImg.GetComponent<SpriteRenderer>().color = tileColor;

        }
        // 꼬꼬의 크기를 다시 줄이고 거대화 효과 플래그를 false로, 애니메이션을 위한 코루틴 제어 플래그도 flag로 바꿔줌
        // 크기가 서서히 커지는 듯한 연출
        Vector3 scale = new Vector3(2f, 2f, 0);
        while (true)
        {
            scale -= new Vector3(0.1f, 0.1f, 0);
            theGM.nowPlayer.gameObject.transform.localScale = scale;
            yield return new WaitForSeconds(0.1f);

            if (scale.x <= 1.5f)
            {
                break;
            }
        }
        theGM.nowPlayer.biggerFlag = false;
        completeFlag = true;
    }

    // 투시효과 코루틴
    IEnumerator PenetrateCoroutine()
    {
        // 상대방의 카드가 1장 이상이라면
        if (theGM.nowPlayer.againstPlayer.cards.Count > 0)
        {
            // 상대방 카드의 갯수만큼 showCardListObject에 복제하고 이미지를 맞춰줌
            for (int i = 0; i < theGM.nowPlayer.againstPlayer.cards.Count; i++)
            {
                var _card = Instantiate(theGM.onlyCardImg, new Vector3(0, 0, 0), Quaternion.identity, theGM.showCardListObject.transform);
                _card.transform.localPosition = new Vector2(0, 0);
                _card.transform.localScale = new Vector2(10, 10);
                _card.GetComponent<SpriteRenderer>().sprite = theGM.nowPlayer.againstPlayer.cards[i].cardImg;
            }

            // 3초동안 보여주고 이후에 showCardListObject에 복제했던 오브젝트를 파괴함
            yield return new WaitForSeconds(3f);

            for (int i = 0; i < theGM.nowPlayer.againstPlayer.cards.Count; i++)
            {
                Destroy(theGM.showCardListObject.transform.GetChild(0).gameObject);
            }
        }

        // 상대방의 카드 갯수와 상관없이 투시카드를 사용하면 랜덤하게 카드를 한장 습득
        theGM.nowPlayer.cards.Add(theGM.cards[UnityEngine.Random.Range(0, 4)]);
        // 팻말 아래에 카드 생성
        Instantiate(theGM.nowPlayer.cardPrefab, theGM.nowPlayer.cardParent.transform.position, Quaternion.identity, theGM.nowPlayer.cardParent);
        // 상세 카드 창에 카드 리스트 업데이트
        theGM.CardListUpdate();
    }

    IEnumerator LaserBeamCoroutine()
    {
        theGM.nowPlayer.tpBack.SetActive(true);
        for (int i = 0; i < theTile.tiles.Length; i++)
        {
            theTile.tiles[i].cardActive = true; //모든 카드 클릭 가능하도록 미리 클릭하고 다음턴에 해당 위치로 이동.
        }

        yield return new WaitUntil(() => theGM.tpTile != null);

        for (int i = 0; i < theTile.tiles.Length; i++)
        {
            theTile.tiles[i].cardActive = false; //다시 클릭 못하도록 변경
        }
        theGM.nowPlayer.tpBack.SetActive(false);

        // 건물과 타일의 컬러를 받아옴
        Color buildingColor = theGM.tpTile.GetComponent<Tile>().buildingImg.GetComponent<SpriteRenderer>().color;
        Color tileColor = theGM.tpTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color;

        // 건물파괴 파티클을 활성화하고 위치를 현재 타일의 건물 위치로 옮긴 다음 파티클 실행
        laserParticle.gameObject.SetActive(true);
        laserParticle.transform.position = theGM.tpTile.transform.GetChild(0).position;
        laserParticle.Play();

        // 건물의 Alpha 값을 조절해서 서서히 사라지는 듯한 연출
        while (buildingColor.a > 0f)
        {
            buildingColor.a -= 0.02f;
            tileColor.a -= 0.02f;

            theGM.tpTile.GetComponent<Tile>().buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
            theGM.tpTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;

            yield return new WaitForSeconds(0.02f);
        }

        // 파티클 비활성화
        laserParticle.gameObject.SetActive(false);

        // 현재 타일의 소유주와 건물을 없앰
        theGM.tpTile.GetComponent<Tile>().ownPlayer = -1;
        theGM.tpTile.GetComponent<Tile>().building.type = -1;

        yield return new WaitForEndOfFrame();

        // 0으로 감소시켰던 건물과 타일의 Alpha 값을 원상복구
        buildingColor.a = 1f;
        tileColor.a = 1f;
        theGM.tpTile.GetComponent<Tile>().buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
        theGM.tpTile.GetComponent<Tile>().signImg.GetComponent<SpriteRenderer>().color = tileColor;

        theGM.tpTile = null;
    }
}