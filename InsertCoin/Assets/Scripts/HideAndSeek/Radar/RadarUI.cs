using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarUI : MonoBehaviour
{
    [SerializeField]
    private HeroController _hero;

    [SerializeField]
    private Transform _coin;

    [Space]
    [SerializeField]
    private UnityEngine.UI.Image _image;

    [SerializeField]
    private Color _farColor;

    [SerializeField]
    private Color _closeColor;

    [SerializeField]
    private Color _disabled;

    [Space]
    [SerializeField]
    private float _radius;

    [SerializeField]
    private float _offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_hero.HasCoin && _coin.gameObject.activeSelf)
        {
            float distance = Vector3.Magnitude(_hero.transform.position - _coin.position);
            float lerp = Mathf.InverseLerp(_offset, _offset + _radius, distance);
            _image.color = Color.Lerp(_closeColor, _farColor, lerp);
        }
        else
        {
            _image.color = _disabled;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_coin)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_coin.position, _offset);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_coin.position, _offset + _radius);
        }
    }
}
