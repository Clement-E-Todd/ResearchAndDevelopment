using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 45f;

    void Update()
    {
        // Position
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += transform.forward * Time.deltaTime * moveSpeed;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position -= transform.forward * Time.deltaTime * moveSpeed;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += transform.right * Time.deltaTime * moveSpeed;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position -= transform.right * Time.deltaTime * moveSpeed;
        }

        // Rotation
        if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(new Vector3(-1, 0, 0) * Time.deltaTime * turnSpeed);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(new Vector3(1, 0, 0) * Time.deltaTime * turnSpeed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * turnSpeed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * turnSpeed);
        }
    }
}
