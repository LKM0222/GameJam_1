using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackendManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        var bro = Backend.Initialize(true);
        if(bro.IsSuccess()){
            print("서버 연결 성공" + bro);
        } else { 
            print("서버 연결 실패" + bro);
        }
    }
}
