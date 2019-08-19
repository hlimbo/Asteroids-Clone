using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Temporary class until I implement object pooling
public class Destructable : MonoBehaviour
{
    [SerializeField] private PlayerLifeManager playerLifeManager;
    public string owner; // player OR UFO
    public bool willDestructOverTime = true;
    public float lifeTimeInSeconds = 3f;
    private void Start()
    {
        StartCoroutine(ExpirationTimer());
        playerLifeManager = FindObjectOfType<PlayerLifeManager>();
    }

    IEnumerator ExpirationTimer()
    {   
        yield return new WaitForSeconds(lifeTimeInSeconds);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("DestroyShip"))
        {
            if(owner.Equals("player"))
            {
                if(other.name.Contains("ufo"))
                {
                    playerLifeManager.AddTwentyFivePoints();
                }
                else if(other.name.Contains("Asteroid"))
                {
                    playerLifeManager.AddTenPoints();
                }
            }
            Destroy(gameObject);
        }
    }
}
