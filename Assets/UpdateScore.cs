using UnityEngine;
using TMPro;

public class GameUIScoreUpdater : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        scoreText = GetComponentInChildren<TextMeshProUGUI>();

        if (scoreText == null)
        {
            Debug.LogError("GameUIScoreUpdater: No TextMeshProUGUI component found in children!");
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || scoreText == null)
            return;

        scoreText.text = $"SCORE: {GameManager.Instance.GetScore()}";
    }
}