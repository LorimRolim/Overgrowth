using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _playerIndex;
    public PlayerOneScript ActivePlayerScript;
    public PlayerOneScript[] PlayerScripts;
    public List<GameObject> InActiveGameObjects = new List<GameObject>();

    public float BoardBorder = 5;

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
        EraseInActiveGameObjects();
    }

    private void EraseInActiveGameObjects()
    {
        for (int i = InActiveGameObjects.Count - 1; i >= 0; i--)
        {
            Destroy(InActiveGameObjects[i]);
            InActiveGameObjects.RemoveAt(i);
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
