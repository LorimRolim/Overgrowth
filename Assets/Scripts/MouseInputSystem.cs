
using System.Collections.Generic;


using UnityEngine;


public class MouseInputSystem : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private Ray _mouseClickRay;
    [SerializeField] private GameObject _hitCube;
    [SerializeField] private GameObject _treeLeaves;
    [SerializeField] private GameObject _potentialGrowCube;
    [SerializeField] private GameManager _gameManager;
    
    [SerializeField] private WriteFeedbackOnScreen _feedbackWriter;

    private bool _hasClicked=false;
    
    // Update is called once per frame
    void Update()
    {
        _feedbackWriter = GetComponent<WriteFeedbackOnScreen>();
        
        if (Input.GetMouseButtonDown(0))
        {
            _hasClicked = true;
            _mouseClickRay = _cam.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(_mouseClickRay.origin, _mouseClickRay.direction * 10, Color.yellow);
            CheckRayCollisions();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ErasePotentialCubes();
        }
    }

    private void CheckRayCollisions()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(_mouseClickRay, out hit, 50f))
        {
            //chopping
            if (_hasClicked && _gameManager.ActivePlayerScript.IsChopping && 
                (hit.collider.gameObject.layer==_gameManager.ActivePlayerScript.gameObject.layer)) //(hit.collider.gameObject.layer == 3)
            {
                //destroy the cube that was hit
                GameObject hitCube = hit.collider.gameObject;
                _hasClicked = false;
                
                _gameManager.InActiveGameObjects.Add(hitCube);
                _gameManager.ActivePlayerScript.MyLeaves.Remove(hitCube);
            }

            //growing
            if (_gameManager.ActivePlayerScript.IsGrowing)
            {
                //grow a leaf or root
                if ((hit.collider.gameObject.layer == 6) && _hasClicked)
                {
                    
                    ErasePotentialCubes();

                    GameObject hitCube = hit.collider.gameObject;
                    if (hitCube.transform.position.y - 0.5f < 0)
                    {
                        GameObject spawnedObject = FillOpenSpot(hitCube.transform.position, Vector3.zero, _gameManager.ActivePlayerScript.TreeRoot, _gameManager.ActivePlayerScript.MyRoots);
                        
                    }
                    if (hitCube.transform.position.y - 0.5f >= 0)
                    {
                        PotentialCubeScript hitCubeScript=hitCube.GetComponent<PotentialCubeScript>();
                        if (hitCubeScript.IsTouchingLeaf)
                        {
                            _gameManager.ActivePlayerScript.WaterPoints -= 1;
                            Destroy(hitCubeScript.LeafTouching);
                        }
                        GameObject spawnedObject = FillOpenSpot(hitCube.transform.position, Vector3.zero, _gameManager.ActivePlayerScript.TreeLeaf, _gameManager.ActivePlayerScript.MyLeaves);


                        _feedbackWriter.IsVisualizing = true;
                        _feedbackWriter.SunlightPoints = 1;
                        _feedbackWriter.SunlightSign = "-";

                        _feedbackWriter.WaterPoints = 0;
                        _feedbackWriter.WaterSign = "+";

                        _feedbackWriter.LeafsAdded = "1";
                        _feedbackWriter.LeafSign = "+";

                    }
                    //Destroy(hitCube);
                    _hasClicked = false;
                }
                //check potential
                if ((hit.collider.gameObject.layer == _gameManager.ActivePlayerScript.gameObject.layer) && _hasClicked)//(hit.collider.gameObject.layer == 3 || hit.collider.gameObject.layer == 7)
                {
                    _hitCube = hit.collider.gameObject;
                    CheckOpenNeighbouringSpots(_hitCube, _potentialGrowCube);
                    _hasClicked = false;
                }
            }
        }
    }

   
    public void ErasePotentialCubes()
    {
        for (int i = _gameManager.ActivePlayerScript.MyPotentialGrowCubes.Count - 1; i >= 0; i--)
        {
            Destroy(_gameManager.ActivePlayerScript.MyPotentialGrowCubes[i]);
            _gameManager.ActivePlayerScript.MyPotentialGrowCubes.RemoveAt(i);
        }
    }

    private void CheckOpenNeighbouringSpots(GameObject hitCube,GameObject spawnObject)
    {
        

        //checkone collisions
        CheckWhoWeCollidingWith(hitCube,spawnObject,Vector3.up);
        CheckWhoWeCollidingWith(hitCube, spawnObject,Vector3.down);
        CheckWhoWeCollidingWith(hitCube,spawnObject,Vector3.right);
        CheckWhoWeCollidingWith(hitCube,spawnObject,Vector3.left);
        CheckWhoWeCollidingWith(hitCube, spawnObject, Vector3.forward);
        CheckWhoWeCollidingWith(hitCube, spawnObject, Vector3.back);

    }

    private void CheckWhoWeCollidingWith(GameObject hitCube, GameObject spawnObject, Vector3 up)
    {
        RaycastHit hit;
        if (!Physics.Raycast(hitCube.transform.position, up, out hit, 1f)) //up if not
        {
            if (hit.collider == null)
            {
                FillOpenSpot(hitCube.transform.position, up, spawnObject, _gameManager.ActivePlayerScript.MyPotentialGrowCubes);
            }
        }
        if (Physics.Raycast(hitCube.transform.position, up, out hit, 1f)
            && _gameManager.ActivePlayerScript.HasAlgea)
        {
            if (hit.collider.tag == "Leaf")
            {
                if (_gameManager.ActivePlayerScript.HasAlgea)
                {
                    GameObject spawnedObject=FillOpenSpot(hitCube.transform.position, up, spawnObject, _gameManager.ActivePlayerScript.MyPotentialGrowCubes);
                    spawnedObject.GetComponent<PotentialCubeScript>().IsTouchingLeaf = true;
                    spawnedObject.GetComponent<PotentialCubeScript>().LeafTouching = hit.collider.gameObject;
                }
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
            spawnedObject.layer = _gameManager.ActivePlayerScript.gameObject.layer;
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
        if((spawnPosition.x > _gameManager.BoardBorder)  || (spawnPosition.z > _gameManager.BoardBorder))// || (spawnPosition.y > _gameManager.BoardHeight)
        {
            return true;
        }
        if(((spawnPosition.x < -_gameManager.BoardBorder) || (spawnPosition.y < -_gameManager.BoardHeight) || (spawnPosition.z < -_gameManager.BoardBorder)))
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
