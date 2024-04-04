using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] Sprite[] tutorialImage;
    public GameObject tutorialObject;

    int currentPage;
    int maxPage;

    void Start()
    {
        currentPage = 0;
        maxPage = tutorialImage.Length;
    }

    public void TurnLeftPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            tutorialObject.GetComponent<Image>().sprite = tutorialImage[currentPage];
            AudioManager.Instance.Play("MenuClick_Sound");
        }
    }

    public void TurnRightPage()
    {
        if (currentPage < maxPage - 1)
        {
            currentPage++;
            tutorialObject.GetComponent<Image>().sprite = tutorialImage[currentPage];
            AudioManager.Instance.Play("MenuClick_Sound");
        }
    }
}
