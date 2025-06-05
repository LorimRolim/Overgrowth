using System;
using UnityEngine;

public class Leaves : MonoBehaviour
{
    public bool IsInShadowed;
    [SerializeField] private GameManager _gameManagerScript;
    [SerializeField] private GameObject _gameManager;
    [SerializeField] private float raycastDistance=10f ;
    [SerializeField] private GameObject _sunPointParticle;
    
    public bool IsOutsideBoard;

    public int ShadowMultiplier = 1;
    public bool IsDead;


    void Update()
    {
        _gameManager = GameObject.Find("GameManager");
        _gameManagerScript = _gameManager.GetComponent<GameManager>();
        MakeCollider1();
        if (IsDead)
        {
            for (int i = _gameManagerScript.ActivePlayerScript.MyLeaves.Count - 1; i >= 0; i--)
            {
                if (_gameManagerScript.ActivePlayerScript.MyLeaves[i] == this)
                {
                    Destroy(this);
                    //_gameManagerScript.InActiveGameObjects.Add(_gameManagerScript.ActivePlayerScript.MyRoots[i]);
                    _gameManagerScript.ActivePlayerScript.MyRoots.RemoveAt(i);
                }

            }
        }
        if (_gameManagerScript.IsNewTurn)
        {
            Instantiate(_sunPointParticle);
        }


        if (IsInShadowed)
        {
            transform.localScale=new Vector3(0.5f,1f,0.5f);
        }
        if (!IsInShadowed)
        {
            transform.localScale =Vector3.one;
        }
    }

    private void MakeCollider1()
    {
        BoxCollider boxCollider= this.GetComponent<BoxCollider>();
        boxCollider.size= Vector3.one;
    }

    public void CheckIfInShadow()
    {

        // Perform a raycast upwards
        Ray ray = new Ray(transform.position, Vector3.up);
        if(Physics.Raycast(ray, raycastDistance))
        {
            IsInShadowed = true;
            ShadowMultiplier = 2;
        }
        else
        {
            IsInShadowed=false;
            ShadowMultiplier = 1;
        }
            
        // Debug visualization
        Debug.DrawRay(transform.position, Vector3.up * raycastDistance, IsInShadowed ? Color.red : Color.green);

    }
    

}
