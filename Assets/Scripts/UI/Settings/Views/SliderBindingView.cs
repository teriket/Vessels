using DataStructures.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Configures a UI Slider element to match a setting bindings requested layout
    /// and transmit UI events to the setting binding
    /// </summary>
    public class SliderBindingView : SettingItemView
    {
        private Slider sliderComponent;
        private FloatBindingSO binding;
        private bool initialized = false;

        void Start()
        {
            GetSlider();
        }

        /// <summary>
        /// Get a Slider component attached to this gameobject.
        /// </summary>
        /// <returns>True if the Get request was successful, False otherwise.</returns>
        private bool GetSlider()
        {
            // the slider component has already been cached, exit early
            if (sliderComponent != null)
            {
                return true;
            }

            sliderComponent = GetComponent<Slider>();

            // the gameobject does not have a slider on it, exit with a failure
            if (sliderComponent == null)
            {
                Development.Logger.Warn("Could not find a slider component on this gameobject");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Update the sliders minimum value
        /// </summary>
        /// <param name="min"></param>
        public void SetMin(float min)
        {
            if (sliderComponent == null && !GetSlider()) return;

            sliderComponent.minValue = min;
        }

        /// <summary>
        /// Update the sliders maximum value
        /// </summary>
        /// <param name="max"></param>
        public void SetMax(float max)
        {
            if (sliderComponent == null && !GetSlider()) return;

            sliderComponent.maxValue = max;
        }

        /// <summary>
        /// Set the initial value the slider is set at
        /// </summary>
        /// <param name="value"></param>
        public void SetSliderValue(float value)
        {
            if (sliderComponent == null && !GetSlider()) return;
            sliderComponent.value = value;
        }

        /// <summary>
        /// Sets the scriptable object whose value this slider updates
        /// </summary>
        /// <param name="binding"></param>
        public void SetBinding(FloatBindingSO binding)
        {
            this.binding = binding;
            initialized = true;
        }

        /// <summary>
        /// Updates the value of a setting on slider events
        /// </summary>
        /// <param name="value"></param>
        public void UpdateBinding(float value)
        {
            if (binding == null)
            {
                LogPostInitializationFailures("Could not update the setting value for a slider event.");
                return;
            }

            binding.Set(value);
        }

        /// <summary>
        /// The order of execution for object creation is not guaranteed.
        /// It add's a lot of false positives to log null pointer exceptions for
        /// objects that have not yet been fully instantiated.  This only logs
        /// errors for objects after they've been fully configured.
        /// </summary>
        /// <param name="message"></param>
        private void LogPostInitializationFailures(string message)
        {
            if (initialized)
            {
                Development.Logger.Warn(message);
            }
        }
    }
}