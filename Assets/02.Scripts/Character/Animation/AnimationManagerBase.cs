using System.Text;
using UnityEngine;

public class AnimationManagerBase : MonoBehaviour
{
    public bool IsComboAvailable;
    [SerializeField] private Animator _animator;
    private AnimatorSubStateMonitor[] _monitors;
    //private StringBuilder _monitorOnString = new StringBuilder();
    //private StringBuilder _monitorOffString = new StringBuilder();
    //private StringBuilder _monitorOnStringMem = new StringBuilder();
    private int _monitorOnStateHash;
    private int _monitorOffStateHash;
    private int _monitorOnStateHashMem;

    public bool IsPreviousAnimationFinished
    {
        get
        {
            return _monitorOffStateHash == _monitorOnStateHashMem;
        }
    }

    public void Play(string clipName)
    {
        _animator.Play(clipName);
    }

    public float GetClipTime(string clipName)
    {
        RuntimeAnimatorController ac = _animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == clipName)
                return ac.animationClips[i].length;
        }
        return -1.0f;
    }

    public float GetCurrentNormalizedTime()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }


    public bool IsClipPlaying(string clipName)
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName(clipName);
    }

    public void SetBool(string name, bool value) => _animator.SetBool(name, value);
    public void SetFloat(string name, float value) => _animator.SetFloat(name, value);
    public void SetTrigger(string name) => _animator.SetTrigger(name);
    public void SetInt(string name, int value) => _animator.SetInteger(name, value);
    public bool GetBool(string name) => _animator.GetBool(name);
    public float GetFloat(string name) => _animator.GetFloat(name);

    public void EnableCombo()
    {
        IsComboAvailable = true;
    }

    public void DisableCombo()
    {
        IsComboAvailable = false;
    }


    private void Awake()
    {
        _monitors = _animator.GetBehaviours<AnimatorSubStateMonitor>();

        for (int i = 0; i < _monitors.Length; i++)
        {
            _monitors[i].OnEnter += (hash) => {
                //_monitorOnStringMem.Clear();
                //_monitorOnStringMem.Append(_monitorOnString);
                //_monitorOnString.Clear();
                //_monitorOnString.Append(para);
                _monitorOnStateHashMem = _monitorOnStateHash;
                _monitorOnStateHash = hash;
            };


            _monitors[i].OnExit += (hash) =>
            {
                //_monitorOffString.Clear();
                //_monitorOffString.Append(para);
                _monitorOffStateHash = hash;
            };
        }
    }
}