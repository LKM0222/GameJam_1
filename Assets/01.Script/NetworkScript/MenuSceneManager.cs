using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSceneManager : MonoBehaviour
{
    [SerializeField] Text nickname;
    public bool coroFlag = false;
    public Text timerText;

    #region Instance
    private static MenuSceneManager _instance = null;
    public static MenuSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(MenuSceneManager)) as MenuSceneManager;
            }
            return _instance;
        }
    }
    #endregion

    private void Awake()
    {
        UserData user = BackendGameData.Instance.GameDataGet();
        nickname.text = user.nickName;
    }

    public IEnumerator TimerCoroutine()
    {
        int sec = 0, min = 0;
        while (true && coroFlag)
        {
            sec += 1;

            if (sec > 59)
            {
                min += 1;
                sec = 0;
            }
            timerText.text = min.ToString("00") + ":" + sec.ToString("00");

            yield return new WaitForSeconds(1f);
        }
    }
}
