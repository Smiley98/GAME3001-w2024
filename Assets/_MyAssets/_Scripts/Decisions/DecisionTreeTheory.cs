using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionTreeTheory : MonoBehaviour
{
    public bool isVisible = false;
    public bool isInRange = false;
    public bool isAudible = false;
    public bool isFlank = false;

    void Update()
    {
        if (isVisible)
        {
            if (isInRange)
            {
                Attack();
            }
            else
            {
                if (isFlank)
                {
                    Move();
                }
                else
                {
                    Attack();
                }
            }
        }
        else
        {
            if (isAudible)
            {
                Creep();
            }
            else
            {
                // Empty node -- no action
                // (You don't actually need this else branch since nothing happens)
            }
        }
    }

    void Attack()
    {
        Debug.Log("Attacking");
    }

    void Move()
    {
        Debug.Log("Moving");
    }

    void Creep()
    {
        Debug.Log("Creeping");
    }
}
