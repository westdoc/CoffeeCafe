
using UnityEngine;
public class MainCanvasManager : MonoBehaviour
{
    public CanvasGroup menuCanvasGroup;
    public CanvasGroup gameplayCanvasGroup;

    public float fadeDuration = 0.5f;

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        switch (GameManager.Instance.currentState)
        {
            case GameManager.GameState.Menu:
                FadeCanvasGroup(menuCanvasGroup, 1f);
                FadeCanvasGroup(gameplayCanvasGroup, 0f);
                break;

            case GameManager.GameState.Playing:
                FadeCanvasGroup(menuCanvasGroup, 0f);
                FadeCanvasGroup(gameplayCanvasGroup, 1f);
                break;
        }
    }

    private void FadeCanvasGroup(CanvasGroup cg, float targetAlpha)
    {
        if (cg == null)
            return;

        cg.alpha = Mathf.MoveTowards(cg.alpha, targetAlpha, fadeDuration * Time.deltaTime);
        cg.interactable = (cg.alpha > 0.95f);
        cg.blocksRaycasts = (cg.alpha > 0.95f);
    }
}