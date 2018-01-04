using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMan : MonoBehaviour {

    public AudioSource SoundEffectSource;
    public AudioClip SelectSound;
    public AudioClip DeselectSound;
    public AudioClip ExpandSound;
    public AudioClip PlaceSound;
    public AudioClip onePointSound;
    public AudioClip errorSound;

    public void PlaySelect()
    {
        if (SoundEffectSource.isPlaying)
            SoundEffectSource.Stop();
        SoundEffectSource.clip = SelectSound;
        SoundEffectSource.Play();
    }

    public void PlayDeselect()
    {
        if (SoundEffectSource.isPlaying)
            SoundEffectSource.Stop();
        SoundEffectSource.clip = DeselectSound;
        SoundEffectSource.Play();
    }

    public void PlayExpand()
    {
        if (SoundEffectSource.isPlaying)
            SoundEffectSource.Stop();
        SoundEffectSource.clip = ExpandSound;
        SoundEffectSource.Play();
    }

    public void PlayPlace()
    {
        if (SoundEffectSource.isPlaying)
            SoundEffectSource.Stop();
        SoundEffectSource.clip = PlaceSound;
        SoundEffectSource.Play();
    }


    public void PlayOnePoint()
    {
        if (SoundEffectSource.isPlaying)
            SoundEffectSource.Stop();
        SoundEffectSource.clip = onePointSound;
        SoundEffectSource.Play();
    }

    public void ErrorSound()
    {
        if (SoundEffectSource.isPlaying)
            SoundEffectSource.Stop();
        SoundEffectSource.clip = errorSound;
        SoundEffectSource.Play();
    }
}
