using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class ColorPicker : MonoBehaviour
{
    public ColorData.ColorOption assignedColor;

    private List<PlayerMovement> playersInTrigger = new List<PlayerMovement>();

    private void Update()
    {
        foreach (var player in playersInTrigger)
        {
            InputDevice device = player.GetAssignedDevice();

            bool selectPressed = false;

            if (device != null)
            {
                if (device is Gamepad gamepad)
                {
                    if (Gamepad.current == gamepad && gamepad.buttonSouth.wasPressedThisFrame)
                    {
                        selectPressed = true;
                    }
                }
                else if (device is Keyboard keyboard)
                {
                    if (Keyboard.current == keyboard && keyboard.eKey.wasPressedThisFrame)
                    {
                        selectPressed = true;
                    }
                }
            }

            if (selectPressed)
            {
                var playerColorAssigner = player.gameObject.GetComponentInChildren<ColorAssigner>();
                if (playerColorAssigner != null)
                {
                    playerColorAssigner.setColor(assignedColor);
                    Debug.Log($"Player changed color to {assignedColor}");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerMovement>();
        if (player != null && !playersInTrigger.Contains(player))
        {
            playersInTrigger.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<PlayerMovement>();
        if (player != null && playersInTrigger.Contains(player))
        {
            playersInTrigger.Remove(player);
        }
    }
}
