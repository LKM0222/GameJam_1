using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackendManager : MonoBehaviour
{
    private BackendManager _instance = null;
    private void Awake() {
        if(_instance == null){
            _instance = new BackendManager();
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
