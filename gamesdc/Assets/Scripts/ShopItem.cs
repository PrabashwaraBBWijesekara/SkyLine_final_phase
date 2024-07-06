using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

[CreateAssetMenu (fileName ="ShopMenu",menuName = "Scriptabl eObject/New shop Item")]
public class ShopItem : ScriptableObject
{
    public Sprite itemImg;
    public string title;
    public int price;
    public string currencyTxt;
    public Sprite currencyImg;
}

