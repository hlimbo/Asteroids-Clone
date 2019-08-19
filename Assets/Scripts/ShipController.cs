using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public enum RotationOrientation
{
    CLOCKWISE,
    COUNTERCLOCKWISE
}

// Script Responsibilities
/*
 *  1. Rotating the ship
 *  2. Moving Ship forward based on its current orientation
 *  3. Shooting projectiles
 *  4. Determining when its destroyed by something and updating PlayerLifeManager its current life count
 *  6. Manages its own state (e.g. when to fire a bullet, when its invinicible, etc)
 */

public class ShipController : MonoBehaviour
{
    public float steerSpeed;
    public float moveSpeed;
    public float rotationAngle;
    public float eulerAngle;
    public float firingDelayInSeconds;
    public float lastBulletFiredInSeconds;
    public float bulletForce;
    public bool hasFiredBullet = false;
    [SerializeField] private bool isInvincible = false;
    public float invincibilityTime = 3f;
    public GameObject bulletPrefab;

    Rigidbody rb;
    PlayerLifeManager playerLifeManager;

    // Start is called before the first frame update
    void Start()
    {
        rotationAngle = gameObject.transform.rotation.eulerAngles.z;
        rb = GetComponent<Rigidbody>();
        playerLifeManager = FindObjectOfType<PlayerLifeManager>();
    }

    IEnumerator DestroyAndRespawnShip()
    {
        isInvincible = true;
        float before = Time.time;
        while (Time.time - before < invincibilityTime)
        {
            GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
            yield return new WaitForSeconds(0.25f);
        }
        GetComponent<Renderer>().enabled = true;
        isInvincible = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(!isInvincible && other.CompareTag("DestroyShip"))
        {
            playerLifeManager.DecrementNumberOfLives();
            if (playerLifeManager.IsGameOver())
            {
                // add in a special effect here as well .....
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(DestroyAndRespawnShip());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") < 0f)
            rotationAngle = Rotate(RotationOrientation.CLOCKWISE, steerSpeed);
        else if (Input.GetAxisRaw("Horizontal") > 0f)
            rotationAngle = Rotate(RotationOrientation.COUNTERCLOCKWISE, steerSpeed);

        if(Input.GetAxisRaw("Vertical") > 0f)
            Move();

        if(Input.GetButtonDown("Jump") && !hasFiredBullet)
        {
            // Shoot
            lastBulletFiredInSeconds = Time.time;
            hasFiredBullet = true;
            GameObject bulletCopy = Instantiate(bulletPrefab, transform.position + transform.up, Quaternion.identity);
            bulletCopy.GetComponent<Destructable>().owner = "player";
            bulletCopy.GetComponent<Rigidbody>().AddForce(transform.up * bulletForce * Time.fixedDeltaTime, ForceMode.Impulse);
        }

        if(hasFiredBullet && Time.time - lastBulletFiredInSeconds > firingDelayInSeconds)
        {
            hasFiredBullet = false;
        }
            
        //Debug.DrawRay(transform.position, transform.up * moveSpeed, Color.red, Time.deltaTime);
        eulerAngle = gameObject.transform.rotation.eulerAngles.z;
    }

    public float Rotate(RotationOrientation direction, float steerSpeed)
    {
        float rotationDelta = direction == RotationOrientation.CLOCKWISE ?
            steerSpeed * Time.fixedDeltaTime :
            -steerSpeed * Time.fixedDeltaTime;

        float newRotationAngle = (rotationAngle + rotationDelta) % 360f;

        // rotation as an euler angle range between 0-360 degrees only
        if(rotationAngle + rotationDelta < 0f)
            newRotationAngle = 360f + newRotationAngle;

        // transform.Rotate(transform.forward * rotationDelta);
        Quaternion deltaRotation = Quaternion.Euler(transform.forward * rotationDelta);
        rb.MoveRotation(rb.rotation * deltaRotation);
        return newRotationAngle;
    }
    
    public Vector3 Move()
    {
        rb.AddForce(transform.up * moveSpeed * Time.fixedDeltaTime);
        return rb.position;
    }
}
