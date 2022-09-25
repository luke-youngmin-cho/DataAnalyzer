using System.Reflection;
using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    private bool _isMovable;
    public bool IsMovable
    {
        get
        {
            return _isMovable;
        }
        set
        {
            if (value == true)
            {
                _targetPos = _targetPosMem;
            }
            _isMovable = value;
        }
    }
    public bool IsMoving { get => _doMove; }
    public float Speed
    {
        get
        {
            float tmp = 0.0f;
            if (IsMoving)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    tmp = PlayerInfo.DashSpeed * SpeedGain;
                else
                    tmp = PlayerInfo.MoveSpeed * SpeedGain;
            }
            else
            {
                tmp = 0.0f;
            }

            if (tmp > PlayerInfo.MoveSpeedMax)
                tmp = PlayerInfo.MoveSpeedMax;

            return tmp;
        }
    }
    public float SpeedGain = 1.0f;
    private Rigidbody _rb;
    private Vector3 _targetPos;
    private Vector3 _targetPosMem;
    private bool _doMove;
    private Plane _plane;
    private Ray _ray;
    private RaycastHit _hit;
    private Camera _camera;
    [SerializeField] private LayerMask _groundLayer;
    private StateMachineForPlayer _machine;
    private float _positionWindow = 0.1f;
    public void Stop()
    {
        _rb.velocity = new Vector3(0.0f, _rb.velocity.y, 0.0f);
        _doMove = false;
    }

    public void ResetVelocityY()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0.0f, _rb.velocity.z);
    }


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _machine = GetComponent<StateMachineForPlayer>();
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            MouseRightDownAction();
        }

        if (Input.GetMouseButtonDown(0))
        {
            MouseLeftDownAction();
        }
    }

    private void MouseRightDownAction()
    {
        _plane = new Plane(transform.up, transform.position);
        _ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(_ray, out _hit, Mathf.Infinity))
        {
            if (1 << _hit.collider.gameObject.layer == _groundLayer)
            {
                _targetPosMem = _hit.point;

                if (IsMovable)
                {
                    _targetPos = _targetPosMem;
                    _doMove = true;
                }
            }
        }
    }

    private void MouseLeftDownAction()
    {

    }

    private void FixedUpdate()
    {
        if (_doMove)
        {
            Move();
            Turn();
        }
    }

    private void Move()
    {
        if (Vector3.Distance(_rb.position, _targetPos) > _positionWindow)
        {
            Vector3 dir = (_targetPos - _rb.position).normalized;
            //_rb.MovePosition(_rb.position + dir * Speed * Time.fixedDeltaTime);
            _rb.position += new Vector3(dir.x, 0.0f, dir.z) * Speed * Time.fixedDeltaTime;
            //_rb.velocity = new Vector3(dir.x, _rb.velocity.y, dir.z) * Speed;
        }
        else if (Vector3.Distance(_rb.position, _targetPosMem) > _positionWindow)
        {
            // wait
        }
        else
        {
            Stop();
        }
    }

    private void Turn()
    {
        Vector3 lookDir = _targetPos - _rb.position;
        lookDir.y = 0;
        _rb.rotation = Quaternion.LookRotation(lookDir);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_targetPos, 0.3f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_targetPosMem, 0.3f);
    }
}