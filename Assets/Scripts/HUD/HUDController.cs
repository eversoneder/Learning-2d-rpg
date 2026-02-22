using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class HUDController : MonoBehaviour
{
    [Header("Collectables")]
    [SerializeField] private Image waterUIBar;
    [SerializeField] private Image woodUIBar;
    [SerializeField] private Image carrotUIBar;
    [SerializeField] private Image fishUIBar;

    [Header("Tools")]
    //[SerializeField] private Image axeUI;
    //[SerializeField] private Image shovelUI;
    //[SerializeField] private Image waterUI;
    //[SerializeField] private Image seedUI;
    [SerializeField] private List<Image> toolsUI = new List<Image>();
    [SerializeField] private Color selectColor;
    [SerializeField] private Color whiteHalfAlpha;

    [Header("Player")]
    [SerializeField] private Player player;

    //[Header("Settings")]
    

    private void Start()
    {
        waterUIBar.fillAmount = 0f;
        woodUIBar.fillAmount = 0f;
        carrotUIBar.fillAmount = 0f;
        fishUIBar.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        waterUIBar.fillAmount = PlayerItems.Instance.CurrentWater / PlayerItems.Instance.WaterMaxLimit;
        woodUIBar.fillAmount = PlayerItems.Instance.CurrentWood / PlayerItems.Instance.WoodMaxLimit;
        carrotUIBar.fillAmount = PlayerItems.Instance.CurrentCarrot / PlayerItems.Instance.CarrotMaxLimit;
        fishUIBar.fillAmount = PlayerItems.Instance.CurrentFish / PlayerItems.Instance.FishMaxLimit;

        toolsUI[player.CurrentHandlingObj].color = selectColor;

        for (int i = 0; i < toolsUI.Count; i++)
        {
            if (i == player.CurrentHandlingObj)
            {
                toolsUI[i].color = selectColor;
            }
            else
            {
                toolsUI[i].color = whiteHalfAlpha;
            }
        } 
    }
}
