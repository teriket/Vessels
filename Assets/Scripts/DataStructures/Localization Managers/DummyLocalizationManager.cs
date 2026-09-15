using DataStructures.IOC;
using UnityEngine;

namespace DataStructures.Localization
{
    public class DummyLocalizationManager : MonoBehaviour, ILocalizationManager, IIoCComponent<ILocalizationManager>
    {
        public IoCContainer iocContainer { get; set; }
        private IIoCComponent<ILocalizationManager> ioc => this;

        void Awake()
        {
            ioc.InitializeIoCContainer(this);
        }

        public void LoadLocalizations()
        {

        }

        public void SubscribeToLanguageChanges(LocalizableString str)
        {
            str.SetDisplay(str.value);
        }

        public void UnsubscribeToLanguageChanges(LocalizableString str)
        {

        }
    }
}