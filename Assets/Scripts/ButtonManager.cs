using UnityEngine;


public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    private PlayerOneScript _playerScript;

    

    // Update is called once per frame
    
    public void ChopClick()
    {
        _gameManager.ActivePlayerScript.IsChopping = true;
        _gameManager.ActivePlayerScript.IsGrowing = false;
    }
    public void GrowClick()
    {
        _gameManager.ActivePlayerScript.IsGrowing = true;
        _gameManager.ActivePlayerScript.IsChopping=false;
    }
}
