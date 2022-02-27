using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
  public enum AmbianteSound
  {
    Forest
  }

  public enum Sound
  {
    Footstep,
    ShellCollision,
    MagazinCollision
  }

  public enum WeaponSound
  {
    Shot,
    EmptyShot,
    Mag_In,
    Mag_Out
  }

  private static Dictionary<Sound, float> soundTimerDictionary;

  public static void PlayAmbianteSound(AmbianteSound sound, float volume)
  {
    GameObject soundGameObject = new GameObject("Ambiante Sound");
    AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
    audioSource.clip = GetAudioClip(sound);
    audioSource.loop = true;
    audioSource.volume = volume;
    audioSource.Play();
  }

  public static void PlaySound(Sound sound)
  {
    GameObject soundGameObject = new GameObject("Sound");
    AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
    soundGameObject.AddComponent<DestroyOnAudioClipFinished>().Initialize(audioSource);
    audioSource.PlayOneShot(GetAudioClip(sound));
  }

  public static void PlaySound(Sound sound, Vector3 position, float volume)
  {
    GameObject soundGameObject = new GameObject("Sound");
    soundGameObject.transform.position = position;
    AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
    soundGameObject.AddComponent<DestroyOnAudioClipFinished>().Initialize(audioSource);
    audioSource.clip = GetAudioClip(sound);
    audioSource.spatialBlend = 1f;
    audioSource.volume = volume;
    audioSource.Play();
  }

  public static void PlayWeaponSound(WeaponSound sound, int weaponId)
  {
    foreach (GameAssets.WeaponSoundAudioClip soundAudioClip in GameAssets.Instance.weaponSoundAudioClips)
    {
      if (soundAudioClip.weapon.weaponId == weaponId && soundAudioClip.sound == sound)
      {
        GameObject soundGameObject = new GameObject("Weapon Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        soundGameObject.AddComponent<DestroyOnAudioClipFinished>().Initialize(audioSource);
        audioSource.PlayOneShot(soundAudioClip.audioClip);
      }
    }
  }

  public static void PlayWeaponSound(WeaponSound sound, int weaponId, Vector3 position)
  {
    foreach (GameAssets.WeaponSoundAudioClip soundAudioClip in GameAssets.Instance.weaponSoundAudioClips)
    {
      if (soundAudioClip.weapon.weaponId == weaponId && soundAudioClip.sound == sound)
      {
        GameObject soundGameObject = new GameObject("Weapon Sound");
        soundGameObject.transform.position = position;
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        soundGameObject.AddComponent<DestroyOnAudioClipFinished>().Initialize(audioSource);
        audioSource.PlayOneShot(soundAudioClip.audioClip);
      }
    }
  }

  private static AudioClip GetAudioClip(Sound sound)
  {
    foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.Instance.soundAudioClips)
    {
      if (soundAudioClip.sound == sound)
      {
        return soundAudioClip.audioClip;
      }
    }
    return null;
  }

  private static AudioClip GetAudioClip(AmbianteSound sound)
  {
    foreach (GameAssets.AmbianteSoundAudioClip soundAudioClip in GameAssets.Instance.ambianteSoundAudioClips)
    {
      if (soundAudioClip.sound == sound)
      {
        return soundAudioClip.audioClip;
      }
    }
    return null;
  }

  private static bool CanPlaySound(Sound sound)
  {
    switch (sound)
    {
      default:
        return true;
    }
  }
}
