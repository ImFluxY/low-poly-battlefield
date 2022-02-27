using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSound : MonoBehaviour
{
    public bool enable;
    public SoundManager.Sound sound;

    private void PlayAnimationSound()
    {
        if(!enable)
            return;

        SoundManager.PlaySound(sound);
    }
}
