using MelonLoader;
using System;
using System.Diagnostics;
using UnityEngine;
using GhostAdvancers;

namespace GhostAdvancers
{
    public class Popup
    {
        private static string? title;
        private static string? message;
        private static event Action<int>? action;
        private static double hideAfter;
        private static Stopwatch timer;
        private static int width = 200, height = 80;

        static Popup()
        {
            timer = new Stopwatch();
        }

        private static void StartShowing()
        {
            timer.Restart();
            MelonEvents.OnGUI.Subscribe(OnGUI, 100);
        }
        private static void StopShowing()
        {
            MelonEvents.OnGUI.Unsubscribe(OnGUI);
        }
        public static void Show(string _title, string _message, double _hideAfter = 10)
        {
            title = _title;
            message = _message;
            action = null;
            hideAfter = _hideAfter;
            StartShowing();
        }
        public static void Show(string _title, Action<int> _action, double _hideAfter = 10)
        {
            title = _title;
            action = _action;
            message = null;
            hideAfter = _hideAfter;
            StartShowing();
        }
        public static void SetSize(int _width, int _height)
        {
            width = _width;
            height = _height;
        }
        public static void SetSizePresetLongLine()
        {
            width = 500;
            height = 80;
        }
        public static void SetSizePresetLongMultiLine(int lines)
        {
            width = 500;
            height = 20+(lines*21)+28; // Probably wrong and bad
            //       ^ title       ^ button
        }
        private static void OnGUI()
        {
            if (timer.Elapsed.TotalSeconds >= hideAfter)
            {
                StopShowing();
                return;
            }
            GUI.Window(0, new Rect((Screen.width/2) - width/2, 20, width, height), Window, title);
        }
        private static void Window(int windowID)
        {
            if (message != null) GUILayout.Label(message);
            else action.Invoke(windowID);
            if (GUILayout.Button($"OK ({(int)(hideAfter - timer.Elapsed.Seconds)})")) StopShowing();
        }
    }
}
