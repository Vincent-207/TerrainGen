using System.Net;
using UnityEngine;

public class JourneyTracker : MonoBehaviour
{
    public bool isInProgress;
    [SerializeField] float timeTraveled, weightCarried, totalCollisionForces;
    public float collisionThreshold;
    public static JourneyTracker instance;
    AudioSource audioSource;
    [SerializeField]
    AudioClip collisionSFX;
    public void EnableJourney()
    {
        timeTraveled = 0;
        isInProgress = true;
    }
    public void DisableJourney()
    {
        isInProgress = false;
    }

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        float collisionForce = collision.impulse.magnitude;
        if(collisionForce > collisionThreshold)
        {
            totalCollisionForces += collisionForce - collisionThreshold;
            audioSource.PlayOneShot(collisionSFX);
        }
    }
    void Update()
    {
        if(isInProgress) timeTraveled += Time.deltaTime;

    }
    public float GetTime()
    {
        return timeTraveled;    
    }

    public float GetWeight()
    {
        return weightCarried;
    }

    public float GetCollisions()
    {
        return totalCollisionForces;
    }

    public void SetWeight(float input)
    {
        weightCarried = input;
    }
}
