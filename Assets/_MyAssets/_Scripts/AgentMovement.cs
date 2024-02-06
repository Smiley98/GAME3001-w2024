using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AgentMovement : MonoBehaviour
{
    // Planet is the obstacle, target is our cursor
    Vector3 target = Vector3.zero;
    public Transform obstacle;
    public float moveSpeed;
    Rigidbody2D rb;

    public float raySpread;

    void Start()
    {
        Vector2 A = new Vector2(2.0f, 3.0f);    // Start position
        Vector2 B = new Vector2(10.0f, 15.0f);  // End position
        Vector2 AB = B - A;                     // Line from start to end
        Vector2 N1 = AB.normalized;
        Vector2 N2 = AB / Mathf.Sqrt(AB.x * AB.x + AB.y * AB.y);
        float length1 = N1.magnitude;
        float length2 = N2.magnitude;
        Debug.Log(N1);  // Automatic normalization
        Debug.Log(N2);  // Manual normalization
        Debug.Log(length1);
        Debug.Log(length2);

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Change this to drag our planet around
        if (Input.GetMouseButton(0))
        {
            // Convert mouse position to world position.
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0.0f; // Ensure the Z-coordinate is correct for a 2D game  .     
        }

        // Direction FROM ship TO target
        Vector2 currentVelocity = rb.velocity;
        Vector2 desiredVelocity = (target - transform.position).normalized * moveSpeed;

        // transform.right is the ship's direction
        Vector3 leftDirection = Quaternion.Euler(0.0f, 0.0f, raySpread) * transform.right;
        Vector3 rightDirection = Quaternion.Euler(0.0f, 0.0f, -raySpread) * transform.right;

        RaycastHit2D leftHit = Physics2D.Raycast(transform.position + leftDirection, leftDirection);
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position + rightDirection, rightDirection);

        if (leftHit.collider != null)
        {
            // Turn right (transform.up * -1) to avoid left obstacle
            desiredVelocity += new Vector2(transform.up.x, transform.up.y) * moveSpeed * -1.0f;
            //Debug.Log(leftHit.collider.gameObject.name);
        }
        else if (rightHit.collider != null)
        {
            // Turn left (transform.up) to avoid right obstacle
            desiredVelocity += new Vector2(transform.up.x, transform.up.y) * moveSpeed;
            //Debug.Log(rightHit.collider.gameObject.name);
        }

        Debug.DrawLine(transform.position, transform.position + leftDirection * 20.0f);
        Debug.DrawLine(transform.position, transform.position + rightDirection * 20.0f);

        // Prolong our rotation until we've applied avoidance force!
        LookAt2D(target);
        rb.AddForce(desiredVelocity - currentVelocity);
    }

    void LookAt2D(Vector3 target)
    {
        Vector3 lookDirection = target - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }
}
