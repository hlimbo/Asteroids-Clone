using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// Controls when other UIPanels are visible in the game
public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject playAgainPanel;
    [SerializeField] GameObject playerUIPanel;
    [SerializeField] PlayerLifeManager playerLifeManager;
    [SerializeField] AsteroidSpawner asteroidSpawner;
    [SerializeField] UfoSpawner ufoSpawner;
    [SerializeField] GameObjectCounter gameObjectCounter;
    [SerializeField] bool isOnFirstFrame = true; // hack to make sure player isn't prompted with you win game dialog
    Text gameConditionText;
    public GameObject playerShipPrefab;

    // Start is called before the first frame update
    void Start()
    {
        playAgainPanel = GameObject.Find("PlayAgainPanel");
        playerUIPanel = GameObject.Find("PlayerUIPanel");
        gameConditionText = playAgainPanel.GetComponentsInChildren<Text>()
                        .ToList()
                        .Find(text => text != null && text.gameObject.name.Equals("GameConditionText"));
        
        playerLifeManager = FindObjectOfType<PlayerLifeManager>();
        asteroidSpawner = FindObjectOfType<AsteroidSpawner>();
        gameObjectCounter = FindObjectOfType<GameObjectCounter>();
        playerLifeManager.playerUIPanel = playerUIPanel;
        playAgainPanel.SetActive(false);
        playerUIPanel.SetActive(false);
    }

    IEnumerator CheckForGameOver()
    {
        while(!playerLifeManager.IsGameOver())
        {
            yield return null;
        }
        Debug.Log("Game is over");
        playAgainPanel.SetActive(true);
        gameConditionText.text = "Game Over";
        yield return null;
    }

    IEnumerator CheckForWinState()
    {
        while(true)
        {
            var obstacles = gameObjectCounter.gameObjects.FindAll(obj => obj != null && (obj.GetComponent<Ufo>() != null || obj.GetComponent<Asteroid>() != null));
            if (!isOnFirstFrame && obstacles.Count <= 0)
            {
                Debug.Log("You win!");
                playAgainPanel.SetActive(true);
                gameConditionText.text = "You Win!";
                break;
            }

            if (isOnFirstFrame)
                isOnFirstFrame = false;

            yield return null;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    public void StartGame()
    {
        // 1. Destroy Asteroids
        var asteroids = gameObjectCounter.gameObjects.FindAll(obj => obj != null && obj.GetComponent<Asteroid>() != null);
        asteroids.ForEach(asteroid => Destroy(asteroid));

        // 2. Spawn Ship Controller
        var playerShip = Instantiate(playerShipPrefab, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f)) + new Vector3(0.0f, 0.0f, 10.0f), Quaternion.identity);
        // 3. Spawn Asteroids
        asteroidSpawner.StartCoroutine(asteroidSpawner.SpawnAsteroids());

        // Begin Checking Game State here
        StartCoroutine(CheckForGameOver());
        StartCoroutine(CheckForWinState());
    }
}
