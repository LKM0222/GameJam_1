#region using
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#endregion

public class InGameScript : MonoBehaviour
{
    #region Instance
    private static InGameScript _instance = null;

    public static InGameScript Instance {
        get{
            if(_instance == null){
                _instance = FindObjectOfType(typeof(InGameScript)) as InGameScript;
            }
            return _instance;
        }
    }
    #endregion

    #region test
    [SerializeField] Text testText;
    public string testStr = "";
    #endregion



    // Update is called once per frame
    void Update()
    {
        testText.text = testStr;
    }
}
