using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item base ( on battle field)
/// </summary>
public abstract class Item : MonoBehaviour
{
    public ItemData Data;
    public int Num;

    public virtual void PickUp()
    {
        InventoryData.Instance.AddItem(new ItemPair(Data.Code, Num));
        Destroy(gameObject);
    }

    public abstract void Use();

    private void FixedUpdate()
    {
        transform.Rotate(50.0f * Vector3.up * Time.fixedDeltaTime);
        
        transform.Translate(0.25f * Vector3.up * Mathf.Sin(4 * Time.time) * Time.fixedDeltaTime);
    }
}
