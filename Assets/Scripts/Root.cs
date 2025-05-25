using UnityEngine;

public class Root : MonoBehaviour
{
    [SerializeField] private GameManager _gameManagerScript;
    [SerializeField] private GameObject _gameManager;
    public bool CalculatedWaterPoints = false;
    public bool IsDead;


    // Update is called once per frame
    void Update()
    {
        if (IsDead)
        {
            _gameManager = GameObject.Find("GameManager");
            _gameManagerScript = _gameManager.GetComponent<GameManager>();

            for (int i = _gameManagerScript.ActivePlayerScript.MyRoots.Count - 1; i >= 0; i--)
            {
                if (_gameManagerScript.ActivePlayerScript.MyRoots[i] == this)
                {
                    Destroy(this);
                    //_gameManagerScript.InActiveGameObjects.Add(_gameManagerScript.ActivePlayerScript.MyRoots[i]);
                    _gameManagerScript.ActivePlayerScript.MyRoots.RemoveAt(i);
                }

            }
        }
    }

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
            _gameManagerScript.ActivePlayerScript.AcquiredWaterPoints += 2;
            CalculatedWaterPoints = true;
            _gameManagerScript.WaterLevel -= _gameManagerScript.WaterDepletingAmount;
        }
    }
    

}
