
using UnityEngine;

public class AlgeaScript : MonoBehaviour
{
    private GameObject _gameManager;
    private GameManager _gameManagerScript;

    void OnTriggerStay(Collider other)
    {
        _gameManager = GameObject.Find("GameManager");
        _gameManagerScript = _gameManager.GetComponent<GameManager>();
        
        _gameManagerScript.ActivePlayerScript.HasAlgea = true;
        Destroy(this.gameObject);
    }
    
}
