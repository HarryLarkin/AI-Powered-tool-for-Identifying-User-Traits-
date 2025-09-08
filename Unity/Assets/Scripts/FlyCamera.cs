using UnityEngine;

// Right mouse to look, WASD/E/Q to move
public class FlyCamera : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    public float lookSpeed = 2.0f;

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            float mx = Input.GetAxis("Mouse X") * lookSpeed;
            float my = -Input.GetAxis("Mouse Y") * lookSpeed;
            transform.Rotate(Vector3.up, mx, Space.World);
            transform.Rotate(Vector3.right, my, Space.Self);
        }

        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) dir += transform.forward;
        if (Input.GetKey(KeyCode.S)) dir -= transform.forward;
        if (Input.GetKey(KeyCode.A)) dir -= transform.right;
        if (Input.GetKey(KeyCode.D)) dir += transform.right;
        if (Input.GetKey(KeyCode.E)) dir += transform.up;
        if (Input.GetKey(KeyCode.Q)) dir -= transform.up;

        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
