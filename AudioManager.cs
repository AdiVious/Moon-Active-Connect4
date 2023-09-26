
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource SFXSource;

  
    [Header("---- Audio Clips ----")]
    public AudioClip DiskDrop;
    public AudioClip KeyPress;
    public AudioClip Swish;
    public AudioClip Win;


    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

   
    public void sfxVolume(float volume)
    {
        SFXSource.volume = volume;
    }
}


