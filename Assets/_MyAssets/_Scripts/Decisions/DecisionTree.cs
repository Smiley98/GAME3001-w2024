using System;
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
}

// Decision nodes return yes or no ("branch" nodes)
[Serializable]
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
public class VisibleNode : DecisionNode
{
    public bool isVisible;

    public override TreeNode Evaluate()
    {
        return isVisible ? yes : no;
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
    public override TreeNode Evaluate()
    {
        Debug.Log("Moving");
        return base.Evaluate();
    }
}

// Move to a position where we have a line-of-sight to the target ("point of visibility")
public class MoveToVisibleAction : ActionNode
{
    public override TreeNode Evaluate()
    {
        Debug.Log("Moving");
        return base.Evaluate();
    }
}

// Close attack
public class MeleeAttackAction : ActionNode
{
    public override TreeNode Evaluate()
    {
        Debug.Log("Attacking");
        return base.Evaluate();
    }
}

// Far attack
public class RangedAttackAction : ActionNode
{
    public override TreeNode Evaluate()
    {
        Debug.Log("Attacking");
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