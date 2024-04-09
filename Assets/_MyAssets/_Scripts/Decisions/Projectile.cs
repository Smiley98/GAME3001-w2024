using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum Type
    {
        NONE,
        PLAYER,
        ENEMY
    }

    public Type type;
    public float damage;

    public void Create(Type type, float damage, Vector3 position, Vector3 direction, float speed)
    {
        this.type = type;
        this.damage = damage;
        transform.position = position;
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Damage enemy if hit by a player projectile
        if (type == Type.PLAYER && collision.name == "Enemy")
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.health -= damage;
            Destroy(gameObject);
            if (enemy.health <= 0.0f)
                Destroy(enemy.gameObject);
        }

        // Damage player if hit by an enemy projectile
        if (type == Type.ENEMY && collision.name == "Player")
        {
            Player player = collision.GetComponent<Player>();
            player.health -= damage;
            Destroy(gameObject);
            if (player.health <= 0.0f)
            {
                Enemy enemy = (Enemy)FindObjectOfType(typeof(Enemy));
                enemy.playerDead = true;
                Destroy(player.gameObject);
            }
        }

        if (collision.name == "Obstacle")
            Destroy(gameObject);
    }
}
