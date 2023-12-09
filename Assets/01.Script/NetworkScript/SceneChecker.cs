using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// using UnityEngine.UI;

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
            // text.GetComponent<Text>().text = "참여코드 : " + PhotonNetwork.CurrentRoom.Name;
        }
    }
}
