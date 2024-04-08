using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSceneManager : MonoBehaviour
{
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

    [SerializeField] Text nickname;
    public Text timerText;
    public bool timerFlag = false;

    private void Awake()
    {
        UserData user = BackendGameData.Instance.GameDataGet();
        nickname.text = user.nickName;
    }

    public IEnumerator TimerCoroutine()
    {
        float sec = 0, min = 0;
        while (timerFlag)
        {
            sec += 0.01f;
            if (sec > 59f) { min += 1f; sec = 0f; }
            timerText.text = min.ToString("00") + ":" + sec.ToString("00");
            yield return new WaitForSeconds(0.01f);
        }
    }
}
