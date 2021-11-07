using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeScreen : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ArcadeGame _arcadeGame;

    public void OnInteraction(Interaction interaction)
    {
        if (interaction.heroController.HasCoin)
        {
            _arcadeGame.AddCredit();
            interaction.heroController.HasCoin = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
