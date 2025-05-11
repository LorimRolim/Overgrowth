using UnityEngine;

public class Root : MonoBehaviour
{
    [SerializeField] private GameManager _gameManagerScript;
    [SerializeField] private GameObject _gameManager;
    public bool CalculatedWaterPoints=false;


    // Update is called once per frame
    
    public void GiveWaterpoints()
    {
        _gameManager = GameObject.Find("GameManager");
        _gameManagerScript = _gameManager.GetComponent<GameManager>();
        if (_gameManagerScript.IsNewTurn)
        {
            CalculatedWaterPoints = false;
        }

        if (((transform.position.y - 0.5f) <= _gameManagerScript.WaterLevel) && !CalculatedWaterPoints)
        {
            _gameManagerScript.ActivePlayerScript.WaterPoints += 2;
            CalculatedWaterPoints = true;
            _gameManagerScript.WaterLevel -= _gameManagerScript.WaterDepletingAmount;
        }
    }
    
}
