using UnityEngine;

public class Leaves : MonoBehaviour
{
    public bool IsInShadowed;
    [SerializeField] private GameManager _gameManagerScript;
    [SerializeField] private GameObject _gameManager;
    [SerializeField] private float raycastDistance=10f ;
    
    public bool IsOutsideBoard;

    public int ShadowMultiplier = 1;

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
