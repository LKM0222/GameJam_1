using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 보스의 HP
    public int enemyHp;

    // 현재 보스의 HP
    public int currentEnemyHp;

    // 보스가 공격하기까지의 대기 턴 수
    public int attackCount;

    // 보스가 사용하는 능력
    enum Skill
    {
        PUNCH,
        KICK,
        LASER
    }

    PlayerStat thePlayerStat;

    // Start is called before the first frame update
    void Start()
    {
        currentEnemyHp = enemyHp;
        thePlayerStat = FindObjectOfType<PlayerStat>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 일정 턴이 돌아올때마다 보스가 공격하는 함수
    // 정확한 수치는 수정 예정이라서 차후 작성
    public void BossAttack()
    {
        if (120 >= currentEnemyHp && 81 <= currentEnemyHp)
        {
            // 1단계 공격
            // thePlayerStat.PlayerHit(보스 공격력);
        }
        else if (80 >= currentEnemyHp && 41 <= currentEnemyHp)
        {
            // 2단계 공격
            // thePlayerStat.PlayerHit(보스 공격력);
        }
        else if (40 >= currentEnemyHp && 1 <= currentEnemyHp)
        {
            // 3단계 공격
            // thePlayerStat.PlayerHit(보스 공격력);
        }
    }

    // 보스가 플레이어에게 공격 당했을 때 데미지를 입히는 함수
    public void BossHit(int _dmg)
    {
        currentEnemyHp -= _dmg;
    }

    // 보스 사망 체크, 사망했으면 true 반환
    public bool BossDieCheck()
    {
        if (currentEnemyHp <= 0)
        {
            Debug.Log("보스 사망");
            return true;
        }
        else
            return false;
    }
}