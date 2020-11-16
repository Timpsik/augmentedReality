using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SwitchMaze : MonoBehaviour
{

    private ARTrackedImageManager _arTrackedImageManager;

    public GameObject visualMaze;

    private void Awake()
    {
        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        Debug.Log("Unity script is awoken");
    }

    public void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;
        Debug.Log("Unity script is in onEnable");
        visualMaze = (GameObject)Instantiate(visualMaze, transform);
            visualMaze.SetActive(false);
    }

    public void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
        Debug.Log("Unity script is in onDisable");
    }

    void ActivateTrackedObject()
    {
        Debug.Log("Visual maze set active");
        visualMaze.SetActive(true);
        // Give the initial image a reasonable default scale
        //visualMaze.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        //Debug.Log( "Image changed");
        // for each tracked image that has been added
        foreach (var addedImage in args.added)
        {
            Debug.Log("Image added");
            ActivateTrackedObject();
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
            Debug.Log("Image destroyed");
            // destroy the AR object associated with the tracked image
            Destroy(visualMaze);
        }
    }

    private void UpdateTrackedObject(ARTrackedImage trackedImage)
    {
                //if tracked image tracking state is comparable to tracking
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            //set the image tracked ar object to active 
            visualMaze.SetActive(true);
            visualMaze.transform.position = trackedImage.transform.position;
            visualMaze.transform.rotation = trackedImage.transform.localRotation;
        }
        else //if tracked image tracking state is limited or none 
        {
            //deactivate the image tracked ar object 
            visualMaze.SetActive(false);
        }
    }

}
