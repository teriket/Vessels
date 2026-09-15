using UnityEngine;
using UI;
using Config;

namespace DataStructures.Settings
{
    [CreateAssetMenu(fileName = "IntBindingSO", menuName = "Settings/Int Binding")]
    public class BoolBindingAsIntSO : SettingBindingSO<int>
    {
        public enum IntParsingType
        {
            ZERO_OR_ONE,
            POSITIVE_NEGATIVE_ONE
        }

        [Header("View Settings")]
        [Tooltip("How the view should manage input data")]
        [SerializeField] IntParsingType acceptableValues;

        public override void BindTo(SettingItemView view)
        {
            view.SetLabelContents(label.Display());

            if (view is CheckboxBindingView checkbox)
            {
                checkbox.SetInitialCheckedValue(Value);

                if (acceptableValues == IntParsingType.ZERO_OR_ONE)
                {
                    checkbox.SetReturnValueAsZeroOrOne();
                }
                else
                {
                    checkbox.SetReturnValueAsNegativeOrPositiveOne();
                }

                checkbox.SetBinding(this);
            }
        }

        public override string RequestMenuType()
        {
            return Strings.SETTING_VIEW_CHECKBOX;
        }
    }
}