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

    void Start()
    {
        farDistance.agent = gameObject;
        farDistance.target = player;

        farDistance.distance = 7.5f;
        nearDistance.distance = 2.5f;

        ColorAction colorAction = new ColorAction();
        colorAction.agent = gameObject;
        colorAction.color = Color.red;
        colorAction.Evaluate();
    }

    void Update()
    {
        
    }
}
