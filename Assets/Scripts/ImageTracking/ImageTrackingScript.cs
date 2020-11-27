using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using TMPro;

public class ImageTrackingScript : MonoBehaviour
{
    [SerializeField]
    private GameObject[] placablePrefabs;

    private ARTrackedImageManager _arTrackedImageManager;

    private AudioController audioController;

    private TimerController timerController;

    [SerializeField]
    private GameObject fortuneCookie;

    [SerializeField]
    private GameObject mazeOverlay;

    public Text mazeEnteredDirections;


    private bool cookieEaten = false;

    private bool safeSpawned = false;

    private bool mazeSolved = false;

    private ArrayList mazeSolution = new ArrayList() {
        "L","D","R","D","L","D","R","D","R","D"
    };

    private string mazeDirectionString = "";
    private int mazeDirectionIndex = 0;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    string[] monarchs = { "Caesar", "ElizabethII", "HenryVII", "JamesVI", "WilliamI" };
    string[] maps = { "Netherlands", "Japan", "SouthAfrica", "Morocco" };

    ArrayList countries_visted = new ArrayList();
    string currentCountry = "Netherlands";

    private void Awake()
    {

        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        audioController = GetComponent<AudioController>();
        timerController = GetComponent<TimerController>();
        Debug.Log("Unity script is awoken");
        mazeOverlay.gameObject.SetActive(false);
        foreach (GameObject prefab in placablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Adding " + prefab.name + " to creatable objects.");
            newPrefab.name = prefab.name;
            spawnedPrefabs.Add(prefab.name, newPrefab);
            newPrefab.SetActive(false);
        }
        fortuneCookie = Instantiate(fortuneCookie, Vector3.zero, Quaternion.identity);
        fortuneCookie.SetActive(false);
    }

