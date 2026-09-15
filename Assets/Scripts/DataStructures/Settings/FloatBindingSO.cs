using UnityEngine;
using UI;
using Config;
using System;

namespace DataStructures.Settings
{
    [CreateAssetMenu(fileName = "FloatBindingSO", menuName = "Settings/Float Binding")]
    public class FloatBindingSO : SettingBindingSO<float>
    {
        [SerializeField]
        float min;

        [SerializeField]
        float max;

        public override void BindTo(SettingItemView view)
        {
            view.SetLabelContents(label.Display());

            if (view is SliderBindingView slider)
            {
                slider.SetMin(min);
                slider.SetMax(max);
                slider.SetSliderValue(Value);
                slider.SetBinding(this);
            }
            else
            {
                Development.Logger.Warn("Tried to update the settings of the slider view, but could not find a component of " +
                                        $"type SliderBinding.  Does the template gameobject have a SliderBinding component at its root?");
            }

        }

        public override string RequestMenuType()
        {
            return Strings.SETTING_VIEW_SLIDER;
        }
    }
}