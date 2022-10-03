using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _itemLayer;
    private void OnTriggerStay(Collider other)
    {
        if (1<<other.gameObject.layer == _itemLayer)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                other.gameObject.GetComponent<Item>().PickUp();
            }
        }
    }
}
