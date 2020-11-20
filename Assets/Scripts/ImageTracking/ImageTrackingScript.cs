using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ImageTrackingScript : MonoBehaviour
{
    [SerializeField]
    private GameObject[] placablePrefabs;

    [SerializeField]
    private TextMeshProUGUI timer;
    private ARTrackedImageManager _arTrackedImageManager;

    [SerializeField]
    private AudioSource morseCode;

    [SerializeField]
    private AudioSource chicken;

    private int minutes = 60;
    private int seconds = 0;
    private bool takingAway = false;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    private bool audioPlaying;
    string[] Monarchs = { "Ceasar", "ElizabethII", "HenryVII", "JamesVI","WilliamI","Maze" };
    string[] maps = { "Netherlands", "Japan","SouthAfrica","Morocco" };
    string currentCountry = "Netherlands";

    private void Awake()
    {
        morseCode = FindObjectOfType<AudioSource>();
        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        Debug.Log("Unity script is awoken");
        timer.enabled = false;
        foreach (GameObject prefab in placablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Adding " + prefab.name + " to creatable objects.");
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
        morseCode.Stop();
        chicken.Stop();
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {

        // for each tracked image that has been added
    foreach (ARTrackedImage addedImage in args.added)
    {
        name = addedImage.referenceImage.name;
            if (name == "Phone")
            {
                UpdateSound(addedImage);
     
            } else if (name == "Timer")
            {
                UpdateTimerDisplay(addedImage);
            } else if (maps.Contains(name)) {
                CheckMapOrder(addedImage);
            } else if (Monarchs.Contains(name)) {
                UpdateTemporaryObject(addedImage);
            }
            else
            {
                UpdateImage(addedImage);
            }

        }

        // for each tracked image that has been updated
    foreach (var updatedImage in args.updated)
    {
            name = updatedImage.referenceImage.name;
            if (name == "Phone")
            {
                UpdateSound(updatedImage);
            }
            else if (name == "Timer")
            {
                UpdateTimerDisplay(updatedImage);
            }
            else if (maps.Contains(name))
            {
                CheckMapOrder(updatedImage);
        } else if (Monarchs.Contains(name))
        {
            UpdateTemporaryObject(updatedImage);
        }
        else
            {
                UpdateImage(updatedImage);
            }

        }

        // for each tracked image that has been removed  
        foreach (var removedImage in args.removed)
        {
            spawnedPrefabs[removedImage.name].SetActive(false);
        }
    }

    private void CheckMapOrder(ARTrackedImage addedImage)
    {
        if (addedImage.referenceImage.name == currentCountry)
        {
            int index = Array.IndexOf(maps, name);
            if (index == (maps.Length - 1))
            {
                SpawnPrefab("Symbol", addedImage.transform.position);
            }
            else {
                SpawnPrefab("Plane", addedImage.transform.position);
                currentCountry = maps[index + 1];
            }
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        Debug.Log("Found");
        Debug.Log(name);
        Vector3 position = trackedImage.transform.position;
        SpawnPrefab(name, position);


    }

    private void SpawnPrefab(string name, Vector3 position)
    {
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
            if (trackedImage.referenceImage.name == "Phone")
            {
                
                if (morseCode.isPlaying)
                {
                    Debug.Log("Stopping morse");
                    morseCode.Stop();
                }
            }
            else
            {
                
                if (chicken.isPlaying)
                {
                    Debug.Log("Stopping chicken");
                    chicken.Stop();
                }
            }


        }
        else if (trackedImage.trackingState == TrackingState.Tracking)
        {
            if (trackedImage.referenceImage.name == "Phone")
            {
                Debug.Log("Recognized morse");
                if (!morseCode.isPlaying)
                {
                    Debug.Log("Playing morse");
                    morseCode.Play();
                }
            }
            else
            {
                Debug.Log("Recognized chicken");
                if (!chicken.isPlaying)
                {
                    Debug.Log("Playing chicken");
                    chicken.Play();
                }
            }
        }
    }

    private void UpdateTemporaryObject(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;

        Vector3 position = trackedImage.transform.position;
        Debug.Log(name);
        GameObject spawnedObject = spawnedPrefabs[name];
        Debug.Log(spawnedObject.name);
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


    IEnumerator TimerTake()
    {
        takingAway = true;
        yield return new WaitForSecondsRealtime(1);
        if (seconds != 0)
        {
            seconds -= 1;
        }
        else if (minutes > 0)
        {
            minutes -= 1;
            seconds = 59;
        }
        if (seconds == 0 && minutes == 0)
        {
            timer.text = "You died";
            timer.fontSize = 100;
            timer.enabled = true;
        }
        else
        {
            timer.text = GetTimeString(minutes) + ":" + GetTimeString(seconds);
            takingAway = false;
        }
    }

    private string GetTimeString(int time)
    {
        if (time < 10)
        {
            return "0" + time;
        }
        return "" + time;
    }

    void Update()
    {
        if (takingAway == false && (seconds > 0 || minutes > 0))
        {
            StartCoroutine(TimerTake());
        }
    }

    private void UpdateTimerDisplay(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState != TrackingState.Tracking)
        {
            // hide the countdown timer
            if (seconds > 0 || minutes > 0)
            {
                timer.enabled = false;
            }

        }
        else if (trackedImage.trackingState == TrackingState.Tracking)
        {
            // show the countdown timer
            timer.enabled = true;
        }
    }

}
