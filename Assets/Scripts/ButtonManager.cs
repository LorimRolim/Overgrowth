using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    private PlayerOneScript _playerScript;

    //button texts
    //public TextField _sunLightAmount;
    //[SerializeField] private Text _waterAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _playerScript = _gameManager.ActivePlayerScript;
        
    }
    public void ChopClick()
    {
        _playerScript.IsChopping = true;
        _playerScript.IsGrowing = false;
    }
    public void GrowClick()
    {
        _playerScript.IsGrowing = true;
        _playerScript.IsChopping=false;
    }
}
