using UnityEngine;
using UnityEngine.SceneManagement;

namespace AaronMeaney.BusStop.Utilities
{
    public class LoadSceneOnStart : MonoBehaviour
    {
        // HACK: Can't access SceneAssets in production builds. Need to refer to the scene by string name.
        // I'll look for a type safe way to do this when working on the full version of this sim.
        [SerializeField]
        public string sceneName;

        void Start()
        {
            if (sceneName != default(string))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("LoadSceneOnStart: Scene not set in inspector!");
            }
        }
    }
}
