using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTrackingScript : MonoBehaviour
{
    [SerializeField]
    private GameObject[] placablePrefabs;
    private ARTrackedImageManager _arTrackedImageManager;
    private AudioSource audioSource;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    private bool audioPlaying;

    private void Awake()
    {
        audioSource = FindObjectOfType<AudioSource>();
        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        Debug.Log( "Unity script is awoken");
        //audioSource.Play();
        foreach(GameObject prefab in placablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            spawnedPrefabs.Add(prefab.name, newPrefab);
        }
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
        foreach (ARTrackedImage addedImage in args.added)
        {
            Debug.Log("Image added");
            Debug.Log(addedImage.referenceImage.name);
            if (addedImage.referenceImage.name != "phone")
            {
                UpdateImage(addedImage);
            }
            else
            {
                audioSource.Play();
                audioPlaying = true;
            }
          

        }

        // for each tracked image that has been updated
        foreach (var updatedImage in args.updated)
        {
            if (updatedImage.referenceImage.name != "phone")
            {
                UpdateImage(updatedImage);
            }
            else
            {
                UpdateSound(updatedImage);
            }
               
        }

        // for each tracked image that has been removed  
        foreach (var removedImage in args.removed)
        {
            spawnedPrefabs[removedImage.name].SetActive(false);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        Vector3 position = trackedImage.transform.position;

        GameObject prefab = spawnedPrefabs[name];
        prefab.transform.position = position;
        prefab.SetActive(true);


    }

    private void UpdateSound(ARTrackedImage trackedImage)
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
