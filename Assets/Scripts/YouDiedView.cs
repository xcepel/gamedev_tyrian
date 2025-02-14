using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YouDiedView : AView
{
    [SerializeField] private Button backToMenuButton;

    [SerializeField] private TextMeshProUGUI scoreText;

    public override void Initialize()
    {
        if (backToMenuButton) backToMenuButton.onClick.AddListener(OnBackButtonClicked);
        scoreText.text = "Score: " + Currencies.GetScore();
    }

    public override void Deinitialize()
    {
        if (backToMenuButton) backToMenuButton.onClick.RemoveAllListeners();
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