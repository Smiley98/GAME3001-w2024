using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject player;

    VisibleNode visible = new VisibleNode();
    DistanceNode farDistance = new DistanceNode();
    DistanceNode nearDistance = new DistanceNode();

    ActionNode nullAction = new ActionNode();
    ActionNode meleeAction = new MeleeAttackAction();
    ActionNode rangedAction = new RangedAttackAction();

    ColorAction redColorAction = new ColorAction();
    ColorAction greenColorAction = new ColorAction();

    void Start()
    {
        farDistance.agent = gameObject;
        farDistance.target = player;
        farDistance.distance = 7.5f;
        
        redColorAction.agent = gameObject;
        redColorAction.color = Color.red;
        
        redColorAction.agent = gameObject;
        redColorAction.color = Color.green;

        farDistance.yes = greenColorAction;
        farDistance.no = redColorAction;
    }

    void Update()
    {
        TreeNode.Traverse(farDistance);
    }
}