using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameTimer : MonoBehaviour
{
    [SerializeField] private Text _timeText;
    [SerializeField] private Image _progressBar;

    public int StartTime { private get; set; }

    private int _time = 0;
    public int Time 
    {
        private get => _time;
        set
        {
            _time = value;
            UpdateProgressBar();
            _timeText.text = Time.ToString();
        }
    }

    private Action _callBack;

    private WaitForSeconds _wait = new WaitForSeconds(1);

    private Color _yellow = new Color32(253, 203, 110, 255);
    private Color _red = new Color32(225, 112, 85, 255);

    public void SetTimer(int startTime)
    {
        StartTime = startTime;
        Time = startTime;
    }

    public void StartTimer(Action callBack)
    {
        _callBack = callBack;
        StartCoroutine(TimerRoutine());
    }


    private void UpdateProgressBar()
    {
        float fillAmount = (float)Time / (float)StartTime;
        if(fillAmount < 0.5f)
        {
            _progressBar.color = _yellow;
        }
        if(fillAmount < 0.2f)
        {
            _progressBar.color = _red;

        }

        _progressBar.fillAmount = fillAmount;  
    }

    IEnumerator TimerRoutine()
    {
        while (_time > 0)
        {
            yield return _wait;
            Time--;
        }

        _callBack();
        yield return null;
    }

}
