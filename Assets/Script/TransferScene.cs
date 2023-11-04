using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEditor.Rendering;
using UnityEngine.UI;

public class TransferScene : MonoBehaviour
{
    public float fadeTime = 3f;
    public float currentFadeTime;

    public SpriteRenderer spriteRender;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void Exit()
    {
        Application.Quit();
    }

    IEnumerator FadeOutCoroutine()
    {
        while (currentFadeTime > 0)
        {
            currentFadeTime -= Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
