using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Menu,
        Playing
    }

    public GameObject playerPrefab;
    public GameObject objectiveMarkerPrefab;
    public Transform spawnArea; // New spawn area reference

    public GameState currentState = GameState.Menu;

    private Dictionary<InputDevice, Color> pendingPlayers = new Dictionary<InputDevice, Color>();
    private Dictionary<InputDevice, GameObject> players = new Dictionary<InputDevice, GameObject>();

    private List<GameObject> activeObjectives = new List<GameObject>();
    public float objectiveSpawnTimer = 0f;
    private float objectiveSpawnInterval = 10f;
    private int score = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public Dictionary<InputDevice, Color> GetPendingPlayers()
    {
        return pendingPlayers;
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.Menu:
                HandleMenu();
                break;
            case GameState.Playing:
                HandleGameplay();
                break;
        }
    }

    private void HandleMenu()
    {
        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad gamepad && gamepad.startButton.wasPressedThisFrame)
            {
                RegisterPlayer(device);
            }
            else if (device is Keyboard keyboard && keyboard.enterKey.wasPressedThisFrame)
            {
                RegisterPlayer(device);
            }

            if (device is Gamepad gamepad2)
            {
                if (Gamepad.current == gamepad2 && gamepad2.buttonSouth.wasPressedThisFrame)
                {
                    StartGame();
                }
            }
            else if (device is Keyboard keyboard)
            {
                if (Keyboard.current == keyboard && keyboard.eKey.wasPressedThisFrame)
                {
                    StartGame();
                }
            }
        }
    }

    private void HandleGameplay()
    {
        //foreach (var device in players.Keys)
        //{
        //    // nothing needs to happen for players now, they are already spawned
        //}

        // Objective spawning logic
        objectiveSpawnTimer += Time.deltaTime;
        if (objectiveSpawnTimer >= objectiveSpawnInterval)
        {
            objectiveSpawnTimer = Random.Range(0.0f, 8.0f);
            SpawnObjective();
        }
    }

    private void RegisterPlayer(InputDevice device)
    {
        if (!pendingPlayers.ContainsKey(device))
        {
            Color randomColor = new Color(Random.Range(0.25f, 0.8f), Random.Range(0.25f, 0.8f), Random.Range(0.25f, 0.8f)); // completely random color
            pendingPlayers[device] = randomColor;
            Debug.Log($"Registered player {device.displayName} with color {randomColor}");
        } else
        {
            pendingPlayers.Remove(device);
            Debug.Log($"Unregistered player {device.displayName}");
        }
    }

    private void StartGame()
    {
        Debug.Log("Starting Game!");

        foreach (var entry in pendingPlayers)
        {
            var device = entry.Key;
            var color = entry.Value;
            var playerObj = Instantiate(playerPrefab, GetSafeSpawnPosition(), Quaternion.identity);
            var playerMovement = playerObj.GetComponent<PlayerMovement>();
            CameraManager.GetInstance().AddFocus(playerObj.transform);
            playerMovement.Initialize(device);

            MeshRenderer meshRenderer = playerObj.GetComponent<MeshRenderer>();
            if (meshRenderer != null) {
                // Create a new material instance to avoid modifying the shared material
                Material newMaterial = new Material(meshRenderer.material);

                newMaterial.color = color;

                // Apply the new material to the mesh renderer
                meshRenderer.material = newMaterial;
            }

            players[device] = playerObj;
        }

        pendingPlayers.Clear();
        currentState = GameState.Playing;
    }

    private void SpawnPlayer(InputDevice device)
    {
        var player = Instantiate(playerPrefab, GetSafeSpawnPosition(), Quaternion.identity);
        var playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.Initialize(device);
        players[device] = player;

        Debug.Log("Spawned player for device: " + device.displayName);
    }

    private void SpawnObjective()
    {
        Vector3 spawnPos = GetSafeSpawnPosition();
        var objective = Instantiate(objectiveMarkerPrefab, spawnPos, Quaternion.identity);
        var colorAssigner = objective.GetComponent<ColorAssigner>();
        if (colorAssigner != null)
        {
            ColorData.ColorOption randomColor = (ColorData.ColorOption)Random.Range((int)ColorData.ColorOption.Red, (int)(ColorData.ColorOption.Blue + 1));
            colorAssigner.setColor(randomColor);
        }

        activeObjectives.Add(objective);

        Debug.Log("Spawned 1 new objectives.");
    }

    private Vector3 GetSafeSpawnPosition()
    {
        Vector3 spawnPos;
        bool safe;
        int maxAttempts = 20;
        int attempts = 0;

        do
        {
            spawnPos = GetSpawnPosition();
            safe = true;

            foreach (var obj in activeObjectives)
            {
                if (obj != null && Vector3.Distance(obj.transform.position, spawnPos) < 1.6f)
                {
                    safe = false;
                    break;
                }
            }

            attempts++;
        }
        while (!safe && attempts < maxAttempts);

        return spawnPos;
    }

    private Vector3 GetSpawnPosition()
    {
        if (spawnArea != null)
        {
            Vector3 center = spawnArea.position;
            Vector3 size = spawnArea.localScale;

            float x = Random.Range(center.x - size.x * 0.5f, center.x + size.x * 0.5f);
            float y = center.y;
            float z = Random.Range(center.z - size.z * 0.5f, center.z + size.z * 0.5f);

            return new Vector3(x, y, z);
        }
        else
        {
            return new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
    }

    public int GetScore()
    {
        return score;
    }
}
