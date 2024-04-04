using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private static TileManager _instance;
    public static TileManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(TileManager)) as TileManager;
            }
            return _instance;
        }
    }

    [SerializeField]
    public Tile[] tiles;
}
