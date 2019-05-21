using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class ReadMe : ScriptableObject
{
    public Texture2D icon;
    public float iconMaxWidth = 128f;
    public string title;
    public Section[] sections;

    [Serializable]
    public class Section
    {
        public string heading, linkText, url;
        public string[] text;
    }
}
