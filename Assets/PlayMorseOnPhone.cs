using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageRecgonition : MonoBehaviour
{

    private ARTrackedImageManager _arTrackedImageManager;
    private AudioSource audioSource;

    private bool audioPlaying;

    private void Awake()
    {
        audioSource = FindObjectOfType<AudioSource>();
        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        Debug.Log( "Unity script is awoken");
        //audioSource.Play();
    }

    public void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;
        Debug.Log( "Unity script is in onEnable");
        //audioSource.Play();
    }

    public void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
        Debug.Log( "Unity script is in onDisable");
        audioSource.Stop();
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        //Debug.Log( "Image changed");
                // for each tracked image that has been added
        foreach (var addedImage in args.added)
        {
            audioSource.Play();
            Debug.Log( "Image added");
            audioPlaying = true;

        }

        // for each tracked image that has been updated
        foreach (var updated in args.updated)
        {
           // //throw tracked image to check tracking state
         UpdateTrackedObject(updated);
           //Debug.Log( "Image updated");
        }

        // for each tracked image that has been removed  
        foreach (var trackedImage in args.removed)
        {
            Debug.Log( "Image destroyed");
            // destroy the AR object associated with the tracked image
            //Destroy(trackedImage.gameObject);
        }
    }

        private void UpdateTrackedObject(ARTrackedImage trackedImage)
    {
        //if tracked image tracking state is comparable to tracking
        if (trackedImage.trackingState != TrackingState.Tracking)
        {
            //deactivate the image tracked ar object 
            if( audioPlaying) {
            Debug.Log("Stopping audio");
           audioSource.Stop();
           audioPlaying = false;
            }

        } else if (trackedImage.trackingState == TrackingState.Tracking) {
            if(!audioPlaying) {
            Debug.Log("Restarting audio");
           audioSource.Play();
           audioPlaying = true;
            }
        }
    }

}
