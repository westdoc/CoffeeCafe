using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjectiveMarker : MonoBehaviour
{
    private ColorAssigner colorAssigner;

    private void Awake()
    {
        colorAssigner = GetComponent<ColorAssigner>();
        if (colorAssigner == null)
        {
            Debug.LogError("ObjectiveMarker is missing RandomColorAssigner!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            var playerColorAssigner = player.gameObject.GetComponentInChildren<ColorAssigner>();
            if (playerColorAssigner != null && colorAssigner != null)
            {
                if (playerColorAssigner.selectedColor == colorAssigner.selectedColor)
                {
                    playerColorAssigner.setColor(ColorData.ColorOption.Empty);
                    // Add score to the GameManager
                    GameManager.Instance.AddScore(1);
                    gameObject.SetActive(false);

                    // Destroy this objective marker
                    Destroy(gameObject, 0.2f);
                }
            }
        }
    }
}