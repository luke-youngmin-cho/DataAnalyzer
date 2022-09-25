using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPart : MonoBehaviour
{
    public bool IsTriggered;
    public Collider TriggeredTarget;

    private void OnTriggerStay(Collider other)
    {
        TriggeredTarget = other;
        IsTriggered = other;
    }

    private void OnTriggerExit(Collider other)
    {
        IsTriggered = false;
    }
}
