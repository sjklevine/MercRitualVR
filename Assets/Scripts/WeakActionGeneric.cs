﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WeakAction<T> : WeakAction
{
    private Action<T> _staticAction;

    public override string MethodName
    {
        get
        {
            if (_staticAction != null)
                return _staticAction.Method.Name;

            return Method.Name;
        }
    }

    public override bool IsAlive
    {
        get
        {
            if (_staticAction == null && Reference == null)
                return false;

            if (_staticAction != null)
            {
                if (Reference != null)
                    return Reference.IsAlive;

                return true;
            }

            return Reference.IsAlive;
        }
    }

    public WeakAction(Action<T> action) : this(action == null ? null : action.Target, action) { }

    public WeakAction(object target, Action<T> action)
    {
        if (action.Method.IsStatic)
        {
            _staticAction = action;

            if (target != null)
                Reference = new WeakReference(target);

            return;
        }

        Method = action.Method;
        ActionReference = new WeakReference(action.Target);
        Reference = new WeakReference(target);
    }

    public new void Execute()
    {
        Execute(default(T));
    }

    public void Execute(T parameter)
    {
        if (_staticAction != null)
        {
            _staticAction(parameter);
            return;
        }

        var actionTarget = ActionTarget;

        if (IsAlive)
        {
            if (Method != null &&
                ActionReference != null &&
                actionTarget != null)
            {
                Method.Invoke(actionTarget, new object[] { parameter });
            }
        }
    }

    public new void MarkForDeletion()
    {
        _staticAction = null;
        base.MarkForDeletion();
    }
}
