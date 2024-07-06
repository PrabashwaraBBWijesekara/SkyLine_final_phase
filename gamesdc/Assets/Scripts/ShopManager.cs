using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System;

public class ShopManager : MonoBehaviour
{

    //[SerializeField] private Text coinsCount;
    //[SerializeField] private Text gemsCount;
    public int coin;
    public int gem;
    public ShopItem[] ShopItem;
    public ShopTemplate[] ShopPanels;
    public Button[] purchaseBtn;



    // Start is called before the first frame update
    void Start()
    {
        loadPanels();

    }

    // Update is called once per frame
    void Update()
    {
        coin = int.Parse(ItemCollector.coins.ToString());
        gem = int.Parse(ItemCollector.gems.ToString());
        CheckPurchase();
    }

    public void CheckPurchase()
    {
        for (int i = 0; i < ShopItem.Length; i++)
        {
            if (ShopItem[i].currencyTxt == "coin")
            {
                if (coin >= ShopItem[i].price)
                    purchaseBtn[i].interactable = true;
                else
                    purchaseBtn[i].interactable = false;
            }

            else
            {
                if (gem >= ShopItem[i].price)
                    purchaseBtn[i].interactable = true;
                else
                    purchaseBtn[i].interactable = false;
            }


        }
    }
    public void loadPanels()
    {
        for (int i = 0; i < ShopItem.Length; i++)
        {
            ShopPanels[i].itemImg.sprite = ShopItem[i].itemImg;
            ShopPanels[i].titleTxt.text = ShopItem[i].name;
            ShopPanels[i].priceTxt.text = ShopItem[i].price.ToString();
            ShopPanels[i].currencyImg.sprite = ShopItem[i].currencyImg;
        }

    }

    public void purchaseItem(int btnNo)
    {
        Debug.Log("clicked  " + btnNo);
        if (ShopItem[btnNo].currencyTxt == "coin")
        {
            if (coin >= ShopItem[btnNo].price)
            {
                coin = coin - ShopItem[btnNo].price;
                
                ItemCollector.coins = coin;
                CheckPurchase();
            }
        }
        else
        {
            if (gem >= ShopItem[btnNo].price)
            {
                gem = gem - ShopItem[btnNo].price;
                ItemCollector.gems= gem;
                CheckPurchase();
            }
        }
    }


}
