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

    private void Awake() {
        if(_instance == null){
            _instance = FindObjectOfType(typeof(BackendManager)) as BackendManager;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }
    
    void Start()
    {
        var bro = Backend.Initialize(true);
        //성공 실패는 bro.isSuccess
    }

    private void Update() {
        if(Backend.IsInitialized){
            Backend.Match.Poll(); //데이터 전송용 poll
        }
    }
}
