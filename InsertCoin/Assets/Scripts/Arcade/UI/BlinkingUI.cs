using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingUI : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour _component;

    [SerializeField]
    [Range(1f, 8f)]
    private float _frequency;

    // Update is called once per frame
    void Update()
    {
        float delta = 1f / _frequency;
        _component.enabled = Mathf.Repeat(Time.time, delta * 2f) > delta;
    }
}
