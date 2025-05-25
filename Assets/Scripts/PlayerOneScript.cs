
using System.Collections.Generic;
using System.Linq;
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
    public int RootCount;
    public int ShadowLeafCount;

    public Text SunPointsText; // Reference to the Text component in the Canvas
    public Text WaterPointsText; // Reference to the Text component in the Canvas
    public Text LeafCountText; // Reference to the Text component in the Canvas
    public Text RootCountText;
    public Text ShadowLeavesCountText;

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
    public int SubtractedWaterPoints;

    public Text NotEnoughWater;

    public bool HasAlgea;
    [SerializeField] private Image _algeaImage;


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

        //reset waterpoints after chopping enough leaves
        if (HasAlgea)
        {
            _algeaImage.color = LeafMat.color;
        }
        else
        {
            _algeaImage.color = Color.gray;
        }

        //CalculateWaterPoints();
        if (SunLightPoints <= 0)
        {
            IsGrowing = false;
        }

        for (int i = MyLeaves.Count - 1; i >= 0; i--)
        {
            if (MyLeaves[i] == null)
            {
                MyLeaves.RemoveAt(i);
                continue;
            }
            MyLeaves[i].GetComponent<MeshRenderer>().material = LeafMat;
            Leaves leafScript = MyLeaves[i].GetComponent<Leaves>();
            leafScript.CheckIfInShadow();
            if (_gameManager.IsNewTurn)
            {
                CalculateAcquiredSunPoints(leafScript);
            }
        }

            //iterate all my leaves and roots get sunpoints, waterpoints
        //foreach (var leaf in MyLeaves)
        //{
        //    if (leaf == null)
        //    {
        //        MyLeaves.Remove(leaf);
        //    }
        //    leaf.GetComponent<MeshRenderer>().material = LeafMat;
            
        //    Leaves leafScript = leaf.GetComponent<Leaves>();

        //    leafScript.CheckIfInShadow();
        //    if (_gameManager.IsNewTurn)
        //    {
        //        CalculateAcquiredSunPoints(leafScript);
        //    }
        //}
        foreach (var root in MyRoots)
        {
            root.GetComponent<MeshRenderer>().material = _rootMat;
            Root rootScript = root.GetComponent<Root>();
            if (_gameManager.IsNewTurn)
            {
                rootScript.GiveWaterpoints();
            }
        }

        if (HasTooLittleWater && !CalculateHasTooLittleWater())// recalculate the waterpoints amount
        {
            HasTooLittleWater = false;
            SubtractWaterPoints();
            NotEnoughWater.enabled = false;
            _gameManager.NextTurnButtonWasClicked = true;
            //_gameManager.UpdateActivePlayer();
            _growButtonImage.color = Color.green;
        }

        CalculateLeafRootCounts();

        //die if no leaves
        if (LeafCount == 0)
        {
            for( int i=_gameManager.PlayerScripts.Count-1;i>=0;i--)
            {
                if (_gameManager.PlayerScripts[i] == this)
                {
                    _gameManager.PlayerScripts.RemoveAt(i);
                }
                
            }
            
            Destroy(this.gameObject);
        }

        //calculate if enough water
        if (_gameManager.IsNewTurn)
        {
            IsGrowing = true;
            if (CalculateHasTooLittleWater()) // account for shadowed cubes and such (WaterPoints - LeafCount) < 0
            {
                HasTooLittleWater = true;
                
            }
            if(!CalculateHasTooLittleWater()) 
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

        //if enough water subtract waterpoints
        if (!HasTooLittleWater && _gameManager.IsNewTurn)
        {
            SubtractWaterPoints();
            NotEnoughWater.enabled = false;
            
        }
        if (IsGrowing)
        {
            _growButtonImage.color = Color.green;
        }

        //wincondition
        if (_acquiredSunlightPoints >= _gameManager.WinCondition)
        {
            HasWon = true;
        }
        if (WaterPoints<0)
        {
            WaterPoints = 0;
        }
        //Update UI points
        
        if (_gameManager.IsNewTurn)
        {
            WriteGainedPointsToUI();
        }

        _acquiredSunlightPoints = 0;
        AcquiredWaterPoints = 0;
        SubtractedWaterPoints = 0;

        SunPointsText.text = SunLightPoints.ToString();
        WaterPointsText.text = WaterPoints.ToString();
        LeafCountText.text = LeafCount.ToString();
        RootCountText.text = RootCount.ToString();
        ShadowLeavesCountText.text=ShadowLeafCount.ToString();
    }

    private void WriteGainedPointsToUI()
    {
        _feedbackWriter.IsVisualizing = true;
        _feedbackWriter.SunlightPoints = _acquiredSunlightPoints;
        _feedbackWriter.SunlightSign = "+";

        _feedbackWriter.WaterPoints = SubtractedWaterPoints;
        _feedbackWriter.WaterSign = "";

        _feedbackWriter.LeafsAdded = "0";
        _feedbackWriter.LeafSign = "+";
    }

    private void CalculateLeafRootCounts()
    {
        ShadowLeafCount = 0;
        LeafCount = 0;
        RootCount = 0;
        
        foreach(var root in MyRoots)
        {
            RootCount++;
        }

        foreach (var leaf in MyLeaves)
        {
            if (leaf.GetComponent<Leaves>().IsInShadowed)
            {
                ShadowLeafCount++;
            }
            else
            {
                LeafCount++;
            }
        }
    }

    private void SubtractWaterPoints()
    {
        foreach (var leaf in _gameManager.ActivePlayerScript.MyLeaves)
        {
            Leaves leafScript = leaf.GetComponent<Leaves>();
            WaterPoints -= 1 * leafScript.ShadowMultiplier;
            SubtractedWaterPoints -= 1 * leafScript.ShadowMultiplier;
        }
    }

    private bool CalculateHasTooLittleWater()
    {
        int leafCount = 0;
        foreach (var leaf in _gameManager.ActivePlayerScript.MyLeaves)
        {
            Leaves leafScript = leaf.GetComponent<Leaves>();

            leafCount += 1 * leafScript.ShadowMultiplier;
        }
        if (leafCount > WaterPoints)
        {
            return true;
        }
        if (leafCount < WaterPoints)
        {
            return false;
        }
        return false;

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
