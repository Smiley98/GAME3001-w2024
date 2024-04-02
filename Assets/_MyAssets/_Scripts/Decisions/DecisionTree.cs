using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for all actions & decisions
public abstract class TreeNode
{
    // This is no longer re-usable since we're storing agent & target, but you could make it more generic
    public GameObject agent;    // Our AI (thing that's running the decision tree)
    public GameObject target;   // The player

    public abstract TreeNode Evaluate();

    // Recursively traverse our decision tree until we reach a leaf (null node)
    public static void Traverse(TreeNode node)
    {
        if (node != null)
        {
            Traverse(node.Evaluate());
        }
    }

    // Tests if there's a line of sight from view position to player
    protected bool IsPlayerVisible(Vector3 viewPosition, float viewDistance = float.PositiveInfinity)
    {
        // Both the Enemy and Waypoints are on layer 2 which is "Ignore Raycast"
        int layerMask = ~(1 << 2);
        Vector3 direction = (target.transform.position - viewPosition).normalized;
        RaycastHit2D hit = Physics2D.Raycast(viewPosition, direction, viewDistance, layerMask);
        return hit.collider && hit.collider.CompareTag(target.tag);
    }
}

// Decision nodes return yes or no ("branch" nodes)
[System.Serializable]
public abstract class DecisionNode : TreeNode
{
    public TreeNode yes = null;
    public TreeNode no = null;
}

// Actions are "leaf" nodes, meaning they're the bottom of the tree
public class ActionNode : TreeNode
{
    public ActionNode next = null;

    public override TreeNode Evaluate()
    {
        return next;
    }
}

// Whether or not we have line of sight
//public class VisibleNode : DecisionNode
//{
//    public bool isVisible;
//
//    public override TreeNode Evaluate()
//    {
//        return isVisible ? yes : no;
//    }
//}

public class VisibleNode : DecisionNode
{
    // Using infinite distance for simplicity
    //public float distance;

    public override TreeNode Evaluate()
    {
        // Unity built-in raycast layers:
        // 0 = Default
        // 1 = Transparent
        // 2 = Ignore Raycast
        // 3 = ???
        // 4 = Water
        // 5 = UI

        // Decimal number system is how must (non-programmer) humans count
        // Called "base 10" because there's 10 digits -- 0 through 9
        // 123
        // 1 in the 100's column,
        // 2 in the 10's column,
        // 3 in the 1's column,

        // Binary number system is base 2 because there's 2 digits -- 0 or 1
        // [Easiest to read binary right to left]
        // 1 1 1 0
        // 0 in the 1's column
        // 1 in the 2's column
        // 1 in the 4's column
        // 1 in the 8's column
        // 8 + 4 + 2 + 0 = 14 (in decimal)

        // Unity has 32 possible layers becuase there are 32 bits in a (4 byte, default) integer
        // Hence, we must bit-shift in order to determine which layer to ignore.
        // We work in multiples of 2. Consider the first (right-most) 8 bits:
        // (We must start with the right-most bit [1's column] as 1 otherwise its like multiplying by 0)
        // 0 0 0 0 0 0 0 1
        // 1 << 0 = 1   ---> 0 0 0 0 0 0 0 1
        // 1 << 1 = 2   ---> 0 0 0 0 0 0 1 0
        // 1 << 2 = 4   ---> 0 0 0 0 0 1 0 0
        // 1 << 3 = 8   ---> 0 0 0 0 1 0 0 0

        // In this context, 0 means ignore, 1 means don't ignore.
        // The Ignore Raycast has a value of 2 meaning it corresponds to the 2nd bit.
        // Hence, we take 1, shift it left twice, then negate it to ignore every bit except bit 2.
        //int layerIndex = LayerMask.NameToLayer("Ignore Raycast");
        //int layerMask = 1 << layerIndex;
        //layerMask = ~layerMask;
        // Expressed in one line (assuming agent.layer == Ignore Raycast):

        // Initial value: 0 0 0 0 0 0 0 1
        // Value after shifting left based on layer (2)
        // 1 << 2 --> 0 0 0 0 0 1 0 0
        // ~(1 << 2) "negate" -- flips our bits:
        // 0 0 0 0 0 1 0 0
        // 1 1 1 1 1 0 1 1

        // The following was moved to IsPlayerVisible function in base class!
        //int layerMask = ~(1 << agent.layer);
        //RaycastHit2D hit = Physics2D.Raycast(from, direction, distance, layerMask);
        //bool targetHit = hit.collider && hit.collider.CompareTag(target.tag);

        return IsPlayerVisible(agent.transform.position) ? yes : no;
    }
}

