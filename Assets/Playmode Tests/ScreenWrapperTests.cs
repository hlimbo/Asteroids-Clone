using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ScreenWrapperTests
    {
        private GameObject mainCamPrefab;
        private GameObject shipPrefab;

        private GameObject mainCam;
        private GameObject ship;
        private float errorTolerance = 0.001f;

        [SetUp]
        public void Setup()
        {
            mainCamPrefab = Resources.Load<GameObject>("Prefabs/Test Camera");
            shipPrefab = Resources.Load<GameObject>("Prefabs/Cube");
            mainCam = GameObject.Instantiate(mainCamPrefab, new Vector3(0f, 0f, -10f), Quaternion.identity);
            ship = GameObject.Instantiate(shipPrefab, Vector3.zero, Quaternion.identity);
            mainCam.GetComponent<ScreenWrapper>().objectsToTrack = new List<GameObject>();
            mainCam.GetComponent<ScreenWrapper>().objectsToTrack.Add(ship);
            mainCam.GetComponent<ScreenWrapper>().offsetTolerance = 0f;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(mainCam);
            GameObject.DestroyImmediate(ship);
        }

        [UnityTest]
        public IEnumerator TestShipMovingPastRightEdgeOfCameraView()
        {
            // given game object moves past the right edge of camera view,
            Vector3 shipViewportPosition = mainCam.GetComponent<Camera>().WorldToViewportPoint(ship.transform.position);
            shipViewportPosition.x += 1f;
            Vector3 shipNewWorldPosition = mainCam.GetComponent<Camera>().ViewportToWorldPoint(shipViewportPosition);
            ship.GetComponent<Rigidbody>().MovePosition(shipNewWorldPosition);

            yield return new WaitForSeconds(1.0f);

            // then game object teleports to the left edge of camera view
            Vector3 expectedViewportPosition = Vector3.zero;
            Vector3 actualViewportPosition = mainCam.GetComponent<Camera>().WorldToViewportPoint(ship.transform.position);
            Assert.AreEqual(expectedViewportPosition.x, actualViewportPosition.x, errorTolerance);

            // uncomment to see ship teleport to left edge of screen
            // yield return new WaitForSeconds(3.0f);
        }

        [UnityTest]
        public IEnumerator TestShipMovingPastLeftEdgeOfCameraView()
        {
            // given game object moves past the left edge of camera view
            Vector3 shipViewportPosition = mainCam.GetComponent<Camera>().WorldToViewportPoint(ship.transform.position);
            shipViewportPosition.x -= 1f;
            Vector3 shipNewWorldPosition = mainCam.GetComponent<Camera>().ViewportToWorldPoint(shipViewportPosition);
            ship.GetComponent<Rigidbody>().MovePosition(shipNewWorldPosition);

            yield return new WaitForSeconds(1.0f);

            // then game object teleports to the right edge of camera view
            Vector3 expectedViewportPosition = Vector3.right;
            Vector3 actualViewportPosition = mainCam.GetComponent<Camera>().WorldToViewportPoint(ship.transform.position);
            Assert.AreEqual(expectedViewportPosition.x, actualViewportPosition.x, errorTolerance);
        }

        [UnityTest]
        public IEnumerator TestShipMovingPastTopEdgeOfCameraView()
        {
            // given game object moves past the top edge of camera view
            Vector3 shipViewportPosition = mainCam.GetComponent<Camera>().WorldToViewportPoint(ship.transform.position);
            shipViewportPosition.y += 1f;
            Vector3 shipNewWorldPosition = mainCam.GetComponent<Camera>().ViewportToWorldPoint(shipViewportPosition);
            ship.GetComponent<Rigidbody>().MovePosition(shipNewWorldPosition);

            yield return new WaitForSeconds(1.0f);

            // then game object teleports to the bottom edge of camera view
            Vector3 expectedViewportPosition = Vector3.zero;
            Vector3 actualViewportPosition = mainCam.GetComponent<Camera>().WorldToViewportPoint(ship.transform.position);
            Assert.AreEqual(expectedViewportPosition.y, actualViewportPosition.y, errorTolerance);
        }

        [UnityTest]
        public IEnumerator TestShipMovingPastBottomEdgeOfCameraView()
        {
            // given game object moves past the bottom edge of camera view
            Vector3 shipViewportPosition = mainCam.GetComponent<Camera>().WorldToViewportPoint(ship.transform.position);
            shipViewportPosition.y -= 1f;
            Vector3 shipNewWorldPosition = mainCam.GetComponent<Camera>().ViewportToWorldPoint(shipViewportPosition);
            ship.GetComponent<Rigidbody>().MovePosition(shipNewWorldPosition);

            yield return new WaitForSeconds(1.0f);

            // then game object teleports to the top edge of camera view
            Vector3 expectedViewportPosition = Vector3.up;
            Vector3 actualViewportPosition = mainCam.GetComponent<Camera>().WorldToViewportPoint(ship.transform.position);
            Assert.AreEqual(expectedViewportPosition.y, actualViewportPosition.y, errorTolerance);
        }
    }
}
