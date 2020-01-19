using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{

    public string Name = string.Empty;
    public bool MainCharacter = false;
    public enumCharType charType;
    public int HP = 0;
    public int MP = 0;
    public int Attack = 0;
    public int Defence = 0;
    public int Magic = 0;
    public int MagicDefence = 0;
    public int Level = 0;
    public int XP = 0;
    public int MaxHP;
    public int MaxMP;

    public ItemData LeftHand;
    public ItemData RightHand;
    public ItemData Head;
    public ItemData Body;

    public List<SpellData> SpellsList = new List<SpellData>();


}
