using System;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class MouseInputSystem : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private Ray _mouseClickRay;
    [SerializeField] private GameObject _hitCube;
    [SerializeField] private GameObject _treeLeaves;
    [SerializeField] private GameObject _potentialGrowCube;
    [SerializeField] private PlayerOneScript _playerScript;
    private bool _hasClicked=false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _hasClicked = true;
            _mouseClickRay = _cam.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(_mouseClickRay.origin, _mouseClickRay.direction * 10, Color.yellow);
            CheckRayCollisions();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            EraseListElements();
        }
    }

    private void CheckRayCollisions()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(_mouseClickRay, out hit, 50f))
        {
            //chopping
            if ((hit.collider.gameObject.layer == 3) && _hasClicked && _playerScript.IsChopping)
            {
                //destroy the cube that was hit

                GameObject hitCube = hit.collider.gameObject;
                _hasClicked = false;
                //
                Destroy(hitCube);
            }
        }
        if (Physics.Raycast(_mouseClickRay, out hit, 50f))
        {
            //grow a leaf
            if ((hit.collider.gameObject.layer == 6) && _hasClicked)
            {
                EraseListElements();

                _playerScript.SunLightPoints -= 1;
                GameObject hitCube = hit.collider.gameObject;
                FillOpenSpot(hitCube.transform.position, Vector3.zero, _treeLeaves, _playerScript.MyLeaves);
                Destroy(hitCube);
                _hasClicked = false;
            }
        }
        if (Physics.Raycast(_mouseClickRay, out hit, 50f))
        {
            //check potential
            if ((hit.collider.gameObject.layer == 3) && _hasClicked)
            {
                _hitCube = hit.collider.gameObject;
                CheckOpenNeighbouringSpots(_hitCube, _potentialGrowCube);
                _hasClicked = false;
            }
        }
        ////chopping
        //if ((hit.collider.gameObject.layer == 3) && _hasClicked && _playerScript.IsChopping)
        //{
        //    //destroy the cube that was hit

        //    GameObject hitCube = hit.collider.gameObject;
        //    _hasClicked = false;
        //    //
        //    Destroy(hitCube);
        //}

        ////grow a leaf
        //if ((hit.collider.gameObject.layer == 6) && _hasClicked)
        //{
        //    EraseListElements();

        //    _playerScript.SunLightPoints -= 1;
        //    GameObject hitCube = hit.collider.gameObject;
        //    FillOpenSpot(hitCube.transform.position, Vector3.zero, _treeLeaves, _playerScript.MyLeaves);
        //    Destroy(hitCube);
        //    _hasClicked = false;
        //}

        ////check potential
        //if ((hit.collider.gameObject.layer == 3) && _hasClicked)
        //{
        //    _hitCube = hit.collider.gameObject;
        //    CheckOpenNeighbouringSpots(_hitCube, _potentialGrowCube);
        //    _hasClicked = false;

        //}
    }

    private void EraseListElements()
    {
        for (int i = _playerScript.MyPotentialGrowCubes.Count - 1; i >= 0; i--)
        {
            Destroy(_playerScript.MyPotentialGrowCubes[i]);
            _playerScript.MyPotentialGrowCubes.RemoveAt(i);
        }
    }

    private void CheckOpenNeighbouringSpots(GameObject hitCube,GameObject spawnObject)
    {
        RaycastHit hit;
        if (!Physics.Raycast(hitCube.transform.position, Vector3.up,out hit ,1f)) //up
        {
            if (hit.collider == null)
            {
                FillOpenSpot(hitCube.transform.position,Vector3.up,spawnObject, _playerScript.MyPotentialGrowCubes);
            }
        }
        if (!Physics.Raycast(hitCube.transform.position, Vector3.right, out hit, 1f)) //right
        {
            if (hit.collider == null)
            {
                FillOpenSpot(hitCube.transform.position, Vector3.right, spawnObject, _playerScript.MyPotentialGrowCubes);
            }
        }
        if (!Physics.Raycast(hitCube.transform.position, -Vector3.right, out hit, 1f)) //left
        {
            if (hit.collider == null)
            {
                FillOpenSpot(hitCube.transform.position, -Vector3.right, spawnObject, _playerScript.MyPotentialGrowCubes);
            }
        }
        if (!Physics.Raycast(hitCube.transform.position, -Vector3.up, out hit, 1f)) //down
        {
            if (hit.collider == null)
            {
                FillOpenSpot(hitCube.transform.position, -Vector3.up, spawnObject, _playerScript.MyPotentialGrowCubes);
            }
        }
        if (!Physics.Raycast(hitCube.transform.position, Vector3.forward, out hit, 1f)) //forward
        {
            if (hit.collider == null)
            {
                FillOpenSpot(hitCube.transform.position, Vector3.forward, spawnObject, _playerScript.MyPotentialGrowCubes);
            }
        }
        if (!Physics.Raycast(hitCube.transform.position, -Vector3.forward, out hit, 1f)) //backward
        {
            if (hit.collider == null)
            {
                FillOpenSpot(hitCube.transform.position, -Vector3.forward, spawnObject, _playerScript.MyPotentialGrowCubes);
            }
        }
    }

    private void FillOpenSpot(Vector3 position, Vector3 direction,GameObject spawnObject, List<GameObject> targetList)
    {
        //add the new cube to the players list of leaves
        Vector3 spawnPosition = position + direction * 1f;
        GameObject spawnedObject=Instantiate(spawnObject,spawnPosition,Quaternion.identity);
        targetList.Add(spawnedObject);

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(new Ray(_hitCube.transform.position, Vector3.forward));
        Gizmos.DrawRay(new Ray(_hitCube.transform.position, -Vector3.forward));

        Gizmos.DrawRay(new Ray(_hitCube.transform.position, -Vector3.up));
        Gizmos.DrawRay(new Ray(_hitCube.transform.position, Vector3.up));

        
        Gizmos.DrawRay(new Ray(_hitCube.transform.position, -Vector3.right));
        Gizmos.DrawRay(new Ray(_hitCube.transform.position, Vector3.right));
        
    }
}
