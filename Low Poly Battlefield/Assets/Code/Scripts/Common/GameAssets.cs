using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
  private static GameAssets _instance;

  public static GameAssets Instance
  {
    get
    {
      if (_instance == null) _instance = Instantiate(Resources.Load<GameAssets>("GameAssets"));
      return _instance;
    }
  }

  [Header("Audio")]
  public AmbianteSoundAudioClip[] ambianteSoundAudioClips;
  public SoundAudioClip[] soundAudioClips;
  public WeaponSoundAudioClip[] weaponSoundAudioClips;

  [System.Serializable]
  public class AmbianteSoundAudioClip
  {
    public string clipName;
    public SoundManager.AmbianteSound sound;
    public AudioClip audioClip;
  }

  [System.Serializable]
  public class SoundAudioClip
  {
    public string clipName;
    public SoundManager.Sound sound;
    public AudioClip audioClip;
  }

  [System.Serializable]
  public class WeaponSoundAudioClip
  {
    public string audioName;
    public WeaponProperties weapon;
    public SoundManager.WeaponSound sound;
    public AudioClip audioClip;
  }

  private void Start()
  {
    SoundManager.PlayAmbianteSound(SoundManager.AmbianteSound.Forest, 0.25f);
  }
}
