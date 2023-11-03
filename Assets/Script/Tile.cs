using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile: MonoBehaviour
{
    public Player[] players;
    public Building building;
    public GroundType type;
    public int price;
    public GameObject tilePrefab;


}
