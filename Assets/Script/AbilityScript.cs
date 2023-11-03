using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityScript : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] GameObject middleCheck;
    [SerializeField] GameObject parentObj;
    [SerializeField] bool midFlag = false; //중앙 그림에 들어갔는지 체크
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
    public void OnBeginDrag(PointerEventData eventData){

    }
    public void OnDrag(PointerEventData eventData){
        if(!midFlag){
            Vector3 mousePos = new Vector3(Input.mousePosition.x, 
            Input.mousePosition.y, this.transform.position.z);//마우스 포지션을 받아서
            Vector3 objPos = Camera.main.ScreenToWorldPoint(mousePos); //카메라 좌표로 변경 후에
            transform.position = new Vector3(objPos.x, objPos.y, this.transform.position.z); //위치 변환
        }
        
    }
    public void OnEndDrag(PointerEventData eventData){
        if(middleCheck == null){ //아무것도 없다면 그냥 원래 자리로
            this.transform.localPosition = Vector3.zero;
            middleCheck = null;
        }
        else{//하나가 들어갔다면 middle에 고정
            if(middleCheck.transform.childCount > 0){ //들어가려는 자리에 child가 있다면
                AbilityScript sChild = middleCheck.transform.GetChild(0).GetComponent<AbilityScript>();
                //이미 위치하고 있던 슬롯을 원래의 parent로 이동
                sChild.transform.SetParent(sChild.parentObj.transform);
                //이후 이전에 있던 슬롯은 원래의 자리로 이동.
                sChild.transform.localPosition = new Vector3(0f,0f,0f);
                //슬롯을 이동한 자리로 위치시키는 코드
                this.transform.SetParent(middleCheck.transform);
                this.transform.localPosition = Vector3.zero;
                midFlag = true; //다시 못움직이게 클릭 방지
                
                //원래 있던 슬롯의 클릭 다시 활성화
                sChild.midFlag = false;
            }
            else{ //아무것도 없다면 그냥 움직이면됨
                //슬롯을 이동한 자리로 위치시키는 코드
                this.transform.SetParent(middleCheck.transform);
                this.transform.localPosition = Vector3.zero;
                midFlag = true; //다시 못움직이게 클릭 방지
            }
            
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Middle")){
            middleCheck = other.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Middle")){
            middleCheck = null;
        }
    }
}
