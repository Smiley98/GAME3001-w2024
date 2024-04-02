using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    // Player variables
    public GameObject player;

    // Giving the enemy infinite view distance to make things simpler
    //[Range(0.0f, 25.0f)] public float viewDistance;

    // Enemy variables
    float speed = 10.0f;

    // Game world variables
    public Transform[] waypoints;
    int nextWaypoint = 0;

    // Testing only
    //ColorAction redColorAction = new ColorAction();
    //ColorAction greenColorAction = new ColorAction();
    //redColorAction.agent = gameObject;
    //redColorAction.color = Color.red;
    //greenColorAction.agent = gameObject;
    //greenColorAction.color = Color.green;

    DistanceNode farDistance = new DistanceNode();
    DistanceNode nearDistance = new DistanceNode();
    PatrolAction patrolAction = new PatrolAction();

    VisibleNode visible = new VisibleNode();
    MoveToVisibleAction moveToVisibleAction = new MoveToVisibleAction();
    MoveToTargetAction moveToTargetAction = new MoveToTargetAction();

    ActionNode nearAttackAction = new NearAttackAction();
    ActionNode farAttackAction = new FarAttackAction();

    void Start()
    {
        // 1. Assign data to nodes
        farDistance.agent = nearDistance.agent = gameObject;
        farDistance.target = nearDistance.target = player;
        farDistance.distance = 7.5f;
        nearDistance.distance = 2.5f;

        visible.agent = gameObject;
        visible.target = player;

        patrolAction.agent = gameObject;
        patrolAction.waypoints = waypoints;
        patrolAction.speed = speed;

        moveToVisibleAction.agent = gameObject;
        moveToVisibleAction.target = player;
        moveToVisibleAction.waypoints = waypoints;
        moveToVisibleAction.speed = speed;

        moveToTargetAction.agent = gameObject;
        moveToTargetAction.target = player;
        moveToTargetAction.speed = speed;

        // 2. Build decision tree
        farDistance.no = patrolAction;
        farDistance.yes = visible;
        visible.no = moveToVisibleAction;
        visible.yes = nearDistance;
        nearDistance.no = moveToTargetAction;
        nearDistance.yes = nearAttackAction;
    }

    void Update()
    {
        TreeNode.Traverse(farDistance);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Waypoint"))
        {
            nextWaypoint++;
            nextWaypoint %= waypoints.Length;
            patrolAction.nextWaypoint = nextWaypoint;
        }
    }
}
