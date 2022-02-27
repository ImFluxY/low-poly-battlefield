using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnAudioClipFinished : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    public void Initialize(AudioSource audioSource)
    {
        this.audioSource = audioSource;
    }

    private void Update()
    {
        if(audioSource == null)
            return;

        if (!audioSource.isPlaying)
        {
            audioSource.Stop();
            Destroy(gameObject);
        }
    }
}
