using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum Music
    {
        Dark_Atmosphere,

        Count
    }

    public enum Sfx
    {
        Footsteps,
        Handgun_Empty,
        Handgun_Reload,
        Handgun_Shoot,
        Shotgun_Empty,
        Shotgun_Reload,
        Shotgun_Shoot,
        Switch,
        Zombie_Growl,

        Count
    };

    public AudioClip[] MusicAudioClips;
    public AudioClip[] SfxAudioClips;

    public AudioSource MusicAudioSource { get; private set; }
    public AudioSource SfxAudioSource { get; private set; }

    public void Awake()
    {
        // https://docs.unity3d.com/ScriptReference/Resources.html
        MusicAudioClips = Resources.LoadAll<AudioClip>("Audio/Music");
        Debug.Assert((int)Music.Count == MusicAudioClips.Length, "SoundManager : Music enum length (" + (int)Music.Count + ") does not match Resources folder (" + MusicAudioClips.Length + ")");

        SfxAudioClips = Resources.LoadAll<AudioClip>("Audio/SFX");
        Debug.Assert((int)Sfx.Count == SfxAudioClips.Length, "SoundManager : Sfx enum length " + (int)Sfx.Count + ") does not match Resources folder (" + SfxAudioClips.Length + ")");

        // https://docs.unity3d.com/ScriptReference/GameObject.AddComponent.html
        MusicAudioSource = gameObject.AddComponent<AudioSource>();
        MusicAudioSource.loop = true;
        MusicAudioSource.volume = 0.1f;

        SfxAudioSource = gameObject.AddComponent<AudioSource>();
        SfxAudioSource.loop = false;
        SfxAudioSource.volume = 1f;
    }

    public void Play(Music music)
    {
        MusicAudioSource.clip = MusicAudioClips[(int)music];
        MusicAudioSource.Play();
    }

    public void Play(Sfx sfx)
    {
        SfxAudioSource.clip = SfxAudioClips[(int)sfx];
        SfxAudioSource.Play();
        //AudioSource.PlayClipAtPoint(SfxAudioClips[(int)sfx], transform.position);
    }

    public void Stop(Sfx sfx)
    {
        //SfxAudioSource.clip = SfxAudioClips[(int)sfx];
        SfxAudioSource.Stop();
    }
}