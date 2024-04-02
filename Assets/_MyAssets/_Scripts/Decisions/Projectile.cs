using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroy projectile if it collides with anything NOT on the Ignore Raycast layer
        if (collision.gameObject.layer != 2)
        {
            Destroy(gameObject);
        }
    }
}
