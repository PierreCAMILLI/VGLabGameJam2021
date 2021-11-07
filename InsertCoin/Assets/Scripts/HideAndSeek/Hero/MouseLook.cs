using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    private HeroController _heroController;
    public HeroController HeroController { get { return _heroController; } }

    [Space]
    [SerializeField]
    private float _mouseSensitivity = 100f;

    [SerializeField]
    private Transform _playerTransform;

    [Space]
    [SerializeField]
    private float _interactionDistance = 50f;

    [SerializeField]
    private LayerMask _interactableLayer;
    [SerializeField]
    private LayerMask _environmentLayer;

    public void RequestInteraction()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(Vector3.one * 0.5f);
        if (Physics.Raycast(ray, out hit, _interactionDistance, _interactableLayer))
        {
            //if (!Physics.Raycast(ray, hit.distance, _environmentLayer))
            //{
                IInteractable interactable = hit.transform.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    Interaction interaction = new Interaction
                    {
                        heroController = _heroController,
                        distance = hit.distance
                    };
                    interactable.OnInteraction(interaction);
                }
            //}
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Ray ray = Camera.main.ViewportPointToRay(Vector3.one * 0.5f);
        Gizmos.DrawRay(ray.origin, ray.direction * _interactionDistance);
    }

    #region Inputs
    private void OnMouseMove(InputValue value)
    {
        Vector2 mouseDirection = value.Get<Vector2>() * _mouseSensitivity * Time.deltaTime;

        float xRotation = transform.rotation.eulerAngles.x;
        xRotation -= mouseDirection.y;
        //xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        _playerTransform.Rotate(_playerTransform.up, mouseDirection.x);
    }

    private void OnMouseAction(InputValue value)
    {
        RequestInteraction();
    }
    #endregion
}
