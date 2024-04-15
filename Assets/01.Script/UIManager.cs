using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(UIManager)) as UIManager;
            }
            return _instance;
        }
    }
    public GameObject watingUI, turnCardUI, gameoverUI;

    [Header("GameOver UI")]
    public TMP_Text goTitle;
    public TMP_Text goMoney;
    public GameObject goImg;
    public Sprite[] winImg = new Sprite[2];
    public Sprite[] surrend = new Sprite[2];

    [Header("Login Input")]
    public InputField loginIDInput;
    public InputField loginPWInput;

    [Header("TurnImg")]
    public Text player1Text;
    public Text player2Text;

    [Header("SpecialTileText")]
    public GameObject teleportText;
    public GameObject extortionText;
    public GameObject olympicText;
    public GameObject laserText;
    public GameObject magnificationText;

    public void SetUI()
    {
        watingUI.SetActive(false);
        turnCardUI.SetActive(true);
    }

    //오류 났을때, 이 함수를 실행시켜서 나가기 버튼을 누를 수 있게 함.
    public void SetErrorUI()
    {
        watingUI.SetActive(false);
        turnCardUI.SetActive(false);
        gameoverUI.SetActive(true);
    }
}
