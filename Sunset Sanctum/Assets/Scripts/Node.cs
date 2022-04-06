using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public bool outlined = true;
    public bool available = false;
    public bool purchased = false;
    public bool active = false;

    public Sprite availableSprite;
    public Sprite purchasedSprite;
    public Sprite availableActiveSprite;
    public Sprite purchasedActiveSprite;

    private TextMeshProUGUI text;
    private Image image;
    private Button button;
    

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        image.sprite = availableSprite;
    }

    /// <summary>
    /// Show or hide self based on purchased status of toher nodes
    /// </summary>
    public void UpdateAppearance()
    {
        gameObject.SetActive(outlined);
        
        if (outlined == false)
        {
            text.gameObject.SetActive(false);
        }

        text.gameObject.SetActive(available);
        button.interactable = available;

        if (active)
        {
            if (purchased)
            {
                image.sprite = purchasedActiveSprite;
            }
            else
            {
                image.sprite = availableActiveSprite;
            }
        }
        else
        {
            if (purchased)
            {
                image.sprite = purchasedSprite;
            }
            else
            {
                image.sprite = availableSprite;
            }
        }

        
    }
}
