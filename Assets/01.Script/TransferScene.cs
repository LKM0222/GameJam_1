using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.Play("Title_Sound");
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ClickBtnSound()
    {
        AudioManager.Instance.Play("MenuClick_Sound");
    }

    public void LoadingScene()
    {
        AudioManager.Instance.Play("MenuClick_Sound");
        AudioManager.Instance.Stop("Title_Sound");

        // SceneManager.LoadScene("LoadingScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
