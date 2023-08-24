using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameEvent : MonoBehaviour
{
    #region Public properties
    public TextMeshProUGUI _nameText;
    public TextMeshProUGUI _timeText;
    public TextMeshProUGUI _descriptionText;
    #endregion

    #region Private properties
    DateTime _eventCreationTime;
    string _daysText, _hoursText, _minutesText, _secondsText;
    #endregion

    #region Public functions
    public void UpdateTimeText()
    {
        TimeSpan timeSpan = DateTime.Now.Subtract(_eventCreationTime);
        if (timeSpan.Days > 0)
        {
            _daysText = timeSpan.Days + "d ";
        }

        if (timeSpan.Hours > 0)
        {
            _hoursText = timeSpan.Hours + "h ";
        }
        if (timeSpan.Minutes > 0)
        {
            _minutesText = timeSpan.Minutes + "m ";
        }
        if (timeSpan.Seconds > 0)
        {
            _secondsText = timeSpan.Seconds + "s ";
        }

        _timeText.text = _daysText + _hoursText + _minutesText + _secondsText;
        if (String.IsNullOrEmpty(_timeText.text))// edge case handling where timespan less than 0 but time updated
        {
            _timeText.text = "now";
        }
        ClearText();
    }
    public void SetEventText(string nameText, string descriptionText)
    {
        _nameText.text = nameText;
        _timeText.text = "now";
        _descriptionText.text = descriptionText;
    }
    #endregion

    #region Private functions
    void ClearText()
    {
        _daysText = "";
        _hoursText = "";
        _minutesText = "";
    }
    #endregion

    #region Unity functions
    private void Start()
    {
        _eventCreationTime = DateTime.Now;
    }
    #endregion
}
