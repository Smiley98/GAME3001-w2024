using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    // Player variables
    public GameObject player;
    [Range(0.0f, 25.0f)] public float viewDistance;

    // Enemy variables
    float speed = 10.0f;

    // Game world variables
    public Transform[] waypoints;
    int nextWaypoint = 0;

    DistanceNode farDistance = new DistanceNode();
    VisibleNode visible = new VisibleNode();
    //DistanceNode nearDistance = new DistanceNode();
    //
    //ActionNode nullAction = new ActionNode();
    //ActionNode meleeAction = new MeleeAttackAction();
    //ActionNode rangedAction = new RangedAttackAction();

    ColorAction redColorAction = new ColorAction();
    ColorAction greenColorAction = new ColorAction();

    PatrolAction patrolAction = new PatrolAction();
    MoveToVisibleAction moveToVisibleAction = new MoveToVisibleAction();

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

        moveToVisibleAction.agent = gameObject;
        moveToVisibleAction.target = player;
        moveToVisibleAction.speed = speed;
        moveToVisibleAction.waypoints = waypoints;

        greenColorAction.next = redColorAction.next = patrolAction;
        patrolAction.agent = gameObject;
        patrolAction.waypoints = waypoints;
        patrolAction.speed = speed;

        patrolAction.next = moveToVisibleAction;

        visible.agent = gameObject;
        visible.target = player;

        visible.distance = viewDistance;

        visible.yes = greenColorAction;
        visible.no = redColorAction;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        //TreeNode.Traverse(farDistance);
        TreeNode.Traverse(visible);
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
