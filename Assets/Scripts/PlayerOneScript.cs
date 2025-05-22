using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOneScript : MonoBehaviour
{
    // have all variables 
    public List<GameObject> MyLeaves = new List<GameObject>();
    public List<GameObject> MyPotentialGrowCubes= new List<GameObject>();
    public List<GameObject> MyRoots = new List<GameObject>();
    [SerializeField] private WriteFeedbackOnScreen _feedbackWriter;
    
    public bool IsGrowing=true;
    public bool IsChopping=false;
    
    [SerializeField] private GameManager _gameManager;

    [Header("points")]
    private int _acquiredSunlightPoints;
    public int SunLightPoints;
    public int WaterPoints;
    public int LeafCount;

    public Text SunPointsText; // Reference to the Text component in the Canvas
    public Text WaterPointsText; // Reference to the Text component in the Canvas
    public Text LeafCountText; // Reference to the Text component in the Canvas

    public Vector3 PlayerStartPosition;

    public GameObject TreeLeaf;
    public GameObject TreeRoot;

    [SerializeField] private MeshRenderer _leafRenderer;
    [SerializeField] private MeshRenderer _rootRenderer;

    public Material LeafMat;
    [SerializeField] private Material _rootMat;

    [SerializeField] private float raycastDistance = 10f; // Distance the raycast will travel

    public bool HasTooLittleWater = false;

    [SerializeField] private Image _growButtonImage;

    public bool HasWon=false;
    public int AcquiredWaterPoints;

    public Text NotEnoughWater;

    public void SetStartConditions()
    {
        SetMyMaterial();

        //spawn 1 leaf + 1 root
        GameObject firstLeaf = Instantiate(TreeLeaf, PlayerStartPosition, Quaternion.identity);
        firstLeaf.layer = this.gameObject.layer;
        MyLeaves.Add(firstLeaf);
        GameObject firstRoot = Instantiate(TreeRoot, PlayerStartPosition - Vector3.up, Quaternion.identity);
        firstRoot.layer = this.gameObject.layer;
        MyRoots.Add(firstRoot);
        NotEnoughWater.enabled = false;
    }

    private void SetMyMaterial()
    {
        _leafRenderer.material = LeafMat;
        _rootRenderer.material = _rootMat;
    }

    // Update is called once per frame
    void Update()
    {
        _feedbackWriter=GetComponent<WriteFeedbackOnScreen>();

        LeafCount=MyLeaves.Count;
        
        //CalculateWaterPoints();
        if (SunLightPoints <= 0)
        {
            IsGrowing = false;
        }
        if (_gameManager.IsNewTurn)
        {
            IsGrowing = true;
        }

        foreach (var leaf in MyLeaves)
        {
            leaf.GetComponent<MeshRenderer>().material = LeafMat;
            
            Leaves leafScript = leaf.GetComponent<Leaves>();

            leafScript.CheckIfInShadow();
            if (_gameManager.IsNewTurn)
            {
                CalculateAcquiredSunPoints(leafScript);
                _feedbackWriter.IsVisualizing = true;
                _feedbackWriter.SunlightPoints = _acquiredSunlightPoints;
                _feedbackWriter.SunlightSign = "+";

                _feedbackWriter.WaterPoints = 0;
                _feedbackWriter.WaterSign = "+";

                _feedbackWriter.LeafsAdded = "0";
                _feedbackWriter.LeafSign = "+";
            }
        }
        foreach (var root in MyRoots)
        {
            root.GetComponent<MeshRenderer>().material = _rootMat;
            Root rootScript = root.GetComponent<Root>();
            if (_gameManager.IsNewTurn)
            {
                rootScript.GiveWaterpoints();
                _feedbackWriter.IsVisualizing = true;
                _feedbackWriter.SunlightPoints = 0;
                _feedbackWriter.SunlightSign = "+";

                _feedbackWriter.WaterPoints = AcquiredWaterPoints;
                _feedbackWriter.WaterSign = "+";

                _feedbackWriter.LeafsAdded = "0";
                _feedbackWriter.LeafSign = "+";
            }
        }

        if (_gameManager.IsNewTurn)
        {
            if ((WaterPoints - LeafCount) < 0) // account for shadowed cubes and such
            {
                HasTooLittleWater = true;
                _feedbackWriter.IsVisualizing = true;
                _feedbackWriter.SunlightPoints = 0;
                _feedbackWriter.SunlightSign = "+";

                _feedbackWriter.WaterPoints = LeafCount;
                _feedbackWriter.WaterSign = "-";

                _feedbackWriter.LeafsAdded = "0";
                _feedbackWriter.LeafSign = "+";
            }
            else
            {
                HasTooLittleWater = false;
            }
        }

        //make sure that when too little water cant continue unless you reduce tree size
        if (HasTooLittleWater)
        {
            IsChopping = true;
            _growButtonImage.color = Color.red;
            NotEnoughWater.enabled = true;
        }

        //reset waterpoints after chopping enough leaves
        if (HasTooLittleWater && ((WaterPoints - LeafCount) >= 0))// recalculate the waterpoints amount
        {
            HasTooLittleWater = false;
            SubtractWaterPoints();
            NotEnoughWater.enabled = false;
            //WaterPoints -= LeafCount;
            _gameManager.UpdateActivePlayer();
        }

        if (!HasTooLittleWater && _gameManager.IsNewTurn)
        {
            SubtractWaterPoints();
            NotEnoughWater.enabled = false;
            //WaterPoints -= LeafCount;
            _growButtonImage.color = Color.green;
        }
        if (_acquiredSunlightPoints >= _gameManager.WinCondition)
        {
            HasWon = true;
        }
        //Update UI points
        _acquiredSunlightPoints = 0;
        SunPointsText.text = SunLightPoints.ToString();
        WaterPointsText.text = WaterPoints.ToString();
        LeafCountText.text = LeafCount.ToString();
    }

    private void SubtractWaterPoints()
    {
        foreach (var leaf in _gameManager.ActivePlayerScript.MyLeaves)
        {
            Leaves leafScript = leaf.GetComponent<Leaves>();
            WaterPoints -= 1 * leafScript.ShadowMultiplier;
        }
    }

    private void CalculateAcquiredSunPoints(Leaves leafScript)
    {
        if (!leafScript.IsInShadowed)
        {
            _acquiredSunlightPoints++;
            SunLightPoints++;
        }
    }

}
