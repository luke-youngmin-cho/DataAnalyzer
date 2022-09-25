using System;
using System.Reflection;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour
{
    private static Dictionary<KeyCode, Action> _actions = new Dictionary<KeyCode, Action>();
    public static void SetAction(KeyCode keyCode, Action action)
    {
        if (_actions.ContainsKey(keyCode))
        {
            _actions[keyCode] = action;
        }
        else
        {
            _actions.Add(keyCode, action);
        }        
    }

    public static void ResetAction(KeyCode keyCode)
    {
        _actions[keyCode] = null;
    }

    public static void RegisterAction(KeyCode keyCode, Action action)
    {
        _actions[keyCode] += action;
    }

    private void Update()
    {
        CallKeyAction();
    }

    //private void CreateActions()
    //{
    //    Array keyCodes = Enum.GetValues(typeof(KeyCode));
    //    foreach (KeyCode keyCode in keyCodes)
    //    {
    //        _actions.Add(keyCode, (Action)Delegate.CreateDelegate(typeof(Action), null));
    //    }
    //}

    private void CallKeyAction()
    {
        foreach (KeyCode key in _actions.Keys)
        {
            if (Input.GetKeyDown(key))
                _actions[key]?.Invoke();
        }
    }
}