using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DecisionTreeTheoryNodes : MonoBehaviour
{
    // Base class for all actions & decisions
    public abstract class TreeNode
    {
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
    public abstract class DecisionNode : TreeNode
    {
        public TreeNode yes = null;
        public TreeNode no = null;
    }

    // Actions are "leaf" nodes, meaning they're the bottom of the tree
    public class ActionNode : TreeNode
    {
        public override TreeNode Evaluate()
        {
            return null;
        }
    }

    [Serializable]
    public class VisibleNode : DecisionNode
    {
        public bool isVisible;

        public override TreeNode Evaluate()
        {
            return isVisible ? yes : no;
        }
    }

    [Serializable]
    public class RangeNode : DecisionNode
    {
        public bool isInRange;

        public override TreeNode Evaluate()
        {
            return isInRange ? yes : no;
        }
    }

    [Serializable]
    public class FlankNode : DecisionNode
    {
        public bool isFlank;

        public override TreeNode Evaluate()
        {
            return isFlank ? yes : no;
        }
    }

    [Serializable]
    public class AudibleNode : DecisionNode
    {
        public bool isAudible;

        public override TreeNode Evaluate()
        {
            return isAudible ? yes : no;
        }
    }

    public class CreepAction : ActionNode
    {
        public override TreeNode Evaluate()
        {
            Debug.Log("Creeping");
            return base.Evaluate();
        }
    }

    public class MoveAction : ActionNode
    {
        public override TreeNode Evaluate()
        {
            Debug.Log("Moving");
            return base.Evaluate();
        }
    }

    public class AttackAction : ActionNode
    {
        public override TreeNode Evaluate()
        {
            Debug.Log("Attacking");
            return base.Evaluate();
        }
    }

    [SerializeField] VisibleNode visible = new VisibleNode();
    [SerializeField] AudibleNode audible = new AudibleNode();
    [SerializeField] RangeNode range = new RangeNode();
    [SerializeField] FlankNode flank = new FlankNode();

    ActionNode nullAction = new ActionNode();
    ActionNode creep = new CreepAction();
    ActionNode move = new MoveAction();
    ActionNode attack = new AttackAction();

    void Start()
    {
        // Visible is our "root" node since its at the top of the tree
        visible.no = audible;
        visible.yes = range;

        audible.yes = creep;
        audible.no = nullAction;
        // (audible's no branch isn't necessary since yes/no are null by default)

        range.no = flank;
        range.yes = attack;

        flank.no = attack;
        flank.yes = move;
    }

    void Update()
    {
        TreeNode.Traverse(visible);
    }
}
