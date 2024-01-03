using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
public class EventManager : MonoBehaviour
{
    private static EventManager _instance = null;

    private void Awake() {
        if(_instance == null){
            _instance = new EventManager();
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) => {
            Debug.Log("MatchRoom Create");
        };
    }
}
