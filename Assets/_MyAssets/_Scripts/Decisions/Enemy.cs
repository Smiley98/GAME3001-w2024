using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject projectilePrefab;

    // Player variables
    public GameObject player;
    public bool playerDead = false;

    // Giving the enemy infinite view distance to make things simpler
    //[Range(0.0f, 25.0f)] public float viewDistance;

    // Enemy variables
    float speed = 10.0f;
    public float health = 100.0f;

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

    NearAttackAction nearAttackAction = new NearAttackAction();
    FarAttackAction farAttackAction = new FarAttackAction();

    void Start()
    {
        // 1. Assign data to nodes
        farDistance.agent = nearDistance.agent = gameObject;
        farDistance.target = nearDistance.target = player;
        farDistance.distance = 7.0f;
        nearDistance.distance = 3.5f;

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

        nearAttackAction.agent = gameObject;
        nearAttackAction.target = player;
        nearAttackAction.projectilePrefab = projectilePrefab;
        nearAttackAction.projectileSpeed = 5.0f;

        // 2. Build decision tree
        farDistance.no = patrolAction;
        farDistance.yes = visible;
        visible.no = moveToVisibleAction;
        visible.yes = nearDistance;
        nearDistance.no = moveToTargetAction;
        nearDistance.yes = nearAttackAction;

        // Lab 8 homework:
        // Implement a long-ranged enemy who fires a large bullet that deals significant damage.
        // The enemy will only shoot at the player if its 8+ units away!
        // (Optional) Add a FindCoverAction by seeking the nearest waypoint from which the player is not visible.
    }

    void Update()
    {
        // Don't evaluate based on a null object (player gets destroyed once it dies)
        if (!playerDead)
            TreeNode.Traverse(farDistance);
        else
            TreeNode.Traverse(patrolAction);
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
