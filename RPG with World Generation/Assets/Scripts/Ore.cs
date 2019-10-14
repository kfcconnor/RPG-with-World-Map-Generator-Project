using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ore
{
    public string name;
    public int tier;
    [Range (0, 1)]
    public float rarity;
}
