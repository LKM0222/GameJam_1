using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class SceneChecker : MonoBehaviour
{
    SceneChecker instance;
    private void Awake()
    {
        if(null == instance){
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "MenuScene"){
            GameObject text = GameObject.Find("Code");
            text.GetComponent<Text>().text = "참여코드 : " + PhotonNetwork.CurrentRoom.Name;

            GameObject playerText = GameObject.Find("PlayerText");
            string playertxt = "";
            for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++){
                playertxt += PhotonNetwork.PlayerList[i].NickName + ", ";
            }
            playerText.GetComponent<Text>().text = playertxt;
        }
    }
}
