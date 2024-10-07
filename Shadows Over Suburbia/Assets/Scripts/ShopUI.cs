using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public Button buyButton;
    public TextMeshProUGUI soldOutText;
    public int itemCost = 1;
    public int maxPurchases = 2;
    private int purchaseCount = 0;
    private PlayerController localPlayer;
    // Start is called before the first frame update
    void Start()
    {
        localPlayer = FindObjectOfType<PlayerController>();

        
        // Add listener for button click
        buyButton.onClick.AddListener(BuyItem);
    }

    void BuyItem()
    {
        if (localPlayer.gold >= itemCost && purchaseCount < maxPurchases)
        {
            localPlayer.gold -= itemCost; 
            localPlayer.UpdateGold(); 
            purchaseCount++;               // Increase purchase count
            localPlayer.AddItemToInventory("Potion"); // Add item to inventory

            if (purchaseCount >= maxPurchases)
            {
                soldOutText.gameObject.SetActive(true);
                buyButton.interactable = false;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
