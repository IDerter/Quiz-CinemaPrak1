using System.Collections;
using UnityEngine;
using System;

namespace QuizCinema
{
    public static class GameUtility
    {
        private const float _resolutionDelayTime = 0.5f;
        public static float ResolutionDelayTime => _resolutionDelayTime;
        private const string _savePrefLvlKey = "Game_Lvl_Value";
        public static string SavePrefLvlKey => _savePrefLvlKey;

        private const string _savePrefHighScoreKey = "Game_HighScore_Value";
        public static string SavePrefHighScoreKey => _savePrefHighScoreKey;

        private const string _saveAllScoreKey = "Game_AllScore_Value";
        public static string SaveAllScoreKey => _saveAllScoreKey;
        private const string _fileName = "Q";
        public static string FileName => _fileName;

      /*  public static string FileDir
        {
            get
            {
            #if UNITY_ANDROID && !UNITY_EDITOR
                return Application.persistentDataPath + "/";
            #endif
            #if UNITY_EDITOR
                return Application.streamingAssetsPath + "/";
            #endif

            }
        }
      */
    }
}