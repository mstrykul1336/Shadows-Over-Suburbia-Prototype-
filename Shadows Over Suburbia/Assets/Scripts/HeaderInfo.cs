using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HeaderInfo : MonoBehaviourPun
{
    public TextMeshProUGUI nameText;
    //public TextMeshProUGUI characterText;
    public Image bar;
    private float maxValue;

    public void Initialize(string text, int maxVal)
    {
        nameText.text = text;
        //characterText.text = PlayerController.character_name;
        maxValue = maxVal;
        bar.fillAmount = 1.0f;
    }
    
    [PunRPC]
    void UpdateHealthBar(int value)
    {
        bar.fillAmount = (float)value / maxValue;
    }
}