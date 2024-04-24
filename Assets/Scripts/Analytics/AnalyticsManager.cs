using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public LevelAnalytics analytics;

    public void TriggerMarkerTime(string name)
    {
        LevelMarker marker = analytics.getMarker(name);
        marker.TriggerTime();
    }
}

[Serializable]
public class LevelMarker
{
    public string name;
    public double time { get; set; }

    public LevelMarker(string name)
    {
        this.name = name;
    }

    public TimeSpan getTimeSpan()
    {
        return TimeSpan.FromSeconds(time);
    }

    public override string ToString()
    {
        TimeSpan timeSpan = getTimeSpan();
        return timeSpan.ToString(@"hh\:mm\:ss\.fff");
    }

    public void TriggerTime()
    {
        time = Time.timeSinceLevelLoadAsDouble;
    }
}

[Serializable]
public class LevelAnalytics
{
    public string name;
    public List<LevelMarker> markers = new List<LevelMarker>();

    public LevelAnalytics(string levelName)
    {
        name = levelName;
        markers.Append(new LevelMarker("EndOfLevel"));
    }

    public void addMarker(string markerName)
    {
        markers.Append(new LevelMarker(markerName));
    }

    public LevelMarker removeMarker(string markerName)
    {
        for (int i = 0; i < markers.Count; i++)
        {
            if (markers[i].name == markerName)
            {
                LevelMarker marker = markers[i];
                markers.RemoveAt(i);
                return marker;
            }
        }
        return null;
    }

    public LevelMarker getMarker(string markerName)
    {
        for (int i = 0; i < markers.Count; i++)
            if (markers[i].name.Equals(markerName))
                return markers[i];
        return null;
    }
}