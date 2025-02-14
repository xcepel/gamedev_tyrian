using System;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform healthBarForeground;

    // Sets the new percentage of the health bar fill
    public void SetFillLevel(float newFillLevel)
    {
        healthBarForeground.localScale = new Vector3(Mathf.Clamp(newFillLevel / 1.0f, 0.0f, 1.0f), 1.0f, 1.0f);

        if (newFillLevel <= 0.0f)
        {
            HideHealthBar();
        }
        else
        {
            ShowHealthBar();
        }
    }

    // Hide the health bar and its associated object (including the model and health bar)
    private void HideHealthBar()
    {
        gameObject.SetActive(false);
    }

    // Show the health bar and its associated object (including the model and health bar)
    private void ShowHealthBar()
    {
        gameObject.SetActive(true);
    }
}