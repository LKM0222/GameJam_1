using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public Image fill;
    public float speed;
    public GameObject character;
    public GameObject flying_character;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartLoading());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator StartLoading()
    {
        fill.fillAmount = 0f;
        while (fill.fillAmount.ToString() != 1f.ToString())
        {
            float amount = Random.Range(0.01f, 0.03f);
            fill.fillAmount += amount;
            character.transform.Translate(amount * 16, 0, 0);
            yield return new WaitForSeconds(0.1f);
        }
        StopAllCoroutines();
        SceneManager.LoadScene("TestScene");
    }
}
