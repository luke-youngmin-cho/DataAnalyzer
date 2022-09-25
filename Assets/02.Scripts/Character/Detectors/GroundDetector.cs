using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public bool IsDetected;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _detectRadius;
    //private void FixedUpdate()
    //{
    //    IsDetected = Physics.CheckSphere(transform.position, _detectRadius, _groundLayer, QueryTriggerInteraction.Ignore);
    //}
    //
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, _detectRadius);
    //}

    private void OnTriggerStay(Collider other)
    {
        IsDetected = true;
    }

    private void OnTriggerExit(Collider other)
    {
        IsDetected = false;
    }
}
