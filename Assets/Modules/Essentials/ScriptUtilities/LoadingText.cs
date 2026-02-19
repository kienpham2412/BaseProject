using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

namespace MyLoading
{
    public class LoadingText : MonoBehaviour
    {
        [SerializeField] private float delayDisplay = 0f;
        [SerializeField] private TextMeshProUGUI txtLoading;
        [SerializeField] private bool randomColorText = true;
        private Coroutine cLoading;
        private object delay = new WaitForSecondsRealtime(.3f);

        /// <summary>
        /// Reset is called when the user hits the Reset button in the Inspector's
        /// context menu or when adding the component the first time.
        /// </summary>
        void Reset()
        {
            txtLoading = GetComponent<TextMeshProUGUI>();
        }

        private void Awake()
        {
            txtLoading.SetText("");
        }

        private void OnEnable()
        {
            if (randomColorText)
                txtLoading.color = GetRandomColor();
            if (cLoading != null)
                StopCoroutine(cLoading);
            cLoading = StartCoroutine(ILoading());
        }

        /// <summary>
        /// Create a random color
        /// </summary>
        /// <returns></returns>
        private Color GetRandomColor()
        {
            float r = UnityEngine.Random.Range(0f, 1f);
            float g = UnityEngine.Random.Range(0f, 1f);
            float b = UnityEngine.Random.Range(0.5f, 1f);

            return new Color(r, g, b, 1);
        }

        private IEnumerator ILoading()
        {
            if (delayDisplay > 0f)
            {
                txtLoading.SetText("");
                yield return new WaitForSecondsRealtime(delayDisplay);
            }
            
            int count = 0;
            string strLoading = "Loading"; // LeanLocalization.GetTranslationText("LOADING", "Loading");

            var delay = new WaitForSecondsRealtime(.3f);
            while (gameObject.activeInHierarchy)
            {
                if (txtLoading != null)
                {
                    if (count == 0)
                        txtLoading.SetText($"{strLoading}.");
                    else if (count == 1)
                        txtLoading.SetText($"{strLoading}..");
                    else
                        txtLoading.SetText($"{strLoading}...");
                }
                count++;
                if (count > 2)
                    count = 0;
                yield return delay;
            }
        }
    }
}