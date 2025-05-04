using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuUIUpdater : MonoBehaviour
{
    [System.Serializable]
    public class PlayerEntryUI
    {
        public TextMeshProUGUI playerNameText;
        public Image playerColorImage;
    }

    public TextMeshProUGUI menuInstruction;

    public List<PlayerEntryUI> playerEntries;

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        int i = 0;

        foreach (var (device, color) in GameManager.Instance.GetPendingPlayers())
        {
            playerEntries[i].playerNameText.gameObject.SetActive(true);
            playerEntries[i].playerColorImage.gameObject.SetActive(true);

            playerEntries[i].playerNameText.text = device.displayName;
            playerEntries[i].playerColorImage.color = color;
            i++;
          
        }

        if(i > 0)
        {
            menuInstruction.gameObject.SetActive(true);
        } else
        {
            menuInstruction.gameObject.SetActive(false);
        }

        for(; i < playerEntries.Count; i++)
        {
            playerEntries[i].playerNameText.gameObject.SetActive(false);
            playerEntries[i].playerColorImage.gameObject.SetActive(false);
        }
    }
}
