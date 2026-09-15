using UnityEngine;
using UI;
using System.Reflection.Emit;
using Config;
using DataStructures.Localization;

namespace DataStructures.Settings
{
    [CreateAssetMenu(fileName = "StringBindingSO", menuName = "Settings/String Binding")]
    public class StringBindingSO : SettingBindingSO<string>
    {
        [SerializeField] 
        [Tooltip("A list of dropdown options for the user to select")]
        LocalizableString[] validInputs;

        public override void BindTo(SettingItemView view)
        {
            view.SetLabelContents(label.Display());
        }

        public override string RequestMenuType()
        {
            return Strings.SETTING_VIEW_DROPDOWN;
        }
    }
}