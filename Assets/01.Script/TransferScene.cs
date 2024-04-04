using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferScene : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.Play("Title_Sound");
    }

    public void ClickBtnSound()
    {
        AudioManager.Instance.Play("MenuClick_Sound");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
