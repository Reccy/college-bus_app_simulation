using UnityEngine;

// Prevents the destruction of this Game Object on load
public class DontDestroyOnLoad : MonoBehaviour {

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
