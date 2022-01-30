using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGJ2022
{

    public class DebugPrinter : MonoBehaviour
    {
        private List<string> logMessages = new List<string>();
        [SerializeField] private TMPro.TextMeshProUGUI text;
        // Start is called before the first frame update
        void Start()
        {
            Application.logMessageReceived += Application_logMessageReceived;
        }

        private void UpdateTexts()
        {
            List<string> reverseList = new List<string>(logMessages);
            reverseList.Reverse();
            text.text = string.Join("\n", reverseList);
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            logMessages.Add(condition);
            if (type != LogType.Log)
            {
                logMessages.Add(stackTrace);
            }
            UpdateTexts();
        }
    }
}