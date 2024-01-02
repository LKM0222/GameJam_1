using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using Unity.VisualScripting;

public class MatchingRoomScript : MonoBehaviour
{
    [SerializeField] Text roomName;
    private static MatchingRoomScript _instance;
    public static MatchingRoomScript Instance{
        get {
            if(_instance == null){
                _instance = new MatchingRoomScript();
            }
            return _instance;
        }
    }
   
    public MatchCard roomMatchCard;
    // Start is called before the first frame update
    void Start()
    {
        
        
        // string roomInfo = room.GetReturnValue();
        // roomName.text = roomInfo.ToString();
        // print("MatchCard" + MatchManager.Instance.matchCards[0]); 
        StartCoroutine(WaitInfoCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator WaitInfoCoroutine(){
        print("waitcoroutine start!");
        yield return new WaitUntil(() => MatchManager.Instance.listIsEmpty == true);
        print("listisEmpty is True" + MatchManager.Instance.listIsEmpty);
        roomMatchCard = MatchManager.Instance.matchCards[0];
        print("matchCard is " + roomMatchCard + "and MatchCards Count is " + MatchManager.Instance.matchCards.Count);
        roomName.text = roomMatchCard.matchTitle;
        
    }
}
