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
    private readonly string finalCountry = "Morocco";

    private string planeActivator = "";

    private void Awake()
    {

        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        audioController = GetComponent<AudioController>();
        timerController = GetComponent<TimerController>();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Debug.Log("Unity script is awoken");
        mazeOverlay.gameObject.SetActive(false);
        // Create prefabs
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
                HandleFoundMap(addedImage);
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
                HandleFoundMap(updatedImage);
            }
            else if (monarchs.Contains(name))
            {
                UpdateTemporaryObject(updatedImage);
            }
            else if (name == "cookie")
            {
                UpdateCookieObject(updatedImage);
            }
            else
            {
                UpdateImage(updatedImage);
            }

        }

        // for each tracked image that has been removed  
        foreach (var removedImage in args.removed)
        {
            if (spawnedPrefabs.ContainsKey(removedImage.name)) {
                spawnedPrefabs[removedImage.name].SetActive(false);
            }
        }
    }

    /**
    Handle the direction button pressed
    */
    public void DirectionButtonPressed(string direction)
    {
        Debug.Log("Button " + direction + " pressed");
        // Check if correct direction
        if (direction.Equals(mazeSolution[mazeDirectionIndex]))
        {

            mazeDirectionString += direction + " ";
            if (mazeDirectionIndex == mazeSolution.Count - 1)
            {
                // All directions entered, activate maze image recognition
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
            // Entered wrong direction, reset everything
            mazeDirectionIndex = 0;
            mazeDirectionString = "";
        }
        mazeEnteredDirections.text = mazeDirectionString;

    }

    /**
        Hide the direction overlay
    */
    public void CloseButtonPressed()
    {
        Debug.Log("Close pressed");
        mazeOverlay.gameObject.SetActive(false);
        mazeDirectionIndex = 0;
        mazeDirectionString = "";
    }

    /**
    Handle the atlas pictures
    */
    private void HandleFoundMap(ARTrackedImage addedImage)
    {
        // Get the recognized country
        string foundCountry = addedImage.referenceImage.name;

        // If tracking the country
        if (addedImage.trackingState == TrackingState.Tracking)
        {
            // If found the correct country
            if (foundCountry == currentCountry)
            {
                Debug.Log("Added " + currentCountry);

                countries_visted.Add(foundCountry);
                // Check if found the final country
                if (foundCountry != finalCountry)
                {
                    int counter = Array.IndexOf(maps, foundCountry);
                    currentCountry = maps[counter + 1];
                }
                else
                {
                    currentCountry = "finished";
                }
            }
            // Show plane above old countries as well
            if (countries_visted.Contains(foundCountry))
            {
                // Show end portal
                if (foundCountry == finalCountry)
                {
                    Debug.Log("Spawning portal: " + addedImage.referenceImage.name);
                    UpdatePortal(addedImage);
                }
                else
                {
                    Debug.Log("Spawning plane: " + addedImage.referenceImage.name);
                    SpawnPrefab("Plane", addedImage.transform.position, addedImage.transform.rotation);
                    planeActivator = foundCountry;
                    audioController.StartAudio(AudioPlayer.Plane);
                }
            }
        }
        else
        {
            // Deactivate the plane if it is spawned by current country
            if (foundCountry == planeActivator && spawnedPrefabs["Plane"].activeSelf)
            {
                spawnedPrefabs["Plane"].SetActive(false);
            }
        }
    }


    /**
        Update the tracked image
    */
    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        Debug.Log("Found " + name);
        Vector3 position = trackedImage.transform.position;
        SpawnPrefab(name, position, trackedImage.transform.rotation);
    }

    /**
    Spawn the end portal
    */
    private void UpdatePortal(ARTrackedImage trackedImage)
    {


        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            GameObject spawnedObject = spawnedPrefabs["portal"];
            Debug.Log("Showing " + name);
            TextMeshPro text = spawnedObject.transform.Find("FinalRoom/room/TakenTime").GetComponent<TextMeshPro>();
            text.text = "It took you " + timerController.GetTimeTaken();
            spawnedObject.SetActive(true);
            spawnedObject.transform.position = trackedImage.transform.position;
            spawnedObject.transform.rotation = trackedImage.transform.localRotation;
            timerController.StopTicking();
            audioController.StopAudio(AudioPlayer.Police);
        }


    }

    /**
    Spawn the prefab with given name
    */
    private void SpawnPrefab(string name, Vector3 position, Quaternion rotation)
    {

        GameObject prefab = spawnedPrefabs[name];
        prefab.transform.position = position;
        prefab.transform.rotation = rotation;
        prefab.SetActive(true);

    }
    /**
    Handles Morse code and chicken audio
    */
    private void UpdateSound(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState != TrackingState.Tracking)
        {
            // Stop the audio
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
            // Start the audio
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

    /**
    Handle pictures that show only one prefab on them
    */
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

    /**
    Handle cookie picture recognition, shows either cookie or piece of paper
    */
    private void UpdateCookieObject(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;

        Vector3 position = trackedImage.transform.position;
        GameObject spawnedObject;
        // Select to show cookie or the fortune inside the cookie
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

    /**
    Check if cookie was clicked
    */
    void Update()
    {
        if (!cookieEaten && spawnedPrefabs["cookie"].activeSelf)
        {
            // Eat the cookie if cookie active
            if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
            {
                cookieEaten = true;
                audioController.StartAudio(AudioPlayer.Cookie);
                spawnedPrefabs["cookie"].SetActive(false);
            }
        }
    }

    /**
    Show and hide the game time timer
    */
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
