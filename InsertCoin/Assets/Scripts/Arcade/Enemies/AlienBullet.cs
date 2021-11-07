using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBullet : MonoBehaviour, IPopPool, IPushPool
{
    [SerializeField]
    private float _maxSpeed;
    public float MaxSpeed { get { return _maxSpeed; } set { _maxSpeed = value; } }

    public Vector2 Forward { get; set; }

    public PoolObject<AlienBullet> Pool { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)Forward * _maxSpeed * Time.deltaTime;
    }

    public void Destroy()
    {
        Pool.Push(this);
    }

    private void OnBecameInvisible()
    {
        Destroy();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ArcadeShip ship = collision.GetComponent<ArcadeShip>();
        if (ship && ship.Damage())
        {
            Destroy();
        }
    }

    public void OnPush()
    {
        gameObject.SetActive(false);
    }

    public void OnPop()
    {
        gameObject.SetActive(true);
    }
}
