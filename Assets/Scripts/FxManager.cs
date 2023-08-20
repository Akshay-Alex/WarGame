using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxManager : MonoBehaviour
{
    #region Public properties
    public AudioSource _audioSource;
    public AudioClip _sfxSwordHit;
    public AudioClip _sfxPoof;
    public AudioClip _sfxPlay;
    public AudioClip _sfxClick;
    public static FxManager fxManager;
    public GameObject _vfxPoof;
    #endregion

    #region Public functions
    public void PlaySFXAudio(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }
    public void PlayVFX(GameObject gameObject, Vector3 position)
    {
        GameObject.Instantiate(gameObject, position, Quaternion.identity);
    }
    #endregion

    #region Unity functions
    // Start is called before the first frame update
    void Start()
    {
        fxManager = this;
        _audioSource = GetComponent<AudioSource>();
    }
    #endregion
}
