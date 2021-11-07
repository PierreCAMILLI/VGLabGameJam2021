using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Pickable _pickable;

    public void OnInteraction(Interaction interaction)
    {
        interaction.heroController.HasCoin = true;
        _pickable.Pick(interaction.heroController.PocketTransform);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_pickable.IsGoalReached())
        {
            _pickable.Drop();
            gameObject.SetActive(false);
        }
    }

    public void OnSpawn()
    {
    }
}
