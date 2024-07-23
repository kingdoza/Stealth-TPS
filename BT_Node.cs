using System.Collections.Generic;
using UnityEngine;

public enum BT_NodeState {
    Success, Failure, Running
}

public abstract class BT_Node {
    public abstract BT_NodeState Execute();
    public virtual void Traverse(System.Action<BT_Node> visitor) {
        visitor(this);
    }
}

public class Selector : BT_Node {
    private List<BT_Node> _children = new List<BT_Node>();

    public Selector(List<BT_Node> children) {
        _children = children;
    }

    public override BT_NodeState Execute() {
        foreach (BT_Node node in _children) {
            BT_NodeState state = node.Execute();
            if (state != BT_NodeState.Failure) {
                return state;
            }
        }
        return BT_NodeState.Failure;
    }

    public override void Traverse(System.Action<BT_Node> visitor) {
        base.Traverse(visitor);
        foreach (var child in _children) {
            child.Traverse(visitor);
        }
    }
}

public class Sequence : BT_Node {
    private List<BT_Node> _children = new List<BT_Node>();
    private ConditionNode conditionNode;

    public Sequence(List<BT_Node> children) {
        _children = children;
    }

    public Sequence(ConditionNode conditionNode)
    {
        this.conditionNode = conditionNode;
    }

    public override BT_NodeState Execute() {
        foreach (BT_Node node in _children) {
            BT_NodeState state = node.Execute();
            if (state != BT_NodeState.Success) {
                return state;
            }
        }
        return BT_NodeState.Success;
    }

    public override void Traverse(System.Action<BT_Node> visitor) {
        base.Traverse(visitor);
        foreach (var child in _children) {
            child.Traverse(visitor);
        }
    }

    public void ResetOnceNodes() {
        Traverse(node => {
            if (node is OnceNode onceNode) {
                onceNode.Reset();
            }
        });
    }
}

public class ActionNode : BT_Node {
    private System.Func<BT_NodeState> _action;

    public ActionNode(System.Func<BT_NodeState> action) {
        _action = action;
    }

    public override BT_NodeState Execute() {
        return _action();
    }
}

public class ConditionNode : BT_Node {
    private System.Func<bool> _condition;

    public ConditionNode(System.Func<bool> condition) {
        _condition = condition;
    }

    public override BT_NodeState Execute() {
        return _condition() ? BT_NodeState.Success : BT_NodeState.Failure;
    }
}

public class WaitNode : BT_Node {
    private float _waitTime;
    private float _startTime;
    private bool _isStarted;

    public WaitNode(float waitTime) {
        _waitTime = waitTime;
    }

    public override BT_NodeState Execute() {
        if (!_isStarted) {
            _startTime = Time.time;
            _isStarted = true;
        }
        Debug.Log(Time.time - _startTime);
        if (Time.time - _startTime >= _waitTime) {
            _isStarted = false; // Reset for the next execution
            return BT_NodeState.Success;
        }
        return BT_NodeState.Running;
    }
}

public class OnceNode : BT_Node {
    private bool _hasExecuted;
    private BT_Node _actionNode;

    public OnceNode(BT_Node actionNode) {
        _actionNode = actionNode;
        _hasExecuted = false;
    }

    public override BT_NodeState Execute() {
        if (!_hasExecuted) {
            BT_NodeState state = _actionNode.Execute();
            if (state == BT_NodeState.Success) {
                _hasExecuted = true;
            }
            return state;
        }
        return BT_NodeState.Success;
    }

    public void Reset() {
        _hasExecuted = false;
    }

    public override void Traverse(System.Action<BT_Node> visitor) {
        base.Traverse(visitor);
        _actionNode.Traverse(visitor);
    }
}

public class ServiceNode : BT_Node {
    private BT_Node _childNode;
    private System.Action _serviceAction;
    private float _interval;
    private float _lastExecutionTime;

    public ServiceNode(System.Action serviceAction, float interval, BT_Node childNode) {
        _childNode = childNode;
        _serviceAction = serviceAction;
        _interval = interval;
        _lastExecutionTime = Time.time;
    }

    public override BT_NodeState Execute() {
        if (Time.time - _lastExecutionTime >= _interval) {
            _serviceAction.Invoke();
            _lastExecutionTime = Time.time;
        }
        return _childNode.Execute();
    }
}

public class ConditionalRunDecorator : BT_Node {
    private BT_Node child;
    private ActionNode action;

    public ConditionalRunDecorator(BT_Node child, ActionNode action) {
        this.child = child;
        this.action = action;
    }

    public override BT_NodeState Execute () {
        if (child.Execute() != BT_NodeState.Running) {
            action.Execute();
        };
        return BT_NodeState.Success;
    }
}

public class Parallel : BT_Node {
    private List<BT_Node> _children;
    private bool _requireAllSuccess;
    private bool _requireOneSuccess;

    public Parallel(List<BT_Node> children, bool requireAllSuccess = false, bool requireOneSuccess = true) {
        _children = children;
        _requireAllSuccess = requireAllSuccess;
        _requireOneSuccess = requireOneSuccess;
    }

    public override BT_NodeState Execute() {
        bool allSucceeded = true;
        bool oneSucceeded = false;

        foreach (BT_Node child in _children) {
            BT_NodeState state = child.Execute();
            if (state == BT_NodeState.Failure) {
                if (_requireAllSuccess) {
                    return BT_NodeState.Failure;
                }
            } else if (state == BT_NodeState.Success) {
                oneSucceeded = true;
            } else {
                allSucceeded = false;
            }
        }

        if (_requireAllSuccess && allSucceeded) {
            return BT_NodeState.Success;
        }

        if (_requireOneSuccess && oneSucceeded) {
            return BT_NodeState.Success;
        }
        return BT_NodeState.Running;
    }
}