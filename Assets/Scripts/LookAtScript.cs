using UnityEngine;

public class LookAtScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MouseLookX();
    }
    private void MouseLookX()
    {
        // Get the mouse delta. This is not in the range -1...1
        float horizontalMouse = Input.GetAxis("Mouse X");
        transform.Rotate(0, horizontalMouse, 0);

    }
}
