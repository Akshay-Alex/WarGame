using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider bar;
    void LookAtCamera()
    {
        Camera camera = Camera.main;
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);

    }
    /// <summary>
    /// 0 is minimum possible health and 1 is maximum
    /// </summary>
    public void UpdateHealthBar(float normalizedHealth)
    {
        bar.value = normalizedHealth;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LookAtCamera();
    }
}
