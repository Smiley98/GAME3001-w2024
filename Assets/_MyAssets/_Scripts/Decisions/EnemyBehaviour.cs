using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject player;
    public Transform[] waypoints;
    int nextWaypoint = 0;

    // 10 units per second
    float speed = 10.0f;

    DistanceNode farDistance = new DistanceNode();
    //VisibleNode visible = new VisibleNode();
    //DistanceNode nearDistance = new DistanceNode();
    //
    //ActionNode nullAction = new ActionNode();
    //ActionNode meleeAction = new MeleeAttackAction();
    //ActionNode rangedAction = new RangedAttackAction();

    ColorAction redColorAction = new ColorAction();
    ColorAction greenColorAction = new ColorAction();

    void Start()
    {
        farDistance.agent = gameObject;
        farDistance.target = player;
        farDistance.distance = 7.5f;
        
        redColorAction.agent = gameObject;
        redColorAction.color = Color.red;

        greenColorAction.agent = gameObject;
        greenColorAction.color = Color.green;

        farDistance.yes = greenColorAction;
        farDistance.no = redColorAction;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        TreeNode.Traverse(farDistance);

        Vector3 enemyPosition = transform.position;
        Vector3 targetPosition = waypoints[nextWaypoint].position;
        transform.position = Vector3.MoveTowards(enemyPosition, targetPosition, speed * dt);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Waypoint"))
        {
            nextWaypoint++;
            if (nextWaypoint >= waypoints.Length) nextWaypoint = 0;


        }
    }
}
