using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_cherry : MonoBehaviour
{
    // 카드의 이름
    public string cardName;

    // 카드의 아이디
    public int cardID;

    // 거대날개 101, 부당거래 102, 공중부양 103, 투명도둑 104, 거대화꼬꼬 105, 투시 106, 레이저빔 107
    const int GIGAWING = 101, UNFAIR_TRADE = 102, LEVITATION = 103,
              INVISIBLE_THIEF = 104, GIGAKKOKKO = 105, PENETRATE = 106, LASER = 107;

    // 카드 사용 함수
    public void UseCard(Card_cherry _card)
    {
        switch (_card.cardID)
        {
            case GIGAWING:
                GigaWing();
                break;
            case UNFAIR_TRADE:
                UnfairTrade();
                break;
            case LEVITATION:
                Levitation();
                break;
            case INVISIBLE_THIEF:
                InvisibleThief();
                break;
            case GIGAKKOKKO:
                GigaKkokko();
                break;
            case PENETRATE:
                Penetrate();
                break;
            case LASER:
                Laser();
                break;
        }
    }

    public void GigaWing()
    {
        // 플레이어 이동 칸 수 *= 2;
    }

    public void UnfairTrade()
    {
        // 플레이어가 도착한 땅의 상대방 건물 매입
    }

    public void Levitation()
    {
        // 주사위 턴을 소모하고 플레이어 이동 칸수 1~3으로 확정
    }

    public void InvisibleThief()
    {
        // 상대방의 위치를 추월할 경우 상대방의 무작위 카드 강탈
        // AddCard(상대방.DestroyCard())
    }

    public void GigaKkokko()
    {
        // 상대방의 땅에 도착했을 때 건물이 있으면 파괴하고 없으면 통행료를 면제
    }

    public void Penetrate()
    {
        // 상대방의 모든 카드를 확인하고 카드팩에서 한 장을 가져옴
    }

    public void Laser()
    {
        // 상대의 건물을 특정해서 하나 파괴함
    }
}
