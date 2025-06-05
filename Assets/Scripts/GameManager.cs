

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int PlayerIndex;
    public PlayerOneScript ActivePlayerScript;
    public List<PlayerOneScript> PlayerScripts;
    public List<GameObject> InActiveGameObjects = new List<GameObject>();
    [SerializeField] private MouseInputSystem mouseInputScript;

    public float BoardBorder = 5f;
    public float BoardHeight = 5f;
    public float WaterLevel = 0f;
    
    public float WaterDepletingAmount = 0.02f;
    [SerializeField] private GameObject _waterLevelPlane;

    [Header("color of ui")]
    [SerializeField] private Image _sunlightPointColor;
    [SerializeField] private Image _waterPointColor;
    [SerializeField] private Image _leafCountColor;
    [SerializeField] private Image _shadowLeavesColor;
    [SerializeField] private Image _rootsColor;
    [SerializeField] private Image _nextTurnColor;
    [SerializeField] private Image _border;
    [SerializeField] private Image _algeaSpriteColor;
    [SerializeField] private Image _leafSpriteColor;
    [SerializeField] private Image _leafShadowSpriteColor;
    [SerializeField] private Image _rootSpriteColor;

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
    public bool NextTurnButtonWasClicked;
    [SerializeField] private ParticleSystem _rainParticles;
    private ParticleSystem _rainParticlesInstance;

    [Header("cursor textures")]
    [SerializeField] private Texture2D _growCursorsprite;
    [SerializeField] private Texture2D _chopCursorsprite;
    private Vector2 _chopHotSpot=new Vector2(9f,97f);
    private Vector2 _growHotSpot=new Vector2(38f,1f);
    public LayerMask OtherPlayers;
    public int ActivePlayerIndex;

    [Header("start conditions")]
    [SerializeField] private int _startPlayerSunStart;
    [SerializeField] private int _otherSunStart;
    [SerializeField] private int _startPlayerWaterStart;
    [SerializeField] private int _otherWaterStart;


    void Start()
    {
        // Ensure the game starts with Player 1's camera
        WinCondition = ((BoardBorder * BoardBorder) * 4)*0.75f;
        SetStartConditions();
        //DeActivatePlayerBodies();
        //IsNextRound = true;
        SetBoardSize();
        
        GameOverScript.Winner = null;
    }

    private void SetStartConditions()
    {
        SetStartPositions();
        GrowStartTree();
        ActivePlayerScript =PlayerScripts[0];
        SetStartPoints();
        PlayerScripts[1].enabled=false;
        PlayerScripts[2].enabled = false;
        PlayerScripts[3].enabled = false;
        ActivePlayerScript.WaterPoints = 1;
        

        SetColorsUI();

        ActivePlayerScript.IsGrowing = true;
    }

    private void SetStartPoints()
    {
        PlayerScripts[0].SunLightPoints=_startPlayerSunStart;
        PlayerScripts[1].SunLightPoints = _otherSunStart;
        PlayerScripts[2].SunLightPoints = _otherSunStart;
        PlayerScripts[3].SunLightPoints = _otherSunStart;
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
        //ActivePlayerScript = PlayerScripts[ActivePlayerIndex];
        //SetOtherPlayerLayerMask();
        //Debug.Log(Convert.ToString(OtherPlayers, 2).PadLeft(32, '0'));

        _waterLevelPlane.transform.position =new Vector3(0, WaterLevel,0);
        _previousRound = _roundCounter;
        _previousTurn = _turnCounter;
        
        if (ActivePlayerScript.IsChopping)
        {
            Cursor.SetCursor(_chopCursorsprite, _chopHotSpot,CursorMode.Auto);
            ActivePlayerScript.IsGrowing = false;

        }
        if (ActivePlayerScript.IsGrowing)
        {
            Cursor.SetCursor(_growCursorsprite, _growHotSpot, CursorMode.Auto);
            ActivePlayerScript.IsChopping = false;
        }

        //OnClickNextTurn();

        if (NextTurnButtonWasClicked)
        {
            NextTurnButton();
            NextTurnButtonWasClicked = false;
        }
        

        if (_turnCounter >= PlayerScripts.Count) //next round
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
            WaterLevel += UnityEngine.Random.Range(0.5f,1);
            _rainParticlesInstance=Instantiate(_rainParticles);
        }

        //erase
        EraseInActiveGameObjects();

        //wincondition
        foreach (var player in PlayerScripts)
        {
            if (player.HasWon)
            {
                GameOverScript.Winner = player.gameObject.name;
                //save them and apoint them the winner;
                SceneManager.LoadScene("End Screen");
                
            }
        }
    }


    public void OnClickNextTurn()
    {
        NextTurnButtonWasClicked=true;
    }

    private void NextTurnButton()
    {
        if (!ActivePlayerScript.HasTooLittleWater) // KeyCode.Return corresponds to the Enter key
        {
            
            UpdateActivePlayer();
        }
    }

    public void EraseInActiveGameObjects()
    {
        for (int i = InActiveGameObjects.Count - 1; i >= 0; i--)
        {
            Destroy(InActiveGameObjects[i]);
            InActiveGameObjects.RemoveAt(i);
        }
    }

    public void UpdateActivePlayer()
    {
        mouseInputScript.ErasePotentialCubes();
        PlayerIndex++;
        
        if (PlayerIndex > PlayerScripts.Count-1)
        {
            PlayerIndex = 0;
        }
        ActivePlayerIndex=PlayerIndex;
        ActivePlayerScript = PlayerScripts[PlayerIndex];
        SetColorsUI();
        DeActivatePlayerBodies();
        _turnCounter++;
        IsNewTurn = true;
    }

    private void SetColorsUI()
    {
        _border.color = ActivePlayerScript.LeafMat.color;
        _leafCountColor.color = ActivePlayerScript.LeafMat.color;
        _nextTurnColor.color = ActivePlayerScript.LeafMat.color;
        _shadowLeavesColor.color = ActivePlayerScript.LeafMat.color;
        _sunlightPointColor.color = ActivePlayerScript.LeafMat.color;
        _waterPointColor.color = ActivePlayerScript.LeafMat.color;
        _rootsColor.color = ActivePlayerScript.LeafMat.color;

        _rootSpriteColor.color= ActivePlayerScript.RootMat.color;
        _algeaSpriteColor.color = Color.green;
        _leafSpriteColor.color = ActivePlayerScript.LeafMat.color;
        _leafShadowSpriteColor.color = ActivePlayerScript.LeafMat.color;
    }

    public void DeActivatePlayerBodies()
    {
        for (int i = 0; i <= PlayerScripts.Count-1; i++)
        {
            if (i == PlayerIndex)
            {
                PlayerScripts[i].enabled = true;
                
            }
            else
            {
               
                PlayerScripts[i].enabled = false;
            }
        }
    }
}
