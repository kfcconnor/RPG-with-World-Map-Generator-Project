using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    //{General, Blacksmith, Armourer, Magic}
    public string shopName;
    public enumShopType shopType;
    public Town townLocation;
    [Range(0, 2)]
    public float costMod;

    public Shop(Town location, int sType)
    {
        if (sType == 0)
        {
            shopType = enumShopType.General;
        }
        else if (sType == 1)
        {
            shopType = enumShopType.Blacksmith;
        }
        else if (sType == 2)
        {
            shopType = enumShopType.Armourer;

        }
        else if (sType == 3)
        {
            shopType = enumShopType.Magic;
        }

        if (location.stability < 50)
        {
            costMod = (location.stability / 100) * 2;
        }
        else if (location.stability == 50)
        {
            costMod = 1;
        }
        else if (location.stability > 50)
        {
            costMod = 1 + (((location.stability - 50) / 100) * 2);
        }
    }

    public void updateCostMod()
    {
        if (townLocation.stability < 50)
        {
            costMod = (townLocation.stability / 100) * 2;
        }
        else if (townLocation.stability == 50)
        {
            costMod = 1;
        }
        else if (townLocation.stability > 50)
        {
            costMod = 1 + (((townLocation.stability - 50) / 100) * 2);
        }
    }

    public void updateInventory()
    {
        if (shopType == enumShopType.General)
        {

        }
        else if (shopType == enumShopType.Armourer)
        {

        }
        else if (shopType == enumShopType.Blacksmith)
        {

        }
        else if (shopType == enumShopType.Magic)
        {

        }
    }

}