using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 상단에 플레이어의 행동 순서 UI 관리 스크립트
public class TurnSignScript : MonoBehaviour
{
    // 테두리 오브젝트
    [SerializeField] GameObject cursor;

    // 행동 순서
    public int cursorPos;

    void Update()
    {
        // 카드를 사용할 때
        if (cursorPos == 1)
        {
            cursor.transform.localPosition = new Vector3(-147f, cursor.transform.position.y, 0f);
        }
        // 주사위를 굴릴 때
        else if (cursorPos == 2)
        {
            cursor.transform.localPosition = new Vector3(0f, cursor.transform.position.y, 0f);
        }
        // 이동할 때
        else if (cursorPos == 3)
        {
            cursor.transform.localPosition = new Vector3(147f, cursor.transform.position.y, 0f);
        }
    }
}
