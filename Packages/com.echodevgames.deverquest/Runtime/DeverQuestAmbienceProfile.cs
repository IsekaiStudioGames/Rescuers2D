using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
[CreateAssetMenu(
        fileName = "NewAmbienceProfile",
        menuName = "DeverQuest/Audio/Ambience Profile")]
    public sealed class DeverQuestAmbienceProfile : ScriptableObject
    {
        public string displayName = "Guildhall Ambience";
        [TextArea(2, 5)]
        public string description = string.Empty;
        public List<AudioClip> ambienceClips =
            new List<AudioClip>();
        [Range(0f, 1f)]
        public float volume = 0.35f;
        public bool shuffle = true;
        public bool playDuringActiveQuest = true;
    }
}
