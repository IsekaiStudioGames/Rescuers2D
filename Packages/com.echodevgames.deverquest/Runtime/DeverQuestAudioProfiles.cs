using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [CreateAssetMenu(
        fileName = "NewWarningAudioProfile",
        menuName = "DeverQuest/Audio/Warning Profile")]
    public sealed class DeverQuestWarningAudioProfile :
        ScriptableObject
    {
        [Range(0f, 1f)]
        public float volume = 0.8f;
        public AudioClip idleWarning;
        public AudioClip idlePaused;
        public AudioClip focusCheckIn;
        public AudioClip hydration;
        public AudioClip movementBreak;
        public AudioClip mealReminder;
        public AudioClip stageComplete;
        public AudioClip encounterAttack;
        public AudioClip encounterDanger;
        public AudioClip encounterVictory;
        public AudioClip encounterDefeat;
        public AudioClip questComplete;
        public AudioClip levelUp;
        public AudioClip purchase;
        public AudioClip error;
    }

    }
