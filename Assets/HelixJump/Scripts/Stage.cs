using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlatformData
{
    [Range(1, 11)]
    public int partCount = 11;

    [Range(0, 11)]
    public int deathPartCount = 1;
}

[CreateAssetMenu(fileName = "New Stage", menuName = "Game/New Stage")]
public class Stage : ScriptableObject
{
    public Color stageBackgroundColor = Color.white;
    public Color stagePlatformPartColor = Color.white;
    public Color stageBallColor = Color.white;
    public List<PlatformData> platforms = new();
}
