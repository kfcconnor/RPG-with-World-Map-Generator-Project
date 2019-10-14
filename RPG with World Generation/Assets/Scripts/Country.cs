using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Country
{
    public string name;
    public List<Town> towns;
    public List<string> dominentRaces;
    public int factionTaxPercent;
    public int Gold;
    public int Wood;
    public int Food;
    public int Stone;
    public int Ore;

}
