using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timer;
    private AudioController audioController;

    private bool takingAway = false;

    private bool addingTime = false;

    private bool timeTicking = true;

    private int minutes = 60;
    private int seconds = 0;

    private int secondsTaken = 0;

    private void Awake()
    {
        timer.enabled = false;
        audioController = GetComponent<AudioController>();
    }

    void Update()
    {
        if (!takingAway && timeTicking && (seconds > 0 || minutes > 0))
        {
            StartCoroutine(TimerTake());
        }

        if (!addingTime && timeTicking)
        {
            StartCoroutine(TimerIncrease());
        }

        if (minutes < 1)
        {
            audioController.ChangeAudioVolume(AudioPlayer.Police, 0.8f);
        }
        else if (minutes < 5)
        {
            audioController.ChangeAudioVolume(AudioPlayer.Police, 0.15f);
        }
        else if (minutes < 10 && timeTicking)
        {
            audioController.StartAudio(AudioPlayer.Police);
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
            audioController.StopAudio(AudioPlayer.Police);
            timer.text = "00:00";
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

        secondsTaken++;
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

    public void ShowTimeLeft()
    {
        timer.enabled = true;
    }

    public void HideTimeLeft()
    {
        timer.enabled = false;
    }

    public void StopTicking() {
        timeTicking = false;
    }

}
