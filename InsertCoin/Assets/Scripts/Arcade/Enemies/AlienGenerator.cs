using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienGenerator : MonoBehaviour
{
    [SerializeField]
    private ArcadeAlien _alienPrefab;

    [SerializeField]
    private AlienBullet _alienBulletPrefab;

    [SerializeField]
    private Transform _targetTransform;

    [SerializeField]
    private Vector2 _boxSize;

    [SerializeField]
    [Range(0.25f, 5f)]
    private float _spawnFrequency;

    [SerializeField]
    [Range(1,5)]
    private int _maxAliensSpawnCount;

    [SerializeField]
    private int _maxAliensCount;

    private PoolObject<ArcadeAlien> _alienPool;
    private PoolObject<AlienBullet> _alienBulletPool;

    private List<ArcadeAlien> _spawnedAliens;

    private float _lastSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        _spawnedAliens = new List<ArcadeAlien>();

        _alienBulletPool = new PoolObject<AlienBullet>(_alienBulletPrefab);
        _alienPool = new PoolObject<ArcadeAlien>(_alienPrefab);
        _alienPool.SetOnPop(alien => {
            alien.transform.parent = transform.parent;
            alien.Spawn(GetRandomPositionInBox());
            alien.transform.localScale = Vector3.one;
            alien.Target = _targetTransform;
            alien.AlienPool = _alienPool;
            alien.BulletPool = _alienBulletPool;

            _spawnedAliens.Add(alien);
        });

        _alienPool.SetOnPush(alien =>
        {
            _spawnedAliens.Remove(alien);
        });

        _lastSpawnTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= _lastSpawnTime + _spawnFrequency)
        {
            int spawnCount = Random.Range(1, _maxAliensSpawnCount + 1);
            spawnCount = Mathf.Min(spawnCount, _maxAliensCount - _spawnedAliens.Count);
            for (int i = 0; i < spawnCount; ++i)
            {
                _alienPool.Pop();
            }

            _lastSpawnTime = Time.time;
        }
    }

    public Vector3 GetRandomPositionInBox()
    {
        Vector3 startPos = transform.position - (Vector3)(_boxSize / 2f);
        Vector2 randomPosition = new Vector2(_boxSize.x * Random.Range(0f, 1f), _boxSize.y * Random.Range(0f, 1f));
        return startPos + (Vector3) randomPosition;
    }

    public void DestroyAllAliens()
    {
        foreach(ArcadeAlien alien in _spawnedAliens)
        {
            alien.Destroy(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _boxSize);
    }
}
