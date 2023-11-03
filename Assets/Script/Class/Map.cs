using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class Map
{
    [SerializeField] int mapcode;
    [SerializeField] int maptype;
    [SerializeField] PolygonCollider2D mapbound;
}
