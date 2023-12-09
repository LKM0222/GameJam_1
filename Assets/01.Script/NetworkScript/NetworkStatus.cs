using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class NetworkStatus : MonoBehaviour
{
    [SerializeField] Text stateText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        stateText.text = "서버상태 : " + PhotonNetwork.NetworkClientState.ToString();
    }
}
