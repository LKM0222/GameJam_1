using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    // 플레이어가 보유한 현금
    public int gold;

    // 플레이어가 건설한 건물
    public List<Building> buildings;

    // 플레이어가 매입한 토지, 우측 하단을 기준으로 시계 방향으로 0번부터 ID를 부여함
    public int[] groundID;

    // 플레이어가 보유하고 있는 카드
    List<Card_cherry> cards;

    Enemy theEnemy;

    // Start is called before the first frame update
    void Start()
    {
        theEnemy = FindObjectOfType<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 빌딩 건설 함수
    public void AddBuilding(Building _building)
    {
        buildings.Add(_building);
    }

    // 빌딩 파괴 함수
    public void DestroyBuilding(int _buildingID)
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            // 자신이 가지고 있는 buildingID와 매개변수로 받아온 ID가 같다면 그 건물을 파괴함
            if (buildings[i].buildingID == _buildingID)
            {
                buildings.RemoveAt(i);
                break;
            }
        }
    }

    // 카드 추가 함수
    public void AddCard(Card_cherry _card)
    {
        cards.Add(_card);
    }

    // 카드 사용 후 파괴, 카드 약탈 함수
    public void DestroyCard(int _cardID)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].cardID == _cardID)
            {
                cards.RemoveAt(i);
                break;
            }
        }
    }

    // 보유중인 카드의 갯수 체크
    public bool CardCountCheck()
    {
        // 카드가 7장 이상이라면 더 이상 카드를 습득 할 수 없으므로 false 반환
        if (cards.Count >= 7)
            return false;
        // 카드가 7장 미만이라면 카드를 1장 이상 습득 할 수 있으므로 true 반환
        else
            return true;

    }

    // 통행료 징수 함수
    public void TollCollect(int _groundID)
    {
        // 자신이 매입한 땅의 갯수만큼 반복
        for (int i = 0; i < groundID.Length; i++)
        {
            //자신이 매입한 땅의 groundID와 일치하는지 검사
            if (groundID[i] == _groundID)
            {
                // 일치한다면 땅에 빌딩도 있는지 검사
                for (int j = 0; i < buildings.Count; j++)
                {
                    // 빌딩도 있다면 (토지금액 + 빌딩금액 * 배수)만큼 통행료 징수
                    if (buildings[i].buildingID == _groundID)
                    {
                        // 상대방보유금액 -= 50 + (buildings[j].toll * buildings[j].multiple);
                        return;
                    }
                }
                // 빌딩은 없다면 토지 기본 통행료인 50골드만 징수
                // 상대방보유금액 -= 50;
            }
        }
    }

    // 플레이어 파산 체크
    public void BankruptcyCheck()
    {
        if (gold < 0)
        {
            Debug.Log("플레이어 파산. 게임 오버");
        }
    }

    // 플레이어가 보스에게 공격 당하면 피격 당하는 함수
    public void PlayerHit(int _dmg)
    {
        gold -= _dmg;
    }

    // 플레이어가 보스를 공격하는 함수
    public void PlayerAttack()
    {
        // theEnemy.currentEnemyHp -= 플레이어 공격력;
        // theEnemy.BossHit(플레이어 공격력);
    }
}
