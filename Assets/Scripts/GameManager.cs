
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int PlayerIndex;
    public PlayerOneScript ActivePlayerScript;
    public PlayerOneScript[] PlayerScripts;
    public List<GameObject> InActiveGameObjects = new List<GameObject>();

    public float BoardBorder = 5f;
    public float WaterLevel = 0f;
    
    public float WaterDepletingAmount = 0.02f;
    [SerializeField] private GameObject _waterLevelPlane;
    [SerializeField] private Image _border;

    public bool IsNextRound=false;
    public bool IsNewTurn;

    [SerializeField] private int _turnCounter = 1;
    [SerializeField] private int _previousTurn = 0;
    [SerializeField] private int _roundCounter = 0;
    [SerializeField] private int _previousRound=-1;
    public int RainFallTime=6;

    void Start()
    {
        // Ensure the game starts with Player 1's camera
       
        SetStartConditions();
        DeActivatePlayerScripts();
        //IsNextRound = true;
    }

    private void SetStartConditions()
    {
        ActivePlayerScript=PlayerScripts[0];
        PlayerScripts[1].enabled=false;
        PlayerScripts[2].enabled = false;
        PlayerScripts[3].enabled = false;
        ActivePlayerScript.WaterPoints = 1;
        ActivePlayerScript.SunLightPoints = 1;
        _border.color = ActivePlayerScript.LeafMat.color;

    }

    void Update()
    {
        _waterLevelPlane.transform.position =new Vector3(0, WaterLevel,0);
        _previousRound = _roundCounter;
        _previousTurn = _turnCounter;
        if (ActivePlayerScript.IsChopping)
        {
            ActivePlayerScript.IsGrowing = false;
        }
        if (ActivePlayerScript.IsGrowing)
        {
            ActivePlayerScript.IsChopping = false;
        }
        
        
        if (Input.GetKeyDown(KeyCode.RightShift) && !ActivePlayerScript.HasTooLittleWater) // KeyCode.Return corresponds to the Enter key
        {
            UpdateActivePlayer();
            
            _turnCounter++;
        }
        
        if (_turnCounter >= PlayerScripts.Length) //next round
        {
            //SetAllPlayersActive();
            
            _turnCounter = 0;
            _roundCounter++;
        }
        if (_previousTurn != _turnCounter)
        {
            IsNewTurn=true;
        }
        else
        {
            IsNewTurn=false;
        }
        if (_roundCounter == RainFallTime)
        {
            _roundCounter = 0;
            WaterLevel += 2;
        }
        //if (_previousRound != _roundCounter)
        //{
        //    IsNextRound = true;
        //}
        //else
        //{
        //    IsNextRound=false;
        //}
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

    public void UpdateActivePlayer()
    {
        
        PlayerIndex++;
        
        if (PlayerIndex > PlayerScripts.Length - 1)
        {
            PlayerIndex = 0;
        }
        ActivePlayerScript = PlayerScripts[PlayerIndex];
        _border.color=ActivePlayerScript.LeafMat.color;
        DeActivatePlayerScripts() ;
    }

    public void DeActivatePlayerScripts()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == PlayerIndex)
            {
                PlayerScripts[i].enabled = true;
                foreach (var leaf in PlayerScripts[i].MyLeaves)
                {
                    leaf.GetComponent<BoxCollider>().enabled = true;
                }
                foreach (var leaf in PlayerScripts[i].MyRoots)
                {
                    leaf.GetComponent<BoxCollider>().enabled = true;
                }
            }
            else
            {
                foreach(var leaf in PlayerScripts[i].MyLeaves)
                {
                    leaf.GetComponent<BoxCollider>().enabled = false;
                }
                foreach (var leaf in PlayerScripts[i].MyRoots)
                {
                    leaf.GetComponent<BoxCollider>().enabled = false;
                }

                PlayerScripts[i].enabled = false;
            }
        }
    }
}
