using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// static class = behaviour (functions) only
public static class Steering
{
    // We started with a Unity-based seeking approach
    //public static void Seek(GameObject seeker, Vector3 target, float moveSpeed)
    //{
    //    Rigidbody2D seekerBody = seeker.GetComponent<Rigidbody2D>();
    //    Vector2 currentVelocity = seekerBody.velocity;
    //    Vector2 desiredVelocity = (target - seeker.transform.position).normalized;
    //    desiredVelocity *= moveSpeed;
    //
    //    Vector2 seekForce = desiredVelocity - currentVelocity;
    //    seekerBody.AddForce(seekForce);
    //}

    // We can refine our code by extracting only the math and then appling it to Unity objects
    public static Vector3 Seek(Vector3 seekerPosition, Vector3 seekerVelocity, float moveSpeed, Vector3 targetPosition)
    {
        return (targetPosition - seekerPosition).normalized * moveSpeed - seekerVelocity;
    }
}
