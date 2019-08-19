using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Tests
{
    public class AsteroidInteractionTests
    {
        private Scene currentScene;
        private GameObject asteroid;
        [SetUp]
        public void Setup()
        {
            var asteroidPrefab = Resources.Load<GameObject>("Prefabs/Asteroid");
            Assert.IsTrue(asteroidPrefab != null);
            asteroid = GameObject.Instantiate(asteroidPrefab, Vector3.zero, Quaternion.identity);
            asteroid.GetComponent<Asteroid>().size = Asteroid.Size.LARGE;

            currentScene = SceneManager.GetActiveScene();
            // 2 since the Test Runner counts as a gameobject in the scene
            Assert.AreEqual(currentScene.GetRootGameObjects().Length, 2);
        }

        [TearDown]
        public void Teardown()
        {
            var gameObjects = currentScene.GetRootGameObjects()
                .Where(go => !go.name.Contains("Code-based tests runner")).ToList();

            for(int i = 0;i < gameObjects.Count; ++i)
            {
                Debug.Log($"Destroying: {gameObjects[i].name}");
                GameObject.DestroyImmediate(gameObjects[i]);
            }
        }

        [UnityTest]
        public IEnumerator TestAsteroidDestruction()
        {
            asteroid.GetComponent<Asteroid>().Destruct();
            yield return null;
            Assert.IsTrue(asteroid == null);
        }

        [UnityTest]
        public IEnumerator TestAsteroidSplitting()
        {
            var currentScene = SceneManager.GetActiveScene();

            asteroid.GetComponent<Asteroid>().Destruct();

            yield return null;

            // I expect there to be 2 asteroids spawned + test runner game object
            Assert.AreEqual(currentScene.GetRootGameObjects().Length, 3);

            var asteroids = currentScene.GetRootGameObjects()
                .Where(go => go.name.Contains("Asteroid")).ToList();

            foreach(GameObject a in asteroids)
            {
                Assert.AreEqual(a.GetComponent<Asteroid>().size, Asteroid.Size.MEDIUM);
                Assert.IsTrue(a.activeInHierarchy);
            }

        }
    }
}
