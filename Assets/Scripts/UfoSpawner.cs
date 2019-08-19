using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UfoSpawner : MonoBehaviour
{
    public GameObject ufoPrefab;
    public float randomViewportSide;
    public float randomViewportHeight;
    public float ufoSpawnChance = 0.5f;
    public float randomChance;
    public float spawnChanceFrequency = 2f;
    private float[] VIEWPORT_SIDES = new float[2] { 0f, 1f };
    private const float MIN_VIEWPORT_HEIGHT = 0f;
    private const float MAX_VIEWPORT_HEIGHT = 1f;
    [SerializeField] private GameObjectCounter gameObjectCounter;
    [SerializeField] private bool canSpawnUfos = false;
    public int maxSpawnCount = 2;
    [SerializeField] private List<GameObject> ufoReferences = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        gameObjectCounter = FindObjectOfType<GameObjectCounter>();
        StartCoroutine(CheckIfUfoCanSpawn());
    }

    // can't guarantee all asteroids in this list haven't been destroyed
    // since this is only a snapshot of ALL asteroids in a PAST frame
    public List<GameObject> GetAsteroids()
    {
        return gameObjectCounter.gameObjects.FindAll(obj => obj != null && obj.GetComponent<Asteroid>() != null);
    }

    IEnumerator CheckIfUfoCanSpawn()
    {
        while (!canSpawnUfos)
        {
            foreach (var asteroid in GetAsteroids())
            {
                // to ensure we don't check stale asteroid references that may have been destroyed last frame
                if (asteroid == null) continue;

                if (asteroid.GetComponent<Asteroid>().size != Asteroid.Size.LARGE)
                {
                    canSpawnUfos = true;
                    break;
                }
            }

            yield return new WaitForSeconds(0.5f);
        }

        yield return BeginUfoSpawning();
    }

    IEnumerator BeginUfoSpawning()
    {
        while(canSpawnUfos)
        {
            // stop randomly spawning UFOs if no more asteroids are in the level.
            if(GetAsteroids().Count <= 0)
            {
                canSpawnUfos = false;
                break;
            }

            // ensure no stale ufo references are kept
            ufoReferences = ufoReferences.FindAll(ufo => ufo != null);
            if (ufoReferences.Count < maxSpawnCount)
            {
                randomChance = Random.Range(0f, 1f);
                if (randomChance < ufoSpawnChance)
                {
                    ufoReferences.Add(SpawnUfo());
                }
            }

            yield return new WaitForSeconds(spawnChanceFrequency);
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    GameObject SpawnUfo()
    {
        randomViewportSide = VIEWPORT_SIDES[Random.Range(0, 1)];
        randomViewportHeight = Random.Range(MIN_VIEWPORT_HEIGHT, MAX_VIEWPORT_HEIGHT);
        Vector3 worldPosition = Camera.main.ViewportToWorldPoint(new Vector3(randomViewportSide, randomViewportHeight, 0f));
        worldPosition.z = 0f;
        GameObject ufo = Instantiate(ufoPrefab, worldPosition, Quaternion.identity);
        ufo.GetComponent<Ufo>().currentAngle = Random.Range(0f, 360f);
        ufo.GetComponent<Ufo>().moveSpeed = Random.Range(2f, 5f);
        ufo.GetComponent<Ufo>().randomHorizontalDirection = randomViewportSide == VIEWPORT_SIDES[0] ? -1 : 1;
        return ufo;
    }
}
