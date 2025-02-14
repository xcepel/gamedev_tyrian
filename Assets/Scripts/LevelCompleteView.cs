using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteView : AView
{
    [SerializeField] private Button upgradesShopButton;
    [SerializeField] private Button backToMenuButton;
    
    [SerializeField] private TextMeshProUGUI scoreText;

    public override void Initialize()
    {
        if (upgradesShopButton) upgradesShopButton.onClick.AddListener(OnUpgradesShopClicked);
        if (backToMenuButton) backToMenuButton.onClick.AddListener(OnBackButtonClicked);
        scoreText.text = "Score: " + Currencies.GetScore();
    }

    public override void Deinitialize()
    {
        if (upgradesShopButton) upgradesShopButton.onClick.RemoveAllListeners();
        if (backToMenuButton) backToMenuButton.onClick.RemoveAllListeners();
    }
    
    private void OnUpgradesShopClicked()
    {
        UIManager.Show<UpgradesShopView>();
    }

    private void OnBackButtonClicked()
    {
        Currencies.Instance.ResetCurrencies();
        CheatsManager.Instance.Destroy();
        ShopManager.Instance.Destroy();
        
        UIManager.Show<MainMenuView>();
    }
    
    public override void DoShow(object args)
    {
        base.DoShow(args);
        scoreText.text = "Score: " + Currencies.GetScore();
    }
}