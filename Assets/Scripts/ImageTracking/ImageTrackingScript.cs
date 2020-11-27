﻿using System;
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

    [SerializeField]
    private Camera arCamera; 
    private ARTrackedImageManager _arTrackedImageManager;

    [SerializeField]
    private AudioSource morseCode;

    [SerializeField]
    private AudioSource chicken;

        [SerializeField]
    private AudioSource policeSiren;

            [SerializeField]
    private AudioSource cookieCrisp;

        [SerializeField]
    private GameObject fortuneCookie;

    private int minutes = 60;
    private int seconds = 0;

    private int minutesTaken = 0;

    private int secondsTaken = 0;
    private bool takingAway = false;

    private bool addingTime = false;

    private bool cookieEaten = false;
    private bool cookieActive = false;

    private bool safeSpawned = true;

    private bool timeTicking = true;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    string[] monarchs = { "Caesar", "ElizabethII", "HenryVII", "JamesVI", "WilliamI", "Maze"};
    string[] maps = { "Netherlands", "Japan", "SouthAfrica", "Morocco" };
    string currentCountry = "Netherlands";

    private void Awake()
    {

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
        morseCode.Stop();
        chicken.Stop();
        policeSiren.Stop();
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
            else if (name == "safe") {
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

    private void CheckMapOrder(ARTrackedImage addedImage)
    {
        
     /*   if (addedImage.referenceImage.name == currentCountry)
        {
            Debug.Log("I'm in country: " + addedImage.referenceImage.name);
            int index = Array.IndexOf(maps, name);
            if (index == (maps.Length - 1))
            {
                SpawnPrefab("Symbol", addedImage.transform.position,  addedImage.transform.rotation);
            }
            else
            {
                Debug.Log("Spawning plane: " + addedImage.referenceImage.name);
                SpawnPrefab("Plane", addedImage.transform.position, addedImage.transform.rotation);
                currentCountry = maps[index + 1];
            }
        }*/
        SpawnPrefab("Plane", addedImage.transform.position, addedImage.transform.rotation);
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
            text.text = "It took you " + minutesTaken + ":" + secondsTaken;
            spawnedObject.SetActive(true);
            spawnedObject.transform.position = trackedImage.transform.position;
            spawnedObject.transform.rotation = trackedImage.transform.localRotation;
            safeSpawned = true;
            timeTicking = false;
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
                if (!morseCode.isPlaying)
                {
                    Debug.Log("Playing morse");
                    morseCode.Play();
                }
            }
            else
            {
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
        GameObject spawnedObject = spawnedPrefabs[name];
        
        //if tracked image tracking state is comparable to tracking
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            //set the image tracked ar object to active 
            if (!spawnedObject.activeSelf) {
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
        if (cookieEaten) {
            spawnedObject = fortuneCookie;
        } else {
            spawnedObject = spawnedPrefabs[name];
        }
        
        
        //if tracked image tracking state is comparable to tracking
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            //set the image tracked ar object to active 
            if (!spawnedObject.activeSelf) {
                Debug.Log("Showing " + spawnedObject.name);
            }
            cookieActive = true;
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
            cookieActive = false;
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
            TextAlert.Show("You were captured, but you can keep on playing");
            timeTicking = false;
            policeSiren.Stop(); 
            timer.text = "00:00" ;
            //timer.fontSize = 100;
            //timer.enabled = true;
        }
        else
        {
            timer.text = GetTimeString(minutes) + ":" + GetTimeString(seconds);
            takingAway = false;
        }
    }

    
    IEnumerator TimerIncrease()
    {
        addingTime = true;
        yield return new WaitForSecondsRealtime(1);
        if (secondsTaken != 59)
        {
            secondsTaken += 1;
        }
        else {
            minutesTaken++;
            secondsTaken = 0;
        }
            addingTime = false;
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
        if (!takingAway && timeTicking && (seconds > 0 || minutes > 0))
        {
            StartCoroutine(TimerTake());
        }

        if (!addingTime && timeTicking) {
            StartCoroutine(TimerIncrease());
        }

        if (minutes < 1 && policeSiren.volume != 1f) {
            policeSiren.volume = 1;
        } else if ( minutes < 5 && policeSiren.volume != 0.5f) {
            policeSiren.volume = 0.5f;
        }
        else if (minutes < 10 && !policeSiren.isPlaying && timeTicking) {
            policeSiren.Play();
        }

        if (!cookieEaten && cookieActive)
        {
            if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
            {
                    cookieEaten = true;
                    cookieCrisp.Play();
                    spawnedPrefabs["cookie"].SetActive(false);
            }
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
