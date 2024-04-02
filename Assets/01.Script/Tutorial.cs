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

    // Start is called before the first frame update
    void Start()
    {
        currentPage = 0;
        maxPage = tutorialImage.Length;
    }

    // Update is called once per frame
    void Update()
    {

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
