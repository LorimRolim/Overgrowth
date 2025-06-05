using Unity.Mathematics;
using UnityEngine;

public class YrotatiionScript : MonoBehaviour
{
    

    [SerializeField] private float _xMin;
    [SerializeField] private float _xMax;
    [SerializeField] private float _rotSpeed;
    private bool _isMouseRotating;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            _isMouseRotating = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            _isMouseRotating = false;
        }
        if (_isMouseRotating)
        {
            float verticalMouse = Input.GetAxis("Mouse Y") * _rotSpeed * Time.deltaTime;
            transform.Rotate(verticalMouse, 0, 0);
            
        }
        

        

        ClampCameraX();
    }

    private void ClampCameraX()
    {
        float yClamp = math.clamp(transform.rotation.eulerAngles.x, _xMin, _xMax);
        transform.rotation = Quaternion.Euler(yClamp, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
