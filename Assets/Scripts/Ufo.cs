using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ufo : MonoBehaviour
{
    public GameObject projectilePrefab;
    public bool canFireBullets = true;
    public float startingAngle = 90f;
    public float currentAngle = 90f;
    public float angleDelta = 15f;
    public float bulletForce = 2f;
    public float bulletFrequency = 5f;
    public float bulletSpawnOffset = 1.25f;
    public float maxLifeTimeInSeconds = 56f;
    public float timeSinceFirstSpawned;
    public float moveSpeed = 10f;
    public float diagonalSpeed;
    public float moveVerticallyFrequency = 1f;
    public float moveHorizontallyFrequency = 1f;
    public int randomVerticalDirection = 0;
    public int randomHorizontalDirection = 0;
    private Vector3 moveDelta;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DespawnTimer());
        StartCoroutine(FireBulletsInCircularMotion());
        StartCoroutine(DecideToMoveVerticalRandomly());
        timeSinceFirstSpawned = Time.time;
        rb = GetComponent<Rigidbody>();
        randomHorizontalDirection = (int)Mathf.Sign(Random.Range(-1, 1));
        diagonalSpeed = Mathf.Sqrt(moveSpeed * moveSpeed + moveSpeed * moveSpeed);
        moveDelta = new Vector3(randomHorizontalDirection * moveSpeed, 0f, 0f);
    }

    // fires a projectile in a circular motion at a particular rate
    IEnumerator FireBulletsInCircularMotion()
    {
        while(canFireBullets)
        {
            float angleInRadians = Mathf.Deg2Rad * currentAngle;
            Vector3 spawnDirection = new Vector3(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians), 0f) * bulletSpawnOffset;
            GameObject projectileClone = Instantiate<GameObject>(projectilePrefab, transform.position + spawnDirection, Quaternion.identity);
            projectileClone.GetComponent<Rigidbody>().AddForce(spawnDirection * bulletForce, ForceMode.Impulse);
            yield return new WaitForSeconds(bulletFrequency);
            currentAngle = (currentAngle + angleDelta) % 360f;
        }
        
    }

    // despawns after a specified amount of time has elapsed
    IEnumerator DespawnTimer()
    {
        yield return new WaitForSeconds(maxLifeTimeInSeconds);
        canFireBullets = false;
        StopCoroutine(FireBulletsInCircularMotion());
        StopCoroutine(DecideToMoveVerticalRandomly());
        Destroy(gameObject);
    }

    IEnumerator DecideToMoveVerticalRandomly()
    {
        while(true)
        {
            randomVerticalDirection = (int)Mathf.Sign(Random.Range(-1, 1));
            moveDelta.Set(randomHorizontalDirection * diagonalSpeed, randomVerticalDirection * diagonalSpeed, 0f);
            yield return new WaitForSeconds(moveVerticallyFrequency);
            moveDelta.Set(randomHorizontalDirection * moveSpeed, 0f, 0f);
            yield return new WaitForSeconds(moveHorizontallyFrequency);
        }
    }

    // Update is called once per frame
    void Update()
    {
        rb.MovePosition(rb.position + moveDelta * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Projectile") || other.CompareTag("DestroyShip"))
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}
