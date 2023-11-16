using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    GameManager theGM;
    [SerializeField] GameObject cardList;
    [SerializeField] GameObject cardBackPrefab;
    // Start is called before the first frame update
    void Start()
    {
        theGM = FindObjectOfType<GameManager>();
    }

    public void CardIn(){//카드 생성    
        Instantiate(cardBackPrefab,cardList.transform.position, Quaternion.identity,cardList.transform);
        print("cardINstantiate!");
    }
    public void CardOut(){//카드 삭제
        Destroy(this.transform.GetChild(this.transform.childCount - 1));
    }
}
