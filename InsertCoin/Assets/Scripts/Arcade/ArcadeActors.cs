using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeActors : MonoBehaviour
{
    [SerializeField]
    private ArcadeShip _arcadeShip;
    public ArcadeShip Ship { get { return _arcadeShip; } }

    [SerializeField]
    private AlienGenerator _alienGenerator;
    public AlienGenerator AlienGenerator { get { return _alienGenerator; } }
}
