using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat instance;

    public int playerHp;
    public int currentPlayerHp;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentPlayerHp = playerHp;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Hit(int _dmg)
    {
        currentPlayerHp -= _dmg;
        if (currentPlayerHp - _dmg <= 0)
            Debug.Log("GAME OVER");
    }
}
