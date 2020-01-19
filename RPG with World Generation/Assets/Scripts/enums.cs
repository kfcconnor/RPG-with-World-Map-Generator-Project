using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enumShopType {

    General,
    //Sells Consumables and assorted equipment
    Blacksmith,
    //Sells Weapons
    Armourer,
    //Sells Armour and Accessories
    Magic
    //Sells Wands, Staffs and Spells
}

public enum enumCharType
{
    Caster,
    //Uses magic, has spell section of menu
    Melee,
    //Uses Melee attacks and equipment, makes use of skill section of menu
    Ranged
    //Uses Ranged Attacks and equipment, makes use of skill section of menu
}

public enum enumEquipmentTypes
{

    None,
    //Placeholder type to avoid errors
    RightHand,
    //Usable only in main hand, Most One-Handed Weapons will fall here
    LeftHand,
    //Usable only in off hand, Most Shields and Off-Hand Equipment will fall here.
    EitherHand,
    //Items that can be duel wielded
    TwoHands,
    //Requires Both hands, Great Weapons will fall here
    Head,
    //Head Armour will fall here
    Body,
    //Body Armour will fall here
    Usable
    //Potions and other consumables will fall here
}