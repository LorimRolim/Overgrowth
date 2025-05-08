using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] players; // Array to hold the 4 players
    public Camera[] cameras; // Array to hold the cameras corresponding to the players
    private int currentPlayerIndex = 0; // Index of the current player

    void Start()
    {
        // Ensure the game starts with Player 1's camera
        UpdatePlayerTurns();
    }

    void Update()
    {
        // Check if the Enter key is pressed
        if (Input.GetKeyDown(KeyCode.Return)) // KeyCode.Return corresponds to the Enter key
        {
            NextTurn();
        }
    }

    public void NextTurn()
    {
        // Move to the next player
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        UpdatePlayerTurns();
    }

    private void UpdatePlayerTurns()
    {
        // Loop through all players
        for (int i = 0; i < players.Length; i++)
        {
            bool isCurrentPlayer = i == currentPlayerIndex;

            // Keep all players active
            players[i].SetActive(true);

            // Enable or disable the corresponding camera
            if (i < cameras.Length && cameras[i] != null)
            {
                cameras[i].gameObject.SetActive(isCurrentPlayer);

                // If this is the current player's camera, set it as the main camera
                if (isCurrentPlayer)
                {
                    // Disable the current main camera if it exists
                    if (Camera.main != null && Camera.main != cameras[i])
                    {
                        Camera.main.enabled = false;
                    }

                    // Set the new camera as the main camera
                    cameras[i].tag = "MainCamera";
                    cameras[i].enabled = true;
                }
            }
        }
    }
}