// Whether the target is within the desired distance of the agent
public class DistanceNode : DecisionNode
{
    public float distance;

    public override TreeNode Evaluate()
    {
        return Vector2.Distance(agent.transform.position, target.transform.position) <= distance ? yes : no;
    }
}

// Move to target object
public class MoveToTargetAction : ActionNode
{
    public float speed;

    public override TreeNode Evaluate()
    {
        agent.transform.position = Vector3.MoveTowards(agent.transform.position, target.transform.position, speed * Time.deltaTime);
        return base.Evaluate();
    }
}

// Move to a position where we have a line-of-sight to the target ("point of visibility")
public class MoveToVisibleAction : ActionNode
{
    public Transform[] waypoints;
    public float speed;

    public override TreeNode Evaluate()
    {
        float nearestDistance = float.PositiveInfinity;
        int nearestIndex = 0;
        for (int i = 0; i < waypoints.Length; i++)
        {
            bool visible = IsPlayerVisible(waypoints[i].transform.position);
            float distance = Vector2.Distance(agent.transform.position, waypoints[i].transform.position);
            if (visible && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestIndex = i;
            }
        }

        Vector3 current = agent.transform.position;
        Vector3 target = waypoints[nearestIndex].transform.position;
        agent.transform.position = Vector3.MoveTowards(current, target, speed * Time.deltaTime);

        return base.Evaluate();
    }
}

// Close attack
public class NearAttackAction : ActionNode
{
    public GameObject projectilePrefab;

    public float cooldown = 0.5f;
    float time = 0.0f;

    public override TreeNode Evaluate()
    {
        float dt = Time.deltaTime;
        if (time >= cooldown)
        {
            // Fire projectile then reset timer if off cooldown
            time = 0.0f;

            Vector3 from = agent.transform.position;
            Vector3 to = target.transform.position;
            Vector3 direction = (to - from).normalized;
            Vector3 left = Quaternion.Euler(0.0f, 0.0f, 30.0f) * direction;
            Vector3 right = Quaternion.Euler(0.0f, 0.0f, -30.0f) * direction;

            GameObject centre = Object.Instantiate(projectilePrefab);
            GameObject top = Object.Instantiate(projectilePrefab);
            GameObject bot = Object.Instantiate(projectilePrefab);
            centre.transform.position = from + direction;
            top.transform.position = from + left;
            bot.transform.position = from + right;
            centre.GetComponent<Rigidbody2D>().velocity = direction * 2.0f;
            top.GetComponent<Rigidbody2D>().velocity = left * 2.0f;
            bot.GetComponent<Rigidbody2D>().velocity = right * 2.0f;
        }
        time += dt;
        return base.Evaluate();
    }
}

// Far attack
public class FarAttackAction : ActionNode
{
    public GameObject projectilePrefab;

    public override TreeNode Evaluate()
    {
        // Consider making a sniper rifle that shoots a single bullet really fast here
        return base.Evaluate();
    }
}

public class ColorAction : ActionNode
{
    public Color color;

    public override TreeNode Evaluate()
    {
        agent.GetComponent<SpriteRenderer>().color = color;
        return base.Evaluate();
    }
}

public class PatrolAction : ActionNode
{
    public Transform[] waypoints;
    public int nextWaypoint = 0;
    public float speed;

    public override TreeNode Evaluate()
    {
        Vector3 enemyPosition = agent.transform.position;
        Vector3 targetPosition = waypoints[nextWaypoint].position;
        agent.transform.position = Vector3.MoveTowards(enemyPosition, targetPosition, speed * Time.deltaTime);
        return base.Evaluate();
    }
}