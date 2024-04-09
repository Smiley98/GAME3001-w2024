using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float health = 100.0f;
    float speed = 10.0f;

    const float cooldown = 0.5f;
    float time = 0.0f;

    void Update()
    {
        float dt = Time.deltaTime;
        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += Vector3.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection += Vector3.down;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector3.right;
        }
        transform.position += moveDirection * speed * dt;

        // Shoot ourselves as a test xD
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    GameObject projectile = Instantiate(projectilePrefab);
        //    projectile.GetComponent<Projectile>().Create(Projectile.Type.ENEMY, 50.0f,
        //        transform.position - Vector3.up * 2.0f, Vector3.up, 10.0f);
        //}

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 lookDirection = (mouse - transform.position).normalized;
        if (Input.GetKey(KeyCode.Space) && time > cooldown)
        {
            time = 0.0f;
            GameObject projectile = Instantiate(projectilePrefab);
            projectile.GetComponent<Projectile>().Create(Projectile.Type.PLAYER, 50.0f,
                transform.position + lookDirection, lookDirection, 10.0f);
        }
        time += Time.deltaTime;
    }
}
