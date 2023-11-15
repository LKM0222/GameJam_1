using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    GameManager theGM;

    // 카드의 정보
    public Card cardInfo;

    // 카드를 위아래로 움직일 수치
    [SerializeField] float upPos;

    PlayerManager thePM;
    public bool completeFlag;
    public ParticleSystem destroyParticle;

    // Start is called before the first frame update
    void Start()
    {
        theGM = FindObjectOfType<GameManager>();
        thePM = FindObjectOfType<PlayerManager>();
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

            // 카드 오브젝트 삭제 및 플레이어가 가지고 있는 카드 리스트에서도 삭제
            Destroy(this.gameObject);
            Destroy(theGM.nowPlayer.cardParent.GetChild(0).gameObject);
            theGM.nowPlayer.cards.Remove(this.cardInfo);
            print("효과 발동");
        }
    }

    public void HighSpeedMove()
    {
        print("고속이동 사용");
        theGM.nowPlayer.diceNum = theGM.nowPlayer.diceNum * 2;
        theGM.nowPlayer.highSpeedFlag = false;
    }

    public void InvisibleThief()
    {
        print("투명도둑 사용");
        // 상대가 가진 카드를 랜덤으로 골라서 현재 플레이어 카드에 추가하고 상대 플레이어 카드에는 삭제
        int randomCard = UnityEngine.Random.Range(0, thePM.againstPlayer.cards.Count);
        theGM.nowPlayer.cards.Add(thePM.againstPlayer.cards[randomCard]);
        thePM.againstPlayer.cards.RemoveAt(randomCard);

        // 상대방 카드 UI를 내 카드 UI로 옮겨오고 스프라이트 이미지도 변경
        GameObject dCard = thePM.againstPlayer.cardParent.GetChild(randomCard).gameObject;
        dCard.transform.parent = thePM.cardParent;
        dCard.GetComponent<SpriteRenderer>().sprite = theGM.nowPlayer.cardPrefab.GetComponent<SpriteRenderer>().sprite;

        // invisibleFlag를 False로 변환
        theGM.nowPlayer.invisibleFlag = false;
    }

    public void BiggerChicken()
    {
        print("거대화꼬꼬 사용");
        if (thePM.nowTile.ownPlayer != theGM.nowPlayer.playerId)
        {
            StartCoroutine(BiggerCoroutine());
        }
    }

    public void Penetrate()
    {
        print("투시 사용");
    }

    IEnumerator BiggerCoroutine()
    {
        Color buildingColor = thePM.nowTile.buildingImg.GetComponent<SpriteRenderer>().color;
        Color tileColor = thePM.nowTile.signImg.GetComponent<SpriteRenderer>().color;
        destroyParticle.gameObject.SetActive(true);
        destroyParticle.transform.position = thePM.nowTile.transform.GetChild(0).position;
        destroyParticle.Play();

        while (buildingColor.a > 0f)
        {
            buildingColor.a -= 0.02f;
            tileColor.a -= 0.02f;

            thePM.nowTile.buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
            thePM.nowTile.signImg.GetComponent<SpriteRenderer>().color = tileColor;

            yield return new WaitForSeconds(0.01f);
        }

        destroyParticle.gameObject.SetActive(false);

        thePM.nowTile.ownPlayer = -1;
        thePM.nowTile.building.type = -1;

        buildingColor.a = 1f;
        tileColor.a = 1f;
        thePM.nowTile.buildingImg.GetComponent<SpriteRenderer>().color = buildingColor;
        thePM.nowTile.signImg.GetComponent<SpriteRenderer>().color = tileColor;

        this.gameObject.transform.localScale = new Vector3(1f, 1f, 0);
        thePM.biggerFlag = false;
        completeFlag = true;
    }
}
