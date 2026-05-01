using UnityEngine;

public class CarAudio : MonoBehaviour
{

    [SerializeField] float minPitch, maxPitch, pitchScalar;
    Rigidbody rb;
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float pitch = rb.linearVelocity.magnitude * pitchScalar;
        pitch = Mathf.Log10(pitch);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        audioSource.pitch = pitch;
        audioSource.volume = (pitch/10f) - 0.03f;
    }
}
