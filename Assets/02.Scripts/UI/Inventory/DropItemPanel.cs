using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

/// <summary>
/// input panel to enter how many items want to drop
/// </summary>
public class DropItemPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _ok;
    [SerializeField] private Button _cancel;

    public void SetOKButtonListener(UnityAction action)
    {
        _ok.onClick.RemoveAllListeners();
        _ok.onClick.AddListener(action);
    }

    public int GetInputNum()
    {
        return Convert.ToInt32(_inputField.text);
    }
}
