using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{   
    public PurchaseSystem thePS;
    GameManager theGM;
    private void Start()
    {
        theGM = FindObjectOfType<GameManager>();
    }
    public void OnRightBtn(){//Right버튼
        thePS.cur += 1;
        if(thePS.cur > theGM.buildings.Length - 1){
            thePS.cur = 0;
        }
        thePS.buildingImg.sprite = theGM.buildings[thePS.cur].buildingImg.building_front;
        thePS.buildingText.text = theGM.buildings[thePS.cur].buildingName;
    }
    public void OnLeftBtn(){//Left버튼
        thePS.cur -= 1;
        if(thePS.cur == -1){
            thePS.cur = theGM.buildings.Length - 1;
        }
        thePS.buildingImg.sprite = theGM.buildings[thePS.cur].buildingImg.building_front;
        thePS.buildingText.text = theGM.buildings[thePS.cur].buildingName;
    }
    public void OnPurchaseBtn(){//구매버튼

    }
    public void OnPurchaseCloseBtn(){
        theGM.UIFlag = false;
    }
}
