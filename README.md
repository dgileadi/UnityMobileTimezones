# Unity Mobile Time Zones

Returns time zone data on iOS and Android. The built-in `System.TimeZoneInfo.Local` often doesn't return a useful `Id`, so this plugin allows querying the system for its own declared current time zone.

This plugin also supplies rough time zone coordinates (latitude and longitude) and covered country codes. These are supplied by [the time zone database](https://www.iana.org/time-zones).

## Usage

Use the `TimeZoneLocation` class from the `MobileTimeZones` namespace to access time zone location data.

* `TimeZoneLocation.Current` returns the current time zone location.
* `TimeZoneLocation.ForId(string)` returns a time zone location by ID like `"America/New_York"`.
* `TimeZoneLocation.GetAllTimeZones()` enumerates all time zone locations.

Each instance of a `TimeZoneLocation` contains the following fields:

* `Id`, e.g. `America/New_York`, which you can use to get a standard `System.TimeZoneInfo` by calling `TimeZoneInfo.FindSystemTimeZoneById(myZoneLocation.Id)`.
* `Coordinates`, the rough latitude and longitude of the time zone.
* `CountryCodes`, an array of two-letter codes of the countries the time zone falls in.

This plugin also adds a couple of extension methods to `System.TimeZoneInfo`:

* `GetCoordinates()` which returns the coordinates of the time zone.
* `GetCountryCodes()` which returns the time zone's country codes.
