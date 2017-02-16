using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Drop  {

    public string itemName;
    [Range(0.0f,1.0f)]
    public float dropChance;

    public bool DropChanceSucess()
    {
        return Random.value <= dropChance;
    }
}
