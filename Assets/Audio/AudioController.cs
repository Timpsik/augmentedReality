using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private AudioSource morseCode;

    [SerializeField]
    private AudioSource chicken;

    [SerializeField]
    private AudioSource policeSiren;

    [SerializeField]
    private AudioSource cookieCrisp;

    [SerializeField]
    private AudioSource directionLock;

    [SerializeField]
    private AudioSource plane;

    public void StartAudio(AudioPlayer audioName)
    {
        AudioSource audio = GetAudioSource(audioName);
        if (!audio.isPlaying)
        {
            Debug.Log("Playing " + audio.name);
            audio.Play();
        }

    }

    public void StopAudio(AudioPlayer audioName)
    {
        AudioSource audio = GetAudioSource(audioName);
        if (audio.isPlaying)
        {
            Debug.Log("Stopping " + audio.name);
            audio.Stop();
        }
    }

    public void ChangeAudioVolume(AudioPlayer audioName, float newVolume)
    {
        AudioSource audio = GetAudioSource(audioName);
        if (audio.volume != newVolume)
        {
            audio.volume = newVolume;
        }

    }

    public void StopAllAudio()
    {
        morseCode.Stop();
        chicken.Stop();
        policeSiren.Stop();
        cookieCrisp.Stop();
        directionLock.Stop();
    }

    private AudioSource GetAudioSource(AudioPlayer audioName)
    {
        if (AudioPlayer.Morse.Equals(audioName))
        {
            return morseCode;
        }
        else if (AudioPlayer.Police.Equals(audioName))
        {
            return policeSiren;
        }
        else if (AudioPlayer.Cookie.Equals(audioName))
        {
            return cookieCrisp;
        }
        else if (AudioPlayer.Lock.Equals(audioName))
        {
            return directionLock;
        }
        else if (AudioPlayer.Chicken.Equals(audioName))
        {
            return chicken;
        }
        else if (AudioPlayer.Plane.Equals(audioName))
        {
            return plane;
        }
        else
        {
            Debug.LogError("Unknown audio requested");
            return null;
        }
    }
}
