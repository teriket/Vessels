using UI;
using UnityEngine;
using UnityEngine.UI;
using DataStructures.Settings;

namespace UI
{
    public class CheckboxBindingView : SettingItemView
    {
        private Toggle checkboxComponent;
        private BoolBindingAsIntSO binding;
        private bool initialized = false;
        private bool isNegativePositiveOne = false;

        private bool GetCheckbox()
        {
            // don't recache the component if it already is cached
            if (checkboxComponent != null)
            {
                return true;
            }

            // cache the component
            checkboxComponent = GetComponent<Toggle>();
            if (checkboxComponent == null)
            {
                Development.Logger.Warn("Could not find a component of Toggle on this GameObject");
                return false;
            }

            return true;
        }

        void Start()
        {
            GetCheckbox();
        }

        public void SetBinding(BoolBindingAsIntSO binding)
        {
            this.binding = binding;
            initialized = true;
        }

        public void UpdateBinding()
        {
            if (binding == null)
            {
                LogPostInitializationError("Could not update the binding for this checkbox");
                return;
            }

            if (checkboxComponent == null && !GetCheckbox())
            {
                LogPostInitializationError("Could not find the Toggle components value for this setting view.");
                return;
            }

            int checkboxValue = ToggleBoolValueToInt();
            binding.Set(checkboxValue);
        }

        private void LogPostInitializationError(string message)
        {
            if (initialized)
            {
                Development.Logger.Warn(message);
            }
        }

        private int ToggleBoolValueToInt()
        {
            if (isNegativePositiveOne)
            {
                if (checkboxComponent.isOn)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (checkboxComponent.isOn)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public void SetInitialCheckedValue(int value)
        {
            if (checkboxComponent == null && !GetCheckbox())
            {
                Development.Logger.Warn("Could not set this checkboxes initial value");
                return;
            }

            if (value <= 0)
            {
                checkboxComponent.isOn = false;
            }
            else
            {
                checkboxComponent.isOn = true;
            }
        }

        public void SetReturnValueAsZeroOrOne()
        {
            isNegativePositiveOne = false;
        }

        public void SetReturnValueAsNegativeOrPositiveOne()
        {
            isNegativePositiveOne = true;
        }
    }
}