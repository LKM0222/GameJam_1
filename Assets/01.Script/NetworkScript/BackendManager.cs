using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.UI;

public class BackendManager : MonoBehaviour
{
    #region Instance
    private static BackendManager instance;
    public static BackendManager Instance
    {
        get
        {
            if (instance == null)
                instance = new();

            return instance;
        }
    }
    #endregion

    [SerializeField] Text hashText;
    private BackendManager _instance = null;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType(typeof(BackendManager)) as BackendManager;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        var bro = Backend.Initialize(true);
        //성공 실패는 bro.isSuccess

        // // Android Hash Key Test
        // Backend.Utils.GetGoogleHash();
        // string googlehash = Backend.Utils.GetGoogleHash();
        // hashText.text = "hash : " + googlehash +" :: is hash";
    }

    private void FixedUpdate()
    {
        if (Backend.IsInitialized)
        {
            //데이터 전송용 poll
            Backend.Match.Poll();
        }
    }
}
