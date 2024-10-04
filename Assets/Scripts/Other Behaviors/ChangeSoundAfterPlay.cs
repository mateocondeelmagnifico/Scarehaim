using UnityEngine;

public class ChangeSoundAfterPlay : MonoBehaviour
{
    [SerializeField] private AudioSource mySource;
    [SerializeField] private AudioClip myClip;
    private float playDuration;

    //This script waits for a sound to finish and then replaces it with another

    private void Start()
    {
        playDuration = mySource.clip.length -0.15f;
    }

    private void Update()
    {
        if (mySource.time >= playDuration)
        {
            mySource.clip = myClip;
            Destroy(this);
        }
    }
}
