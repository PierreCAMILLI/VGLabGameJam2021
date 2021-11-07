using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroController : MonoBehaviour
{
    private CharacterController _characterController;

    [SerializeField]
    private float _maxSpeed = 5f;

    [SerializeField]
    private Transform _headTransform;

    [SerializeField]
    private Transform _pocketTransform;
    public Transform PocketTransform { get { return _pocketTransform; } }

    public bool HasCoin { get; set; }

    private Vector2 _direction;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction3d = transform.forward * _direction.y + transform.right * _direction.x;
        _characterController.Move(direction3d * Time.deltaTime * _maxSpeed);
    }

    public void Move(Vector2 direction)
    {
        _direction = direction;
    }

    public void ToggleControls(bool toggle)
    {
        // Disabling the InputActionMap from InputActions doesn't work strangely, so I'm doing it the dirty way
        UnityEngine.InputSystem.PlayerInput input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (input)
        {
            input.enabled = toggle;
        }

        input = _headTransform.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (input)
        {
            input.enabled = toggle;
        }
    }

    #region Inputs
    private void OnMove(InputValue value)
    {
        Move(value.Get<Vector2>());
    }
    #endregion
}
