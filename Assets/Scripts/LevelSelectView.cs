using UnityEngine;
using UnityEngine.UI;

public class LevelSelectView : AView
{
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    
    [SerializeField] private Button shopDurabilityButton;
    [SerializeField] private Button shopMovementSpeedButton;
    [SerializeField] private Button shopFiringRateButton;
    
    [SerializeField] private Button backButton;

    private Color defaultColor;
    private Color toggledColor = Color.blue;

    public override void Initialize()
    {
        if (shopDurabilityButton) shopDurabilityButton.onClick.AddListener(OnShopDurabilityButtonClicked);
        if (shopMovementSpeedButton) shopMovementSpeedButton.onClick.AddListener(OnShopMovementSpeedButtonClicked);
        if (shopFiringRateButton) shopFiringRateButton.onClick.AddListener(OnShopFiringRateButtonClicked);

        if (level1Button) level1Button.onClick.AddListener(() => LoadLevel("Level_1"));
        if (level2Button) level2Button.onClick.AddListener(() => LoadLevel("Level_2"));
        if (level3Button) level3Button.onClick.AddListener(() => LoadLevel("Level_3"));
        
        if (backButton) backButton.onClick.AddListener(OnBackButtonClicked);
    }

    public override void Deinitialize()
    {
        if (shopDurabilityButton) shopDurabilityButton.onClick.RemoveAllListeners();
        if (shopMovementSpeedButton) shopMovementSpeedButton.onClick.RemoveAllListeners();
        if (shopFiringRateButton) shopFiringRateButton.onClick.RemoveAllListeners();

        if (level1Button) level1Button.onClick.RemoveAllListeners();
        if (level2Button) level2Button.onClick.RemoveAllListeners();
        if (level3Button) level3Button.onClick.RemoveAllListeners();
        
        if (backButton) backButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        ColorUtility.TryParseHtmlString("#AAE9FF", out defaultColor);
    }
    
    public override void DoShow(object args)
    {
        gameObject.SetActive(true);

        ResetShop();
        
        GameObject shopManagerGO = new GameObject("ShopManager");
        ShopManager shopManager = shopManagerGO.AddComponent<ShopManager>();
    }

    private void OnBackButtonClicked()
    {
        ShopManager.Instance.Destroy();
        ResetShop();
        
        UIManager.Show<MainMenuView>();
    }
    
    private void LoadLevel(string levelName)
    {
        UIManager.Instance.HideAllViews();
        GameManager.ChangeScene(levelName);
    }
    
    private void OnShopDurabilityButtonClicked()
    {
        ShopManager.Instance.BuyDurability(0);
        shopDurabilityButton.interactable = false;
    }
    
    private void OnShopMovementSpeedButtonClicked()
    {
        ShopManager.Instance.BuyMovementSpeed(0);
        shopMovementSpeedButton.interactable = false;
    }
    
    private void OnShopFiringRateButtonClicked()
    {
        ShopManager.Instance.BuyFiringRate(0);
        shopFiringRateButton.interactable = false;
    }

    private void ResetShop()
    {
        shopDurabilityButton.interactable = true;
        shopFiringRateButton.interactable = true;
        shopMovementSpeedButton.interactable = true;
    }
}