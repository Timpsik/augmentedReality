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
        foreach(GameObject prefab in placablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            spawnedPrefabs.Add(prefab.name, newPrefab);
            newPrefab.SetActive(false);
        }
    }

    public void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    public void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
        audioSource.Stop();
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {

        // for each tracked image that has been added
        foreach (ARTrackedImage addedImage in args.added)
        {

                
                
          switch (addedImage.referenceImage.name)
      {
          case "phone":
              UpdateSound(addedImage);
              break;
          case "Maze":
          case "Caesar":
              UpdateTemporaryObject(addedImage);
              break;
          default:
              UpdateImage(addedImage);
              break;
      }

        }

        // for each tracked image that has been updated
        foreach (var updatedImage in args.updated)
        {
            switch (updatedImage.referenceImage.name)
      {
          case "phone":
              UpdateSound(updatedImage);
              break;
          case "Maze":
          case "Caesar":
              UpdateTemporaryObject(updatedImage);
              break;
          default:
              UpdateImage(updatedImage);
              break;
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

        private void UpdateTemporaryObject(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        
        Vector3 position = trackedImage.transform.position;

        GameObject spawnedObject = spawnedPrefabs[name];
        
        //if tracked image tracking state is comparable to tracking
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            //set the image tracked ar object to active 
            spawnedObject.SetActive(true);
            spawnedObject.transform.position = trackedImage.transform.position;
            spawnedObject.transform.rotation = trackedImage.transform.localRotation;
        }
        else //if tracked image tracking state is limited or none 
        {
            spawnedObject.SetActive(false);
        }
    }

}
