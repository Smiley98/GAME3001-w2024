using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AgentMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform island;
    [SerializeField] private float loadDistance;
    private Vector3 targetPosition = Vector3.zero;

    void Update()
    {
        
        // Check for mouse input.
        if (Input.GetMouseButton(0))
        {
            Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            // Convert mouse position to world position.
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0f; // Ensure the Z-coordinate is correct for a 2D game  .     
        }

        // Test this out by modifying the z-rotation when in play mode!
        //Vector3 direction = Vector3.right;        // "Global x-axis"
        //Vector3 direction = transform.right;      // "Local x-axis"
        //transform.position += direction * moveSpeed * Time.deltaTime;

        // Move towards the target position.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Rotate to look at the target position.
        LookAt2D(targetPosition);

        // Check island distance.
        if (Vector3.Distance(transform.position, island.position) < loadDistance)
        {
            Debug.Log("Loading end scene...");
            SceneManager.LoadScene(2);
        }
    }

    void LookAt2D(Vector3 target)
    {
        // Vector FROM player TO target (AB = B - A)
        Vector3 lookDirection = target - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        // *NEVER use atan(y / x). This will only give you a value between -90 and 90*
        //float angle = Mathf.Atan(lookDirection.y / lookDirection.x) * Mathf.Rad2Deg;
        //Debug.Log(angle);

        // This pivots around the blue arrow (world-forward) based on angle
        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // This does the exact same thing, but in an arguably easier way
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, angle));
    }
}

