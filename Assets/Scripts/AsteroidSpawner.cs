using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


// Should spawn asteroids not on top of player ship
// Should spawn asteroids such that no asteroids overlap each other, no asteroids overlap player ship
// asteroids all spawn within camera viewport

// all asteroids that spawn are the biggest one
// a preset number of big asteroids are spawned at the start of each level
public class AsteroidSpawner : MonoBehaviour
{
    public int initialSpawnCount;
    public GameObject playerShip;
    public GameObject asteroidPrefab;
    public float minMoveForce, maxMoveForce;
    public float minRotationSpeed, maxRotationSpeed;
    [SerializeField] private List<GameObject> asteroids;

    // Start is called before the first frame update
    void Start()
    {
        float magnitude = asteroidPrefab.GetComponent<Renderer>().bounds.extents.sqrMagnitude;
        float radius = asteroidPrefab.GetComponent<SphereCollider>().radius;
        Debug.Log($"magnitude: {magnitude}");
        Debug.Log($"radius: {radius}");
        StartCoroutine(SpawnAsteroidsWithoutPlayerShip());
    }

    public IEnumerator SpawnAsteroids()
    {
        playerShip = FindObjectOfType<ShipController>().gameObject;
        Assert.IsNotNull(playerShip);
        asteroids = SpawnObjects(playerShip != null);
        yield return null;
    }

    IEnumerator SpawnAsteroidsWithoutPlayerShip()
    {
        asteroids = SpawnObjects(playerShip != null);
        yield return null;
    }

    private Vector3 SelectRandomSpawnPosition()
    {
        Vector3 randomViewportPosition = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), 0f);
        Vector3 randomWorldPosition = Camera.main.ViewportToWorldPoint(randomViewportPosition);
        randomWorldPosition.z = 0f;
        return randomWorldPosition;
    }

    private bool CanOverlapWithOtherPosition(Vector3 shipPosition, Vector3 otherPosition, float minDist)
    {
        return (shipPosition - otherPosition).magnitude < minDist;
    }

    // A huge amount of asteroids that initially spawn at the start of the level will cause Unity to hang... haven't tested the upper bounds for it yet
    public List<Vector3> CalculateRandomSpawnPositions(bool isPlayerShipAvailable)
    {
        // Pick n random points that don't overlap each other
        List<Vector3> spawnPositions = new List<Vector3>();
        float before = Time.time;
        while(spawnPositions.Count < initialSpawnCount)
        {
            Vector3 potentialSpawnPosition = SelectRandomSpawnPosition();
            if (!isPlayerShipAvailable)
                spawnPositions.Add(potentialSpawnPosition);
            else if (!CanOverlapWithOtherPosition(playerShip.transform.position, potentialSpawnPosition, asteroidPrefab.GetComponent<Renderer>().bounds.extents.sqrMagnitude))
                spawnPositions.Add(potentialSpawnPosition);
        }

        float duration = Time.time - before;
        Debug.Log($"Duration: {duration} seconds");

        return spawnPositions;
    }

    public List<Vector2> CalculateMoveForces()
    {
        List<Vector2> moveForces = new List<Vector2>();
        for(int i = 0;i < initialSpawnCount; ++i)
        {
            moveForces.Add(new Vector2(Random.Range(minMoveForce, maxMoveForce), Random.Range(minMoveForce, maxMoveForce)));
        }
        return moveForces;
    }

    public List<float> CalculateRotateSpeeds()
    {
        List<float> rotateSpeeds = new List<float>();
        for(int i = 0;i < initialSpawnCount; ++i)
        {
            rotateSpeeds.Add(Random.Range(minRotationSpeed, maxRotationSpeed));
        }
        return rotateSpeeds;
    }

    public List<Vector2> CalculateRandomDirections()
    {
        List<Vector2> directions = new List<Vector2>();
        for(int i = 0;i < initialSpawnCount; ++i)
        {
            directions.Add(new Vector2((int)Mathf.Sign(Random.Range(-1f, 1f)), (int)Mathf.Sign(Random.Range(-1f, 1f))));
        }
        return directions;
    }

    public List<GameObject> SpawnObjects(bool isPlayerShipAvailable)
    {
        List<GameObject> objects = new List<GameObject>();
        List<Vector3> spawnPositions = CalculateRandomSpawnPositions(isPlayerShipAvailable);
        List<Vector2> moveForces = CalculateMoveForces();
        List<float> rotateSpeeds = CalculateRotateSpeeds();
        List<Vector2> directions = CalculateRandomDirections();

        for(int i = 0;i < initialSpawnCount; ++i)
        {
            GameObject asteroid = Instantiate(asteroidPrefab, spawnPositions[i], Quaternion.identity);
            Assert.IsNotNull(asteroid.GetComponent<Asteroid>());
            // Would replacing this with ScriptableObjects be beneficial?
            asteroid.GetComponent<Asteroid>().moveForce = moveForces[i];
            asteroid.GetComponent<Asteroid>().rotateSpeed = rotateSpeeds[i];
            asteroid.GetComponent<Asteroid>().direction = directions[i];
            asteroid.GetComponent<Asteroid>().size = Asteroid.Size.LARGE;
            objects.Add(asteroid);
        }
        return objects;
    }
}
