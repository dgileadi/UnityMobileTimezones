using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MobileTimezones
{
    public class TimeZoneLocation
    {
        public readonly string Id;
        public readonly Coordinates Coordinates;
        public readonly string[] CountryCodes;

        private TimeZoneLocation(string id, Coordinates coordinates, string[] countryCodes)
        {
            this.Id = id;
            this.Coordinates = coordinates;
            this.CountryCodes = countryCodes;
        }

        public static TimeZoneLocation Current => GetCurrentTimeZone();

        public static IEnumerable<TimeZoneLocation> All => GetAllTimeZones();

        private static Dictionary<string, TimeZoneLocation> _zones;
        private static Dictionary<string, string> _links;

        public static TimeZoneLocation ForId(string id)
        {
            if (_zones == null)
                ParseTimeZones();

            while (!_zones.ContainsKey(id) && _links.ContainsKey(id))
                id = _links[id];
            return _zones.GetValueOrDefault(id, null);
        }

        private static TimeZoneLocation GetCurrentTimeZone()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return TimeZoneLocation.ForId(NativeGetTimeZone());
#elif UNITY_ANDROID && !UNITY_EDITOR
        using (var tzClass = new AndroidJavaClass("java.util.TimeZone"))
        using (var timeZone = tzClass.CallStatic<AndroidJavaObject>("getDefault"))
        {
            var zoneId = timeZone.Call<string>("getID");
            return TimeZoneLocation.ForId(zoneId);
        }
#elif UNITY_EDITOR
            return TimeZoneLocation.ForId(TimeZoneInfo.Local.Id);
#endif
        }

        private static IEnumerable<TimeZoneLocation> GetAllTimeZones()
        {
            if (_zones == null)
                ParseTimeZones();
            return _zones.Values;
        }

        private static void ParseTimeZones()
        {
            // parse time zones
            _zones = new Dictionary<string, TimeZoneLocation>();
            var reader = new StringReader(Resources.Load<TextAsset>("timezones").text);
            for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                if (line.Length == 0 || line[0] == '#')
                    continue;
                var fields = line.Split('\t');
                var name = fields[2];
                _zones[name] = new TimeZoneLocation(name, new Coordinates(fields[1]), fields[0].Split(','));
            }

            // parse backwards links
            _links = new Dictionary<string, string>();
            reader = new StringReader(Resources.Load<TextAsset>("backward").text);
            for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                if (line.Length == 0 || line[0] == '#')
                    continue;
                var fields = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                _links[fields[2]] = fields[1];
            }
        }

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal", EntryPoint="UMTZ_GetTimeZone")]
        static extern string NativeGetTimeZone();
#endif
    }

    public class Coordinates
    {
        public readonly float Latitude;
        public readonly float Longitude;

        private static readonly char[] plusMinus = { '+', '-' };

        internal Coordinates(string s)
        {
            var index = 0;
            Latitude = ParseCoordinate(s, ref index);
            Longitude = ParseCoordinate(s, ref index);
        }

        private static float ParseCoordinate(string s, ref int index)
        {
            // ±DDMM±DDDMM or ±DDMMSS±DDDMMSS
            var start = index;
            if (s[start] != '+' && s[start] != '-')
                throw new ArgumentException();
            var end = (start > 0) ? s.Length : s.IndexOfAny(plusMinus, 1);
            var negative = s[start] == '-';
            index = end;
            if (end - start <= 6)
            {
                var degrees = int.Parse(s.Substring(start, end - start - 2));
                var minutes = float.Parse(s.Substring(end - 2, 2));
                return (float)degrees + (minutes / 60f);
            }
            else
            {
                var degrees = int.Parse(s.Substring(start, end - start - 4));
                var minutes = float.Parse(s.Substring(end - 4, 2));
                var seconds = float.Parse(s.Substring(end - 2, 2));
                return (float)degrees + (minutes / 60f) + (seconds / 3600f);
            }
        }

        public override string ToString()
        {
            return Latitude + ", " + Longitude;
        }
    }

    public static class TimeZoneInfoExtensions
    {
        public static Coordinates GetCoordinates(this TimeZoneInfo zone)
        {
            var zoneLocation = TimeZoneLocation.ForId(zone.Id);
            return zoneLocation != null ? zoneLocation.Coordinates : null;
        }

        public static string[] GetCountryCodes(this TimeZoneInfo zone)
        {
            var zoneLocation = TimeZoneLocation.ForId(zone.Id);
            return zoneLocation != null ? zoneLocation.CountryCodes : null;
        }
    }

}
