using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
  public bool needKinematicToFalse = true;
  public bool playSoundAtPosition = true;
  [Range(0,1)]
  public float volume = .8f;
  public SoundManager.Sound soundToPlay;
  public float minimumCollisionMagnitude = 2;

  void OnCollisionEnter(Collision collision)
  {
    if (GetComponent<Rigidbody>().isKinematic == needKinematicToFalse)
      return;

    if (collision.relativeVelocity.magnitude > minimumCollisionMagnitude)
      if (playSoundAtPosition) SoundManager.PlaySound(soundToPlay, transform.position, volume);
      else SoundManager.PlaySound(soundToPlay);
  }
}
