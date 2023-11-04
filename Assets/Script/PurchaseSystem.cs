using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseSystem : MonoBehaviour
{
    [SerializeField] Tile nowTile; //현재 위치한 타일 이 타일의 빌딩을 구매할것임.
    public Text buildingText; //건물의 이름 출력
    public Image buildingImg;

    public int cur = 0;
    GameManager theGM;
    // Start is called before the first frame update
    void Start()
    {
        theGM = FindObjectOfType<GameManager>();
        buildingImg.sprite = theGM.buildings[0].buildingImg.building_front;
        buildingText.text = theGM.buildings[0].buildingName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
