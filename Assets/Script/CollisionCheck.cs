using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.name == "Dice1")
            cubeMove.instance.dice1_isJumpping = false;
        if (other.transform.name == "Dice2")
            cubeMove.instance.dice2_isJumpping = false;
    }
}