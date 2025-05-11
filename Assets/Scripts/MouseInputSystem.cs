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
    [SerializeField] private GameManager _gameManager;
    
    
    
    private bool _hasClicked=false;
    
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
            if ((hit.collider.gameObject.layer == 3) && _hasClicked && _gameManager.ActivePlayerScript.IsChopping)
            {
                //destroy the cube that was hit

                GameObject hitCube = hit.collider.gameObject;
                _hasClicked = false;
                
                _gameManager.InActiveGameObjects.Add(hitCube);
                _gameManager.ActivePlayerScript.MyLeaves.Remove(hitCube);
            }
            if (_gameManager.ActivePlayerScript.IsGrowing)
            {
                //grow a leaf or root
                if ((hit.collider.gameObject.layer == 6) && _hasClicked)
                {
                    EraseListElements();

                    
                    GameObject hitCube = hit.collider.gameObject;
                    if (hitCube.transform.position.y - 0.5f < 0)
                    {
                        GameObject spawnedObject = FillOpenSpot(hitCube.transform.position, Vector3.zero, _gameManager.ActivePlayerScript.TreeRoot, _gameManager.ActivePlayerScript.MyRoots);
                    }
                    if (hitCube.transform.position.y - 0.5f >= 0)
                    {
                        GameObject spawnedObject = FillOpenSpot(hitCube.transform.position, Vector3.zero, _gameManager.ActivePlayerScript.TreeLeaf, _gameManager.ActivePlayerScript.MyLeaves);
                    }

                    //Destroy(hitCube);
                    _hasClicked = false;
                }
                //check potential
                if ((hit.collider.gameObject.layer == 3 || hit.collider.gameObject.layer == 7) && _hasClicked)
                {
                    _hitCube = hit.collider.gameObject;
                    CheckOpenNeighbouringSpots(_hitCube, _potentialGrowCube);
                    _hasClicked = false;
                }
            }
            
        }
        
    }


    private void EraseListElements()
    {
        for (int i = _gameManager.ActivePlayerScript.MyPotentialGrowCubes.Count - 1; i >= 0; i--)
        {
            Destroy(_gameManager.ActivePlayerScript.MyPotentialGrowCubes[i]);
            _gameManager.ActivePlayerScript.MyPotentialGrowCubes.RemoveAt(i);
        }
    }

    private void CheckOpenNeighbouringSpots(GameObject hitCube,GameObject spawnObject)
    {
        RaycastHit hit;
        if (!Physics.Raycast(hitCube.transform.position, Vector3.up,out hit ,1f)) //up
        {
            if (hit.collider == null)
            {
                FillOpenSpot(hitCube.transform.position,Vector3.up,spawnObject, _gameManager.ActivePlayerScript.MyPotentialGrowCubes);
            }
        }
        if (!Physics.Raycast(hitCube.transform.position, Vector3.right, out hit, 1f)) //right
        {
            if (hit.collider == null)
            {
                FillOpenSpot(hitCube.transform.position, Vector3.right, spawnObject, _gameManager.ActivePlayerScript.MyPotentialGrowCubes);
            }
        }
        if (!Physics.Raycast(hitCube.transform.position, -Vector3.right, out hit, 1f)) //left
        {
            if (hit.collider == null)
            {
                FillOpenSpot(hitCube.transform.position, -Vector3.right, spawnObject, _gameManager.ActivePlayerScript.MyPotentialGrowCubes);
            }
        }
        if (!Physics.Raycast(hitCube.transform.position, -Vector3.up, out hit, 1f)) //down
        {
            if (hit.collider == null)
            {
                FillOpenSpot(hitCube.transform.position, -Vector3.up, spawnObject, _gameManager.ActivePlayerScript.MyPotentialGrowCubes);
            }
        }
        if (!Physics.Raycast(hitCube.transform.position, Vector3.forward, out hit, 1f)) //forward
        {
            if (hit.collider == null)
            {
                FillOpenSpot(hitCube.transform.position, Vector3.forward, spawnObject, _gameManager.ActivePlayerScript.MyPotentialGrowCubes);
            }
        }
        if (!Physics.Raycast(hitCube.transform.position, -Vector3.forward, out hit, 1f)) //backward
        {
            if (hit.collider == null)
            {
                FillOpenSpot(hitCube.transform.position, -Vector3.forward, spawnObject, _gameManager.ActivePlayerScript.MyPotentialGrowCubes);
            }
        }
    }

    private GameObject FillOpenSpot(Vector3 position, Vector3 direction,GameObject spawnObject, List<GameObject> targetList)
    {
        //add the new cube to the players list of leaves
        Vector3 spawnPosition = position + direction * 1f;

        if (spawnObject.tag == "Leaf"||spawnObject.tag=="Root")
        {
            GameObject spawnedObject = Instantiate(spawnObject, spawnPosition, Quaternion.identity);
            Leaves spawnedObjectScript = spawnedObject.GetComponent<Leaves>();
            if (IsOutsideBoard(spawnPosition))
            {
                _gameManager.InActiveGameObjects.Add(spawnedObject);
            }
            else
            {
                targetList.Add(spawnedObject);
                _gameManager.ActivePlayerScript.SunLightPoints -= 1;
            }
            return spawnedObject;
        }
        if (spawnObject.tag == "PotentialCube")
        {

            GameObject spawnedObject = Instantiate(spawnObject, spawnPosition, Quaternion.identity);
            targetList.Add(spawnedObject);
            return spawnedObject;
        }
        else
        {
            return spawnObject;
        }
    }

    private bool IsOutsideBoard(Vector3 spawnPosition)
    {
        if((spawnPosition.x > _gameManager.BoardBorder) || (spawnPosition.y > _gameManager.BoardBorder) || (spawnPosition.z > _gameManager.BoardBorder))
        {
            return true;
        }
        if(((spawnPosition.x < -_gameManager.BoardBorder) || (spawnPosition.y < -_gameManager.BoardBorder) || (spawnPosition.z < -_gameManager.BoardBorder)))
        {
            return true;
        }
        return false;
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
