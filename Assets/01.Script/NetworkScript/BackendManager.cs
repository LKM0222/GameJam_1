using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;

public class BackendManager : MonoBehaviour
{
    #region Instance
    private static BackendManager instance;
    public static BackendManager Instance{
        get{
            if(instance == null)
                instance = new();

            return instance;
        }
    }
    #endregion

    private BackendManager _instance = null;



    //SessionID
    public SessionId mySessionId;

    //SignUpUI

    private void Awake() {
        if(_instance == null){
            // _instance = new BackendManager();
            _instance = FindObjectOfType(typeof(BackendManager)) as BackendManager;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        var bro = Backend.Initialize(true);

        if(bro.IsSuccess()){
            print("서버 접속 성공 : " + bro);
        } else {
            print("서버 접속 실패 : " + bro);
        }
    }
    private void Update() {
        if(Backend.IsInitialized){
            Backend.Match.Poll();
        }
    }

}
