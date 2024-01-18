using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTile : MonoBehaviour
{
    GameManager theGM;
    CardManager theCM;
    public bool isGetCard;

    // Start is called before the first frame update
    void Start()
    {
        theGM = FindObjectOfType<GameManager>();
        theCM = FindObjectOfType<CardManager>();
    }

    public void PoultryParm()
    {
        theGM.nowPlayer.playerMoney += 200;
    }

    public IEnumerator CardProvideCoroutine()
    {
        isGetCard = false;
        if (theGM.nowPlayer.cards.Count < 8)
        {
            // 랜덤하게 카드번호를 추출
            Card newCard = theGM.cards[UnityEngine.Random.Range(0, theGM.cards.Length)];

            // 팻말 아래 카드리스트에 복제하고 플레이어의 카드 목록에 추가함
            var _card = Instantiate(theGM.nowPlayer.cardPrefab, Vector3.zero, Quaternion.identity, theGM.nowPlayer.cardParent);
            _card.transform.localPosition = new Vector3(0f, 0f, 0f);
            theGM.nowPlayer.cards.Add(newCard);

            StartCoroutine(theCM.ShowGetCard());
            yield return new WaitUntil(() => theCM.isShowCard);
            theCM.isShowCard = false;

            // 만약 통행료면제 카드라면 카드효과를 즉시 활성화.
            if (newCard == theGM.cards[6])
            {
                theGM.nowPlayer.exemptionFlag = true;
                theGM.textManager.ShowText("플레이어" + theGM.nowPlayer.playerId + " 통행료 면제 효과 발동");
                yield return new WaitForSeconds(3f);
                theGM.textManager.HideText();
            }
        }
        isGetCard = true;
    }

    public void Teleport()
    {

    }

    public void TexCollect()
    {

    }

    public void Roulette()
    {

    }
}
