using System;
using UnityEngine;
using UnityEngine.UI; // Add this namespace for UI.Text

namespace UnityStandardAssets.Utility
{
    [RequireComponent(typeof(Text))] // Update to require a Text component instead of GUIText
    public class FPSCounter : MonoBehaviour
    {
        const float fpsMeasurePeriod = 0.5f;
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private int m_CurrentFps;
        const string display = "{0} FPS";
        private Text m_GuiText; // Changed from GUIText to Text

        private void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
            m_GuiText = GetComponent<Text>(); // Get the Text component instead of GUIText
        }

        private void Update()
        {
            // measure average frames per second
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += fpsMeasurePeriod;
                m_GuiText.text = string.Format(display, m_CurrentFps); // Update the Text component
            }
        }
    }
}

