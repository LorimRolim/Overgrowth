

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int PlayerIndex;
    public PlayerOneScript ActivePlayerScript;
    public PlayerOneScript[] PlayerScripts;
    public List<GameObject> InActiveGameObjects = new List<GameObject>(); 

    public float BoardBorder = 5f;
    public float BoardHeight = 5f;
    public float WaterLevel = 0f;
    
    public float WaterDepletingAmount = 0.02f;
    [SerializeField] private GameObject _waterLevelPlane;
    [SerializeField] private Image _border;

    public bool IsNextRound=false;
    public bool IsNewTurn;

    [SerializeField] private GameObject _groundPlane;
    [SerializeField] private GameObject _waterPlane;
    [SerializeField] private GameObject _boardHeightVisualization;

    [SerializeField] private int _turnCounter = 1;
    [SerializeField] private int _previousTurn = 0;
    [SerializeField] private int _roundCounter = 0;
    [SerializeField] private int _previousRound=-1;
    public int RainFallTime=6;

    public float WinCondition;
    private bool _nextTurnButtonWasClicked;

    public GameOverScript GameOver;

    void Start()
    {
        // Ensure the game starts with Player 1's camera
        WinCondition = ((BoardBorder * BoardBorder) * 4)*0.75f;
        SetStartConditions();
        DeActivatePlayerBodies();
        //IsNextRound = true;
        SetBoardSize();
        
        GameObject GameOverManager=GameObject.Find("GameOver Manager");
        GameOver = GameOverManager.GetComponent<GameOverScript>();
        GameOver.Winner = null;
    }

    private void SetStartConditions()
    {
        SetStartPositions();
        GrowStartTree();
        ActivePlayerScript =PlayerScripts[0];
        PlayerScripts[1].enabled=false;
        PlayerScripts[2].enabled = false;
        PlayerScripts[3].enabled = false;
        ActivePlayerScript.WaterPoints = 1;
        ActivePlayerScript.SunLightPoints = 1;
        _border.color = ActivePlayerScript.LeafMat.color;
        ActivePlayerScript.IsGrowing = true;

        
    }
    

    private void SetBoardSize()
    {
        _groundPlane.transform.localScale=new Vector3((BoardBorder*2)/10, 1, (BoardBorder*2) / 10);
        _waterPlane.transform.localScale = new Vector3((BoardBorder*2) / 10, 1, (BoardBorder*2) / 10);
        _boardHeightVisualization.transform.localScale = new Vector3(BoardBorder * 2, BoardHeight * 2, BoardBorder * 2);
    }

    private void GrowStartTree()
    {
        PlayerScripts[0].SetStartConditions();
        PlayerScripts[1].SetStartConditions();
        PlayerScripts[2].SetStartConditions();
        PlayerScripts[3].SetStartConditions();
    }

    private void SetStartPositions()
    {
        PlayerScripts[0].PlayerStartPosition= new Vector3(BoardBorder-0.5f,0.5f,BoardBorder-0.5f);
        PlayerScripts[1].PlayerStartPosition = new Vector3(-BoardBorder + 0.5f, 0.5f, BoardBorder - 0.5f);
        PlayerScripts[2].PlayerStartPosition = new Vector3(BoardBorder - 0.5f, 0.5f, -BoardBorder + 0.5f);
        PlayerScripts[3].PlayerStartPosition = new Vector3(-BoardBorder + 0.5f, 0.5f, -BoardBorder + 0.5f);
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

        //OnClickNextTurn();

        if (_nextTurnButtonWasClicked)
        {
            NextTurnButton();
            _nextTurnButtonWasClicked = false;
        }
        

        if (_turnCounter >= PlayerScripts.Length) //next round
        {
            //SetAllPlayersActive();
            
            _turnCounter = 0;
            _roundCounter++;
            IsNextRound = true;
        }
        else
        {
            IsNextRound = false;
        }

        //turn
        if (_previousTurn != _turnCounter)
        {
            IsNewTurn=true;
        }
        else
        {
            IsNewTurn=false;
        }
        

        //rain
        if (_roundCounter == RainFallTime)
        {
            _roundCounter = 0;
            WaterLevel += Random.Range(0.5f,2);
        }

        //erase
        EraseInActiveGameObjects();

        //wincondition
        foreach (var player in PlayerScripts)
        {
            if (player.HasWon)
            {
                GameOver.Winner = player.gameObject;
                //save them and apoint them the winner;
                if (IsNextRound)
                {
                    SceneManager.LoadScene("End Screen");
                }

            }
        }
    }

    public void OnClickNextTurn()
    {
        _nextTurnButtonWasClicked=true;
    }

    private void NextTurnButton()
    {
        if (!ActivePlayerScript.HasTooLittleWater) // KeyCode.Return corresponds to the Enter key
        {
            UpdateActivePlayer();
        }
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
        DeActivatePlayerBodies();
        _turnCounter++;
    }

    public void DeActivatePlayerBodies()
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
