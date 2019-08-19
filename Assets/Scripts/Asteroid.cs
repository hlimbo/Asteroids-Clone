using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // TODO: write a AsteroidSpawner class that's responsible for:
    // Creating asteroids
    // setting their initial rotateSpeed
    // setting their initial moveForce
    // setting their initialSize
    // setting their initial location in the game world
    // TODO: move 
    public float rotateSpeed;
    public Vector2 moveForce;
    public Vector2 direction;

    public enum Size
    {
        SMALL,
        MEDIUM,
        LARGE
    };

    public Size size;
    public GameObject asteroidPrefab;
    private Rigidbody rb;

    /*
     * Given asteroid has a negative x force
     * Rotate the asteroid in the y-axis in a counter clockwise direction
     * 
     * Given asteroid has a positive x force
     * Rotate the asteroid in the y-axis in a clockwise direction
     * 
     * Given asteroid has a negative y force
     * Rotate the asteroid in the x-axis in a clockwise direction
     * 
     * Given asteroid has a positive y force
     * Rotate the asteroid in the x-axis in a counter clockwise direction
     */ 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(direction.x * moveForce.x, direction.y * moveForce.y, 0f), ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        // green axis is y
        // red axis is x
        // blue axis is z
        rb.MoveRotation(GetComponent<Rigidbody>().rotation * Quaternion.Euler((transform.up * direction.x * rotateSpeed * Time.deltaTime) + (transform.right * -direction.y * rotateSpeed * Time.deltaTime)));
        rb.MovePosition(new Vector3(rb.position.x, rb.position.y, 0f));
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Projectile"))
        {
            Destruct();
        }
    }

    public void SpawnSmallerAsteroids(float scale, Size size)
    {
        for(int i = 0;i < 2; ++i)
        {
            // want the asteroid clones to spawn away from each other measured from parent asteroid's center point
            Vector3 spawnPosition = (i == 0) ?
                transform.position + transform.up * 2f :
                transform.position - transform.up * 2f;

            var asteroidCopy = Instantiate(asteroidPrefab, spawnPosition, transform.rotation);
            asteroidCopy.transform.localScale = Vector3.one * scale;
            asteroidCopy.GetComponent<Asteroid>().size = size;
            asteroidCopy.SetActive(true);
        }
    }

    public void Destruct()
    {
        switch (size)
        {
            case Size.LARGE:
                SpawnSmallerAsteroids(transform.localScale.x / 2f, Size.MEDIUM);
                break;
            case Size.MEDIUM:
                SpawnSmallerAsteroids(transform.localScale.x / 2f, Size.SMALL);
                break;
            case Size.SMALL:
                break;
        }
        Destroy(gameObject);
    }
}
