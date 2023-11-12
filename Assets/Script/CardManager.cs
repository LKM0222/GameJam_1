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

    // Start is called before the first frame update
    void Start()
    {
        theGM = FindObjectOfType<GameManager>();
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
            theGM.nowPlayer.cards.Remove(this.gameObject.GetComponent<Card>());
            print("효과 발동");
        }

    }
}
