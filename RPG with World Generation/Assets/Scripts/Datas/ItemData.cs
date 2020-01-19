using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public string PicturesName = string.Empty;
    public string Name = string.Empty;
    public string Description = string.Empty;


    public int HealthPoint = default(int);

    public int Mana = default(int);

    public int Attack = default(int);

    public int Defense = default(int);

    public int Magic = default(int);

    public int MagicDefense = default(int);

    public int Price = default(int);

    //public EnumCharacterType AllowedCharacterType = EnumCharacterType.None;

    public enumEquipmentTypes EquipementType = enumEquipmentTypes.None;

    public bool IsEquiped = false;
}
