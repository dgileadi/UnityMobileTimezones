using MobileTimezones;
using System;
using TMPro;
using UnityEngine;

public class SampleScene : MonoBehaviour
{
    [SerializeField]
    private TMP_Text timeZoneText;

    [SerializeField]
    private TMP_Text coordinatesText;

    [SerializeField]
    private TMP_Text countryCodesText;

    [SerializeField]
    private TMP_Text timeZoneInfoText;

    // Start is called before the first frame update
    void Start()
    {
        var zoneLocation = TimeZoneLocation.Current;
        if (zoneLocation == null)
        {
            timeZoneText.text = "Unknown";
            coordinatesText.text = "Unknown";
            countryCodesText.text = "Unknown";
            timeZoneInfoText.text = TimeZoneInfo.Local.Id;
        }
        else
        {
            timeZoneText.text = zoneLocation.Id;
            coordinatesText.text = zoneLocation.Coordinates.ToString();
            countryCodesText.text = string.Join(", ", zoneLocation.CountryCodes);
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(zoneLocation.Id);
            timeZoneInfoText.text = timeZoneInfo == null ? "Not Found" : timeZoneInfo.Id;
        }
    }
}
