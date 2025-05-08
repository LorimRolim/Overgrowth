using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneScript : MonoBehaviour
{
    // have all variables 
    public List<GameObject> MyLeaves = new List<GameObject>();
    public List<GameObject> MyPotentialGrowCubes= new List<GameObject>();

    public bool IsGrowing=true;
    public bool IsChopping=false;

    public int SunLightPoints;
    public int WaterPoints;

    public Vector3 PlayerStartPosition;

    [SerializeField] private GameObject _treeLeaves;

    [SerializeField] private GameObject[] targetObjects; // Drag multiple objects (e.g., cubes) into this array in the Inspector
    [SerializeField] private float raycastDistance = 10f; // Distance the raycast will travel


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerStartPosition = transform.position;
        //spawn 1 leaf + 1 root
        Instantiate(_treeLeaves, PlayerStartPosition,Quaternion.identity); 
        Instantiate(_treeLeaves,PlayerStartPosition-Vector3.up,Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
            foreach (GameObject targetObject in MyLeaves)
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
                SunLightPoints += 1;

                    }
                    
                
                
            }
        
        
    }
}
