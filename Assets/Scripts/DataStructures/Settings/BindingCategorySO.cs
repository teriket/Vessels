using UnityEngine;
using UI;
using System.Reflection.Emit;
using Config;

namespace DataStructures.Settings
{
    [CreateAssetMenu(fileName = "BindingCategorySO", menuName = "Settings/Setting Category")]
    public class BindingCategorySO : SettingBindingSO<string>
    {
        public override void BindTo(SettingItemView view)
        {
            view.SetLabelContents(label.Display());
        }

        public override string RequestMenuType()
        {
            return Strings.SETTING_VIEW_CATEGORY;
        }
    }
}
