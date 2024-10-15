using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public Button buyButton;
    public Button buyButtonShield;
    public Button buyButtonKnife;
    public TextMeshProUGUI soldOutTextPotion;
    public int itemCostPotion = 1;
    public int maxPurchasesPotion = 2;
    private int purchaseCountPotion = 0;

    public TextMeshProUGUI soldOutTextShield;
    public int itemCostShield = 2;
    public int maxPurchasesShield = 1;
    private int purchaseCountShield= 0;
    private PlayerController localPlayer;

    public TextMeshProUGUI soldOutTextKnife;
    public int itemCostKnife = 2;
    public int maxPurchasesKnife = 1;
    private int purchaseCountKnife= 0;
  
    // Start is called before the first frame update
    void Start()
    {
        localPlayer = FindObjectOfType<PlayerController>();

        
        // Add listener for button click
        //buyButton.onClick.AddListener(BuyItem);
    }

    public void BuyPotion()
    {
        if (localPlayer.gold >= itemCostPotion && purchaseCountPotion < maxPurchasesPotion)
        {
            localPlayer.gold -= itemCostPotion; 
            localPlayer.UpdateGold(); 
            purchaseCountPotion++;               // Increase purchase count
            localPlayer.AddItemToInventory("Potion"); // Add item to inventory

            if (purchaseCountPotion >= maxPurchasesPotion)
            {
                soldOutTextPotion.gameObject.SetActive(true);
                buyButton.interactable = false;
            }
        }
    }

    public void BuyShield()
    {
        if (localPlayer.gold >= itemCostShield && purchaseCountShield < maxPurchasesShield)
        {
            localPlayer.gold -= itemCostShield; 
            localPlayer.UpdateGold(); 
            purchaseCountShield++;               // Increase purchase count
            localPlayer.AddItemToInventory("Shield"); // Add item to inventory

            if (purchaseCountShield >= maxPurchasesShield)
            {
                soldOutTextShield.gameObject.SetActive(true);
                buyButtonShield.interactable = false;
            }
        }
    }


    public void BuyKnife()
    {
        if (localPlayer.gold >= itemCostKnife && purchaseCountKnife < maxPurchasesKnife)
        {
            localPlayer.gold -= itemCostKnife; 
            localPlayer.UpdateGold(); 
            purchaseCountKnife++;               // Increase purchase count
            localPlayer.AddItemToInventory("Knife"); // Add item to inventory

            if (purchaseCountKnife >= maxPurchasesKnife)
            {
                soldOutTextKnife.gameObject.SetActive(true);
                buyButtonKnife.interactable = false;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
