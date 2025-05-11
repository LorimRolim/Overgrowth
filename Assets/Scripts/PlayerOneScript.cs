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
    
    public bool IsGrowing=true;
    public bool IsChopping=false;
    
    [SerializeField] private GameManager _gameManager;

    [Header("points")]
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetMyMaterial();
        PlayerStartPosition = transform.position;

        //spawn 1 leaf + 1 root
        GameObject firstLeaf=Instantiate(TreeLeaf, PlayerStartPosition,Quaternion.identity); 
        MyLeaves.Add(firstLeaf);
        GameObject firstRoot=Instantiate(TreeRoot,PlayerStartPosition-Vector3.up,Quaternion.identity);
        MyRoots.Add(firstRoot);
    }

    private void SetMyMaterial()
    {
        _leafRenderer.material = LeafMat;
        _rootRenderer.material = _rootMat;
    }

    // Update is called once per frame
    void Update()
    {
        LeafCount=MyLeaves.Count;
        
        //CalculateWaterPoints();
        if (SunLightPoints <= 0)
        {
            IsGrowing = false;
        }

        foreach (var leaf in MyLeaves)
        {
            leaf.GetComponent<MeshRenderer>().material = LeafMat;
            
            Leaves leafScript = leaf.GetComponent<Leaves>();

            leafScript.CheckIfInShadow();
            if (_gameManager.IsNewTurn)
            {
                CalculateAcquiredSunPoints(leafScript);
            }
        }
        foreach (var root in MyRoots)
        {
            root.GetComponent<MeshRenderer>().material = _rootMat;
            Root rootScript = root.GetComponent<Root>();
            if (_gameManager.IsNewTurn)
            {
                rootScript.GiveWaterpoints();
            }
        }

        if (_gameManager.IsNewTurn)
        {
            if ((WaterPoints - LeafCount) < 0)
            {
                HasTooLittleWater = true;
            }
            else
            {
                HasTooLittleWater = false;
            }
        }
        if(HasTooLittleWater && ((WaterPoints - LeafCount) >= 0))
        {
            HasTooLittleWater = false;
            WaterPoints -= LeafCount;
            _gameManager.UpdateActivePlayer();
        }
        //make sure that when too liitle water vcant continue unless you reduce tree size
        if (HasTooLittleWater)
        {
            IsChopping = true;
            _growButtonImage.color =Color.red;
        }
        

        if (!HasTooLittleWater && _gameManager.IsNewTurn)
        {
            WaterPoints -= LeafCount;
            _growButtonImage.color = Color.green;
        }
        
        //Update UI points
        SunPointsText.text = SunLightPoints.ToString();
        WaterPointsText.text = WaterPoints.ToString();
        LeafCountText.text = LeafCount.ToString();
    }

    
    private void CalculateAcquiredSunPoints(Leaves leafScript)
    {
        if (!leafScript.IsInShadowed)
        {
            SunLightPoints++;
        }
    }

}
