using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpTool : MonoBehaviour
{
    [SerializeField] private bool doShakeEffect;
    [SerializeField] private float shakeEffectAmplitude;
    [SerializeField] private float shakeEffectTime;
    [SerializeField] private bool doAutoClose;
    [SerializeField] private float closeDelay;
    private Vector3 _posOrigin;


    IEnumerator E_ShakeEffect()
    {
        float timer = shakeEffectTime;
        while (timer > 0)
        {
            transform.position = _posOrigin;
            transform.position += (Vector3.up + Vector3.left) * shakeEffectAmplitude;
            timer -= Time.deltaTime;
            yield return null;
            transform.position = _posOrigin;
            transform.position += (Vector3.down + Vector3.left) * shakeEffectAmplitude;
            timer -= Time.deltaTime;
            yield return null;
            transform.position = _posOrigin;
            transform.position += (Vector3.down + Vector3.right) * shakeEffectAmplitude;
            timer -= Time.deltaTime;
            yield return null;
            transform.position = _posOrigin;
            transform.position += (Vector3.up + Vector3.right) * shakeEffectAmplitude;
            timer -= Time.deltaTime;
            yield return null;
        }
        transform.position = _posOrigin;

        if (doAutoClose)
        {
            yield return new WaitForSeconds(closeDelay);
            gameObject.SetActive(false);
        }   
    }

    private void Awake()
    {
        _posOrigin = transform.position;
    }

    private void OnEnable()
    {
        StartCoroutine(E_ShakeEffect());
    }
}
