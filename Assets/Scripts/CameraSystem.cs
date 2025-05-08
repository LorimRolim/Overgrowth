using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    private Vector3 _moveDir;
    [SerializeField] private float _moveSpeed = 50f;
    [SerializeField] private float _rotSpeed = 50f;
    
    private bool _isMouseRotating;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveWithKey();
        RotateWithKey();
        RotateWithMouse();
        MoveUpAndDown();

    }

    private void MoveUpAndDown()
    {
        float flyDir = 0f;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            flyDir = -1f;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            flyDir = +1f;
        }
        transform.position += _moveSpeed * Time.deltaTime * flyDir*Vector3.up;
    }

    private void RotateWithMouse()
    {
        if (Input.GetMouseButtonDown(2))
        {
            _isMouseRotating = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            _isMouseRotating = false;
        }
        if(_isMouseRotating)
        {
            float verticalRotate = Input.GetAxis("Mouse Y") * _rotSpeed * Time.deltaTime;
            float horizontalRotate = Input.GetAxis("Mouse X") * _rotSpeed * Time.deltaTime;

            transform.Rotate(Vector3.right, -verticalRotate);
            transform.Rotate(Vector3.up, horizontalRotate,Space.World);
        }
        
    }

    private void MoveWithKey()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        _moveDir = (transform.forward * vertical) + (transform.right * horizontal);
        Vector3 realMoveDir= Vector3.ProjectOnPlane(_moveDir, Vector3.up);
        transform.position += _moveSpeed * Time.deltaTime * realMoveDir;
    }

    private void RotateWithKey()
    {
        float rotateDir = 0f;
        if (Input.GetKey(KeyCode.Q))
        {
            rotateDir = +1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotateDir = -1f;
        }
        transform.eulerAngles += new Vector3(0, rotateDir * _rotSpeed * Time.deltaTime, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, _moveDir);
    }
}
