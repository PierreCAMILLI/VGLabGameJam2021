using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooleanTrigger
{
    private bool _value;

    public BooleanTrigger()
    {
        _value = false;
    }

    public void Set()
    {
        _value = true;
    }

    public bool Get()
    {
        if (_value)
        {
            _value = false;
            return true;
        }
        return false;
    }
}
