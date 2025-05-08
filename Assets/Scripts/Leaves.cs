using UnityEngine;

public class Leaves : MonoBehaviour
{
    public bool IsInShadowed;
    [SerializeField] private GameManager _gameManagerScript;
    [SerializeField] private float raycastDistance=10f ;

    public void CheckIfInShadow()
    {
        foreach (GameObject targetObject in _gameManagerScript.ActivePlayerScript.MyLeaves)
        {
            // Get the position of the target object
            Vector3 objectPosition = targetObject.transform.position;

            // Perform a raycast upwards
            Ray ray = new Ray(objectPosition, Vector3.up);
            bool isShadowed = Physics.Raycast(ray, raycastDistance);

            // Debug visualization
            Debug.DrawRay(objectPosition, Vector3.up * raycastDistance, isShadowed ? Color.red : Color.green);

            // Log the result
            if (isShadowed)
            {
                _gameManagerScript.ActivePlayerScript.SunLightPoints += 1;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