    public void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    public void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
        audioController.StopAllAudio();
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {

        // for each tracked image that has been added
        foreach (ARTrackedImage addedImage in args.added)
        {
            name = addedImage.referenceImage.name;
            if (name == "Phone" || name == "Chicken")
            {
                UpdateSound(addedImage);

            }
            else if (name == "Timer")
            {
                UpdateTimerDisplay(addedImage);
            }
            else if (maps.Contains(name))
            {
                CheckMapOrder(addedImage);
            }
            else if (name == "Maze")
            {
                if (mazeSolved)
                {
                    UpdateTemporaryObject(addedImage);
                }
            }
            else if (name == "Lock")
            {
                if (!mazeSolved && addedImage.trackingState == TrackingState.Tracking)
                {
                    mazeOverlay.gameObject.SetActive(true);
                }
            }
            else if (monarchs.Contains(name))
            {
                UpdateTemporaryObject(addedImage);
            }
            else if (name == "cookie")
            {
                UpdateCookieObject(addedImage);
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
            if (name == "Phone" || name == "Chicken")
            {
                UpdateSound(updatedImage);
            }
            else if (name == "Timer")
            {
                UpdateTimerDisplay(updatedImage);
            }
            else if (name == "Maze")
            {
                if (mazeSolved)
                {
                    UpdateTemporaryObject(updatedImage);
                }
            }
            else if (name == "Lock")
            {
                if (!mazeSolved && updatedImage.trackingState == TrackingState.Tracking)
                {
                    mazeOverlay.gameObject.SetActive(true);
                }
            }
            else if (maps.Contains(name))
            {
                CheckMapOrder(updatedImage);
            }
            else if (monarchs.Contains(name))
            {
                UpdateTemporaryObject(updatedImage);
            }
            else if (name == "cookie")
            {
                UpdateCookieObject(updatedImage);
            }
            else if (name == "safe")
            {
                UpdateSafe(updatedImage);
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

    public void DirectionButtonPressed(string direction)
    {
        Debug.Log("Button " + direction + " pressed");
        if (direction.Equals(mazeSolution[mazeDirectionIndex]))
        {

            mazeDirectionString += direction + " ";
            if (mazeDirectionIndex == mazeSolution.Count - 1)
            {
                mazeSolved = true;
                audioController.StartAudio(AudioPlayer.Lock);
                mazeOverlay.gameObject.SetActive(false);
                return;
            }
            else
            {
                mazeDirectionIndex++;
            }
        }
        else
        {
            mazeDirectionIndex = 0;
            mazeDirectionString = "";
        }
        mazeEnteredDirections.text = mazeDirectionString;

    }

    public void CloseButtonPressed()
    {
        Debug.Log("Close pressed");
        mazeOverlay.gameObject.SetActive(false);
        mazeDirectionIndex = 0;
        mazeDirectionString = "";
    }

    private void CheckMapOrder(ARTrackedImage addedImage)
    {
        string foundCountry = addedImage.referenceImage.name;

        if (foundCountry == currentCountry)
        {
            countries_visted.Add(foundCountry);
            if (countries_visted.Count < maps.Length)
            {
                int counter = Array.IndexOf(maps, foundCountry);
                currentCountry = maps[counter + 1];
                audioController.StartAudio(AudioPlayer.Plane);
            }
            else
            {
                audioController.StartAudio(AudioPlayer.Plane);
                UpdateSafe(addedImage);
                currentCountry = "Finished";
                safeSpawned = true;
                timerController.StopTicking();
                audioController.StopAudio(AudioPlayer.Police);
            }
        }
        else if (countries_visted.Contains(foundCountry))
        {
            Debug.Log("Spawning plane: " + addedImage.referenceImage.name);
            SpawnPrefab("Plane", addedImage.transform.position, addedImage.transform.rotation);
        }

    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        Debug.Log("Found " + name);
        Vector3 position = trackedImage.transform.position;
        SpawnPrefab(name, position, trackedImage.transform.rotation);


    }

    private void UpdateSafe(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;


        if (trackedImage.trackingState == TrackingState.Tracking && !safeSpawned)
        {
            GameObject spawnedObject = spawnedPrefabs[name];
            Debug.Log("Showing " + name);
            TextMeshProUGUI text = spawnedObject.transform.Find("FinalRoom/room/TakenTime").GetComponent<TextMeshProUGUI>();
            //text.text = "It took you " + minutesTaken + ":" + secondsTaken;
            spawnedObject.SetActive(true);
            spawnedObject.transform.position = trackedImage.transform.position;
            spawnedObject.transform.rotation = trackedImage.transform.localRotation;
            safeSpawned = true;
            timerController.StopTicking();
            audioController.StopAudio(AudioPlayer.Police);
        }


    }

    private void SpawnPrefab(string name, Vector3 position, Quaternion rotation)
    {

        GameObject prefab = spawnedPrefabs[name];
        prefab.transform.position = position;
        prefab.transform.rotation = rotation;
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
                audioController.StopAudio(AudioPlayer.Morse);
            }
            else
            {
                audioController.StopAudio(AudioPlayer.Chicken);
            }


        }
        else if (trackedImage.trackingState == TrackingState.Tracking)
        {
            if (trackedImage.referenceImage.name == "Phone")
            {
                audioController.StartAudio(AudioPlayer.Morse);
            }
            else
            {
                audioController.StartAudio(AudioPlayer.Chicken);
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
            if (!spawnedObject.activeSelf)
            {
                Debug.Log("Showing " + spawnedObject.name);
            }

            spawnedObject.SetActive(true);
            spawnedObject.transform.position = trackedImage.transform.position;
            spawnedObject.transform.rotation = trackedImage.transform.localRotation;
        }
        else //if tracked image tracking state is limited or none 
        {
            if (spawnedObject.activeSelf)
            {
                Debug.Log("Hiding " + spawnedObject.name);
            }
            spawnedObject.SetActive(false);
        }
    }

    private void UpdateCookieObject(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;

        Vector3 position = trackedImage.transform.position;
        GameObject spawnedObject;
        if (cookieEaten)
        {
            spawnedObject = fortuneCookie;
        }
        else
        {
            spawnedObject = spawnedPrefabs[name];
        }


        //if tracked image tracking state is comparable to tracking
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            //set the image tracked ar object to active 
            if (!spawnedObject.activeSelf)
            {
                Debug.Log("Showing " + spawnedObject.name);
            }
            spawnedObject.SetActive(true);
            spawnedObject.transform.position = trackedImage.transform.position;
            spawnedObject.transform.rotation = trackedImage.transform.localRotation;
        }
        else //if tracked image tracking state is limited or none 
        {
            if (spawnedObject.activeSelf)
            {
                Debug.Log("Hiding " + spawnedObject.name);
            }
            spawnedObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!cookieEaten && spawnedPrefabs["cookie"].activeSelf)
        {
            if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
            {
                cookieEaten = true;
                audioController.StartAudio(AudioPlayer.Cookie);
                spawnedPrefabs["cookie"].SetActive(false);
            }
        }
    }

    private void UpdateTimerDisplay(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState != TrackingState.Tracking)
        {
            // hide the countdown timer
            timerController.HideTimeLeft();

        }
        else if (trackedImage.trackingState == TrackingState.Tracking)
        {
            // show the countdown timer
            timerController.ShowTimeLeft();
        }
    }

}
