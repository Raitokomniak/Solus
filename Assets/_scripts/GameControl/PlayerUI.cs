using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI staminaDebugText;

    public void UpdateStaminaText(int value){
        staminaDebugText.text = "Stamina: " + value.ToString();
    }
}
