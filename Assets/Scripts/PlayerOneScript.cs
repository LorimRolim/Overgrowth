using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneScript : MonoBehaviour
{
    // have all variables 
    public List<GameObject> MyLeaves = new List<GameObject>();
    public List<GameObject> MyPotentialGrowCubes= new List<GameObject>();
    public List<GameObject> MyRoots = new List<GameObject>();
    
    public bool IsGrowing=true;
    public bool IsChopping=false;

    public int SunLightPoints;
    public int WaterPoints;

    public Vector3 PlayerStartPosition;

    [SerializeField] private GameObject _treeLeaves;
    [SerializeField] private GameObject _treeRoots;

    [SerializeField] private MeshRenderer _leafRenderer;
    [SerializeField] private MeshRenderer _rootRenderer;

    [SerializeField] private Material _leafMat;
    [SerializeField] private Material _rootMat;

    [SerializeField] private float raycastDistance = 10f; // Distance the raycast will travel

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetMyMaterial();
        PlayerStartPosition = transform.position;
        //spawn 1 leaf + 1 root
        
        Instantiate(_treeLeaves, PlayerStartPosition,Quaternion.identity); 
        Instantiate(_treeLeaves,PlayerStartPosition-Vector3.up,Quaternion.identity);
    }

    private void SetMyMaterial()
    {
        _leafRenderer.material = _leafMat;
        _rootRenderer.material = _rootMat;
    }

    // Update is called once per frame
    void Update()
    {
        
        
        
        
    }
}
