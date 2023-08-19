using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxManager : MonoBehaviour
{
    public AudioSource _audioSource;
    public AudioClip _sfxSwordHit;
    public AudioClip _sfxPoof;
    public static FxManager fxManager;
    public GameObject _vfxPoof;
    // Start is called before the first frame update
    void Start()
    {
        fxManager = this;
        _audioSource = GetComponent<AudioSource>();
    }
    public void PlaySFXAudio(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }
    public void PlayVFX(GameObject gameObject,Vector3 position)
    {
        GameObject.Instantiate(gameObject, position, Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
