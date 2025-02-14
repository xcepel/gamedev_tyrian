using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesShopView : AView
{
    [SerializeField] private Button shopDurabilityButton;
    [SerializeField] private Button shopMovementSpeedButton;
    [SerializeField] private Button shopFiringRateButton;
    
    [SerializeField] private TextMeshProUGUI creditsText;
    
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button backToMenuButton;

    public override void Initialize()
    {
        UpdateShopInfo();
        
        if (shopDurabilityButton) shopDurabilityButton.onClick.AddListener(OnShopDurabilityButtonClicked);
        if (shopMovementSpeedButton) shopMovementSpeedButton.onClick.AddListener(OnShopMovementSpeedButtonClicked);
        if (shopFiringRateButton) shopFiringRateButton.onClick.AddListener(OnShopFiringRateButtonClicked);
        
        if (nextLevelButton) nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
        if (backToMenuButton) backToMenuButton.onClick.AddListener(OnBackButtonClicked);
    }

    public override void Deinitialize()
    {
        if (shopDurabilityButton) shopDurabilityButton.onClick.RemoveAllListeners();
        if (shopMovementSpeedButton) shopMovementSpeedButton.onClick.RemoveAllListeners();
        if (shopFiringRateButton) shopFiringRateButton.onClick.RemoveAllListeners();
        
        if (nextLevelButton) nextLevelButton.onClick.RemoveAllListeners();
        if (backToMenuButton) backToMenuButton.onClick.RemoveAllListeners();
    }

    public override void DoShow(object args)
    {
        gameObject.SetActive(true);
        UpdateShopInfo();
    }
    
    private void OnShopDurabilityButtonClicked()
    {
        ShopManager.Instance.BuyDurability(300);
        UpdateShopInfo();
    }
    
    private void OnShopMovementSpeedButtonClicked()
    {
        ShopManager.Instance.BuyMovementSpeed(100);
        UpdateShopInfo();
    }
    
    private void OnShopFiringRateButtonClicked()
    {
        ShopManager.Instance.BuyFiringRate(200);
        UpdateShopInfo();
    }
    
    private void OnNextLevelButtonClicked()
    {
        UIManager.Instance.HideAllViews();
        int nextLevel = GameManager.GetCurrentLevel() + 1;
        GameManager.ChangeScene("Level_" + nextLevel.ToString());
    }
    
    private void OnBackButtonClicked()
    {
        Currencies.Instance.ResetCurrencies();
        CheatsManager.Instance.Destroy();
        ShopManager.Instance.Destroy();
        
        UIManager.Show<MainMenuView>();
    }
    
    private void UpdateShopInfo()
    {
        if (ShopManager.Instance)
        {
            creditsText.text = "Credits: " + Currencies.GetCredits();
            // Check if each upgrade has already been purchased
            if (shopDurabilityButton) shopDurabilityButton.interactable = !ShopManager.Instance.BoughtDurability();
            if (shopMovementSpeedButton) shopMovementSpeedButton.interactable = !ShopManager.Instance.BoughtMovementSpeed();
            if (shopFiringRateButton) shopFiringRateButton.interactable = !ShopManager.Instance.BoughtFiringRate();
        }
    }
}