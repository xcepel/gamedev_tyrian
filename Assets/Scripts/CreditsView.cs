using UnityEngine;
using UnityEngine.UI;

public class CreditsView : AView
{
    [SerializeField] private Button backButton;

    public override void Initialize()
    {
        if (backButton) backButton.onClick.AddListener(OnBackButtonClicked);
    }

    public override void Deinitialize()
    {
        if (backButton) backButton.onClick.RemoveAllListeners();
    }

    private void OnBackButtonClicked()
    {
        UIManager.Show<MainMenuView>();
    }
}
