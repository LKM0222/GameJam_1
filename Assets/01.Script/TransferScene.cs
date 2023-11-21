using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.Play("titleBGM");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadingScene()
    {
        AudioManager.instance.Play("buttonSound");
        AudioManager.instance.Stop("titleBGM");
        SceneManager.LoadScene("LoadingScene");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ButtonClickSound()
    {
        
    }
}
