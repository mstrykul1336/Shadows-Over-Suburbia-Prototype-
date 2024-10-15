using UnityEngine;
using Photon.Pun;

public class Shopkeeper : MonoBehaviourPun
{
    public GameObject shopUIPrefab;   // Reference to the shop UI prefab
    private GameObject localShopUIInstance;
    private bool isPlayerNear = false;  // To track if the player is near the shopkeeper

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F))  // Check if player is near and presses 'F'
        {
            if (localShopUIInstance == null)
            {
                OpenShop();
            }
            else
            {
                CloseShop();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)  // Ensure it's the local player
        {
            isPlayerNear = true;  // Set the flag to true when the player is near
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)  // Ensure it's the local player
        {
            isPlayerNear = false;  // Set the flag to false when the player moves away
            CloseShop();  // Automatically close the shop UI when the player leaves the area
        }
    }

    private void OpenShop()
    {
        if (localShopUIInstance == null)  // Prevent multiple instances
        {
            // Instantiate the shop UI for the local player
            localShopUIInstance = Instantiate(shopUIPrefab, Vector3.zero, Quaternion.identity);
            localShopUIInstance.transform.SetParent(GameObject.Find("Canvas").transform, false); // Adjust according to your canvas setup
            localShopUIInstance.SetActive(true);
            var localPlayer = FindObjectOfType<FirstPersonController>();
            if (localPlayer != null)
            {
                localPlayer.DisablePlayerControls();
            }
        }
    }

    private void CloseShop()
    {
        if (localShopUIInstance != null)
        {
            Destroy(localShopUIInstance);  // Destroy the shop UI instance
            localShopUIInstance = null;
            var localPlayer = FindObjectOfType<FirstPersonController>();
            if (localPlayer != null)
            {
                localPlayer.EnablePlayerControls();
            }
        }
    }
}

