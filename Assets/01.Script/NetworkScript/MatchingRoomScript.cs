using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class MatchingRoomScript : MonoBehaviour
{
    [SerializeField] Text roomName;
    // Start is called before the first frame update
    void Start()
    {
        var room = Backend.Match.GetMatchList();
        string roomInfo = room.GetReturnValue();
        roomName.text = roomInfo.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
