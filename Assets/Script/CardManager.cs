using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] Card cardInfo;
    [SerializeField] float upPos;
    GameManager theGM;
    // Start is called before the first frame update
    void Start()
    {
        theGM = FindObjectOfType<GameManager>();   
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnMouseEnter() {
        if(!theGM.UIFlag){
            this.transform.localScale = new Vector3(14f,14f,1f);
            this.transform.position += Vector3.up * upPos; 
            this.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        
    }
    private void OnMouseExit() {
        if(!theGM.UIFlag){
            this.transform.localScale = new Vector3(7f,7f,1f);
            this.transform.position += Vector3.down * upPos;
            this.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
    }
    private void OnMouseDown()
    {   
        if(!theGM.UIFlag){
            //클릭하면 효과 발동
            print("효과 발동");
        }
        
    }
}
