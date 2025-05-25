using UnityEngine;
using UnityEngine.UI;

public class WriteFeedbackOnScreen : MonoBehaviour
{
    public Text SunlightAdder;
    public Text WaterPointsAdder;
    public Text LeafAdder;

    public int SunlightPoints;
    public int WaterPoints;
    public string LeafsAdded;

    public float VisualizationTime = 3f;
    public float VisualizationTimer = 0f;
    public bool IsVisualizing;
    public Text TextPlace;
    public string SunlightSign;
    public string WaterSign;
    public string LeafSign;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LeafAdder.enabled = false;
        WaterPointsAdder.enabled = false;
        SunlightAdder.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (IsVisualizing)
        {
            VisualizationTimer += Time.deltaTime;
        }
        WriteToScreen(SunlightPoints, WaterPoints, LeafsAdded);
    }
    public void WriteToScreen(int SunlightPoints,int WaterPoints,string LeafsAdded)
    {
        if (VisualizationTimer <= VisualizationTime && IsVisualizing)
        {
            
            SunlightAdder.enabled = true;
            WaterPointsAdder.enabled = true;
            LeafAdder.enabled = true;
            
            SunlightAdder.text = SunlightSign+SunlightPoints.ToString();
            WaterPointsAdder.text= WaterSign+WaterPoints.ToString();
            LeafAdder.text = LeafSign + LeafsAdded;
        }
        if (VisualizationTimer > VisualizationTime)
        {
            VisualizationTimer = 0;

            SunlightAdder.enabled = false;
            WaterPointsAdder.enabled = false;
            LeafAdder.enabled = false;
            IsVisualizing = false;
        }
    }
}
