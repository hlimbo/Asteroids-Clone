using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


// A more involved solution would be to attach a component to every gameobject that invokes an event
// SpawnedGameObject that is responsible for adding total game object count by 1 when game object is created
// and decrementing game object count by 1 when game object is destroyed
public class GameObjectCounter : MonoBehaviour
{
    public List<GameObject> gameObjects;
    [SerializeField] private int gameObjectCount = 0;
    public ScreenWrapper screenWrapper;
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(screenWrapper);
        StartCoroutine(UpdateGameObjectCountInScene());
    }

    IEnumerator UpdateGameObjectCountInScene()
    {
        while(true)
        {
            gameObjects = new List<GameObject>(FindObjectsOfType<GameObject>());
            gameObjectCount = gameObjects.Count;
            // may not be an ideal solution -> issue with this is that an update loop is running
            // in the ScreenWrapper script and may affect the actual amount being checked.
            screenWrapper.objectsToTrack = new List<GameObject>(gameObjects);
            yield return null;
        }
    }

    void OnDestroy()
    {
        StopCoroutine(UpdateGameObjectCountInScene());
    }
}
