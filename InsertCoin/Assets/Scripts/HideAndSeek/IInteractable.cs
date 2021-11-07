using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Interaction
{
    public HeroController heroController;
    public float distance;
}

public interface IInteractable
{
    public void OnInteraction(Interaction interaction);
}
