using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextManager : MonoBehaviour
{
    TMP_Text text;

    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public void ShowText(string _text)
    {
        text.text = _text;
        text.gameObject.SetActive(true);
    }

    public void HideText()
    {
        text.text = "";
    }

}
