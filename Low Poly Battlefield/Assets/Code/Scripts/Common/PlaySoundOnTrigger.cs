using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
  public LayerMask layerMask;
  public bool playSoundAtPosition = true;
  [Range(0,1)]
  public float volume = 0.8f;
  public SoundManager.Sound soundToPlay;

  private void OnTriggerEnter(Collider other)
  {
    if ((layerMask.value & (1 << other.transform.gameObject.layer)) > 0)
    {
      if (playSoundAtPosition) SoundManager.PlaySound(soundToPlay, transform.position, volume);
      else SoundManager.PlaySound(soundToPlay);
    }
  }
}
