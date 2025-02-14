using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : AView
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;

    public override void Initialize()
    {
        if (startButton) startButton.onClick.AddListener(OnStartButtonClicked);
        if (optionsButton) optionsButton.onClick.AddListener(OnOptionButtonClicked);
        if (creditsButton) creditsButton.onClick.AddListener(OnCreditsButtonClicked);
        if (exitButton) exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    public override void Deinitialize()
    {
        if (startButton) startButton.onClick.RemoveListener(OnStartButtonClicked);
        if (optionsButton) optionsButton.onClick.RemoveListener(OnOptionButtonClicked);
        if (creditsButton) creditsButton.onClick.RemoveListener(OnCreditsButtonClicked);
        if (exitButton) exitButton.onClick.RemoveListener(OnExitButtonClicked);
    }
    
    private void OnStartButtonClicked()
    {
        UIManager.Show<LevelSelectView>();
    }

    private void OnOptionButtonClicked()
    {
        UIManager.Show<OptionsView>();
    }
    
    private void OnCreditsButtonClicked()
    {
        UIManager.Show<CreditsView>();
    }

    private void OnExitButtonClicked()
    {
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}