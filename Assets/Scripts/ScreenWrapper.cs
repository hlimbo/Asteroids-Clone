using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Resonsible for keeping game objects in camera view by teleporting game objects to the 
// opposite edge of the screen when game objects move out of one of the edges of the screen.
public class ScreenWrapper : MonoBehaviour
{
    public List<GameObject> objectsToTrack;
    [Tooltip("Viewport Coordinate Offset. Adjust this value if you want a game object to teleport to the opposite edge of the camera when it is further out of the camera viewport")]
    public float offsetTolerance;
    private Camera gameCam;
    // camera points
    private static Vector3 BOTTOM_LEFT = Vector3.zero;
    private static Vector3 TOP_RIGHT = new Vector3(1f, 1f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        gameCam = Camera.main;
        Debug.Log($"gameCam: {gameCam}");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var gameObject in objectsToTrack)
        {
            if (gameObject == null) continue;

            Vector3 viewportPosition = gameCam.WorldToViewportPoint(gameObject.transform.position);
            if (viewportPosition.x > TOP_RIGHT.x + offsetTolerance)
            {
                viewportPosition.x = 0f;
                gameObject.transform.position = gameCam.ViewportToWorldPoint(viewportPosition);
            }
            else if (viewportPosition.x < BOTTOM_LEFT.x - offsetTolerance)
            {
                viewportPosition.x = 1f;
                gameObject.transform.position = gameCam.ViewportToWorldPoint(viewportPosition);
            }

            if (viewportPosition.y > TOP_RIGHT.y + offsetTolerance)
            {
                viewportPosition.y = 0f;
                gameObject.transform.position = gameCam.ViewportToWorldPoint(viewportPosition);
            }
            else if (viewportPosition.y < BOTTOM_LEFT.y - offsetTolerance)
            {
                viewportPosition.y = 1f;
                gameObject.transform.position = gameCam.ViewportToWorldPoint(viewportPosition);
            }
        }
    }
}
