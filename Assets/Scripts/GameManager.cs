using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _playerIndex;
    public PlayerOneScript ActivePlayerScript;
    public PlayerOneScript[] PlayerScripts;

    void Start()
    {
        // Ensure the game starts with Player 1's camera
        UpdateActivePlayer();
    }

    void Update()
    {
        // Check if the Enter key is pressed
        if (Input.GetKeyDown(KeyCode.RightShift)) // KeyCode.Return corresponds to the Enter key
        {
            UpdateActivePlayer();
        }
    }

    private void UpdateActivePlayer()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            _playerIndex++;
        }
        
        if (_playerIndex > PlayerScripts.Length - 1)
        {
            _playerIndex = 0;
        }
        ActivePlayerScript = PlayerScripts[_playerIndex];
        //deactivate the player script of non active players
    }

    

    
    
}
