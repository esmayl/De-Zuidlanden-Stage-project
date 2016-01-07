using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour {

    AudioSource[] source;
    public AudioClip mainMusic;
    public AudioClip[] ambients = new AudioClip[3];
    public GUIStyle audioButtonStyle;
    public bool soundOn = true;
    Rect position;

	void Start () {
        if (!transform.gameObject.GetComponent<AudioSource>()) { transform.gameObject.AddComponent<AudioSource>(); }
        source = transform.GetComponents<AudioSource>();
        source[0].clip = mainMusic;
        source[0].Play();
        source[1].volume = 0.1f;

        
        position = new Rect(Screen.width - 66, 10, 64, 64);
	}
	
	void Update () 
    {
        if (soundOn)
        {
            audioButtonStyle.normal = audioButtonStyle.onActive;

            if (!source[0].isPlaying) { source[0].Play();}
            if (Random.Range(0, 100) >= 80 )
            {
                if (!source[1].isPlaying)
                {
                    source[1].volume = 0.01f;
                    source[1].clip = ambients[Random.Range((int)0, (int)2)];
                    source[1].Play();
                }
            }
        }
        if (!soundOn)
        {
            if (source[0].isPlaying) { source[0].Pause(); }
            if (source[1].isPlaying) { source[1].Pause(); }
            audioButtonStyle.normal = audioButtonStyle.onNormal;
        }
	
	}

    void OnGUI()
    {
       if(GUI.Button(position,"", audioButtonStyle)){
           soundOn = !soundOn;
       }
    }
}
