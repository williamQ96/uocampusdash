using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject playerPrefab;
    private GameObject currentPlayer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "campus")
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        Debug.Log("[GameManager] Restarting game...");

        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        GameObject spawn = GameObject.Find("PlayerSpawn");
        if (spawn == null)
        {
            Debug.LogError("⚠️ 'PlayerSpawn' not found in campus scene.");
            return;
        }

        currentPlayer = Instantiate(playerPrefab, spawn.transform.position, Quaternion.identity);
        currentPlayer.name = "Player";

        StarterAssetsInputs.inputEnabled = false;

        var ui = FindObjectOfType<GameUIManager>();
        if (ui != null)
            ui.ShowMainMenu();
    }
}
