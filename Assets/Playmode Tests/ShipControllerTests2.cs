using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ShipControllerTests2
    {
        // Possible Improvement: Use prefabs to test game objects with multiple components attached to them
        // Saves the trouble of having to manually call AddComponent multiple times to create object under test
        private GameObject SpawnShipWithComponents()
        {
            GameObject ship = new GameObject();
            ship.AddComponent<ShipController>();
            ship.AddComponent<Rigidbody>();
            ship.GetComponent<Rigidbody>().useGravity = false;

            return ship;
        }

        private GameObject ship;
        private float delta = 0.0001f;

        [SetUp]
        public void Setup()
        {
            Debug.Log("on Setup");
            ship = SpawnShipWithComponents();
        }

        [TearDown]
        public void Teardown()
        {
            Debug.Log("on Teardown");
            ship = null;
        }


        // Bad test to automate (Just play test it to see how it moves :) )
        // Depends on moveSpeed value and number of frames to pass in game time to see if the ship actually moved or not
        [UnityTest]
        public IEnumerator TestShipMovingForward()
        {
            ship.GetComponent<ShipController>().moveSpeed = 10000f;
            Vector3 oldPosition = ship.GetComponent<ShipController>().Move();
            yield return new WaitForSeconds(1f);
            Vector3 newPosition = ship.GetComponent<Rigidbody>().position;
            Assert.AreNotEqual(oldPosition, newPosition);

        }

        // Note: anything that involves a rigidbody dependency will need to be promoted to a UnityTest
        [UnityTest]
        public IEnumerator TestShipSteeringClockwise()
        {
            float steerSpeed = 100f;
            float newRotationAngle = ship.GetComponent<ShipController>().Rotate(RotationOrientation.CLOCKWISE, steerSpeed);
            yield return null;
            Assert.AreEqual(newRotationAngle, ship.GetComponent<Rigidbody>().rotation.eulerAngles.z, delta);
        }

        [UnityTest]
        public IEnumerator TestShipSteeringCounterClockwise()
        {
            float steerSpeed = 100f;
            float newRotationAngle = ship.GetComponent<ShipController>().Rotate(RotationOrientation.COUNTERCLOCKWISE, steerSpeed);
            yield return null;
            Assert.GreaterOrEqual(newRotationAngle, 0f);
            Assert.AreEqual(newRotationAngle, ship.GetComponent<Rigidbody>().rotation.eulerAngles.z);
        }

        [UnityTest]
        public IEnumerator TestShipSteerOver360Clockwise()
        {
            float steerSpeed = 100f;
            int maxTurnCount = 181;
            for (int i = 0; i < maxTurnCount; ++i)
            {
                ship.GetComponent<ShipController>().rotationAngle = ship.GetComponent<ShipController>().Rotate(RotationOrientation.CLOCKWISE, steerSpeed);
                yield return null;
            }
            Assert.LessOrEqual(ship.GetComponent<ShipController>().rotationAngle, 360f);
        }

        [UnityTest]
        public IEnumerator TestShipSteerLessThan0CounterClockwise()
        {
            float steerSpeed = 100f;
            int maxTurnCount = 181;
            for (int i = 0; i < maxTurnCount; ++i)
            {
                ship.GetComponent<ShipController>().rotationAngle = ship.GetComponent<ShipController>().Rotate(RotationOrientation.COUNTERCLOCKWISE, steerSpeed);
                yield return null;
            }
            Assert.GreaterOrEqual(ship.GetComponent<ShipController>().rotationAngle, 0f);
        }
    }
}
