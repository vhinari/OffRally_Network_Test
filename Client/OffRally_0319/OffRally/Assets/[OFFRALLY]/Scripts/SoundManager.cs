using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
   
    /* 임시 코드 */
    public AudioClip BGM;   
    
    public AudioSource efxSource;                   //사운드 이펙트
    public AudioSource musicSource;                 //음악
    public static SoundManager instance = null;            
    public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
    public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.
        
        
    void Awake ()
    {
        audio.clip = BGM;
        audio.Play();
        audio.loop = true;

        
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy (gameObject);
            
        DontDestroyOnLoad (gameObject);
    }
        
        
    public void PlaySingle(AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        efxSource.clip = clip;
            
        //Play the clip.
        efxSource.Play ();
    }
    
    public void PlayLoop(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    public void RandomizeSfx (params AudioClip[] clips)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);
            
        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
            
        //Set the pitch of the audio source to the randomly chosen pitch.
        efxSource.pitch = randomPitch;
            
        //Set the clip to the clip at our randomly chosen index.
        efxSource.clip = clips[randomIndex];
            
        //Play the clip.
        efxSource.Play();
    }
}

