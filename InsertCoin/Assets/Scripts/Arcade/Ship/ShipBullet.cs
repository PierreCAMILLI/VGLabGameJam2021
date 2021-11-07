using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBullet : MonoBehaviour, IPopPool, IPushPool
{
    [SerializeField]
    private float _maxSpeed;
    public float MaxSpeed { get { return _maxSpeed; } set { _maxSpeed = value; } }

    public Pool<ShipBullet> Pool { get; set; }

    public void OnPop()
    {
        gameObject.SetActive(true);
    }

    public void OnPush()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * _maxSpeed * Time.deltaTime;
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
        ArcadeAlien alien = collision.GetComponent<ArcadeAlien>();
        if (alien)
        {
            alien.Destroy();
            Destroy();
        }
    }
}
