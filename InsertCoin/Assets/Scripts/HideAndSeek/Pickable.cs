using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    [SerializeField]
    [Range(0.01f, 1f)]
    private float _smoothTimeOnPickUp;

    public System.Action OnPicked { get; set; }
    public System.Action OnGoalReached { get; set; }

    private Transform _pocketTransform;
    private Vector3 _velocity;

    // Update is called once per frame
    void Update()
    {
        if (_pocketTransform)
        {
            transform.position = Vector3.SmoothDamp(transform.position, _pocketTransform.position, ref _velocity, _smoothTimeOnPickUp, Mathf.Infinity, Time.deltaTime);
            if (OnGoalReached != null && IsGoalReached())
            {
                OnGoalReached.Invoke();
            }
        }
    }

    public void Pick(Transform pocket)
    {
        _pocketTransform = pocket;
        if (OnPicked != null)
        {
            OnPicked.Invoke();
        }
    }

    public void Drop()
    {
        _pocketTransform = null;
    }

    public bool IsGoalReached()
    {
        if (_pocketTransform)
        {
            return Vector3.SqrMagnitude(_pocketTransform.position - transform.position) < 0.01f;
        }
        return false;
    }
}
