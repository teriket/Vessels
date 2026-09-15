using System;
using UnityEngine;

namespace Events
{
    /// <summary>
    /// Base class for event channels.  Usage has four steps: <br/>
    /// 1. Create a concrete type implementation that inherits from GameEventSO&lt;T&gt;.
    /// <code>
    /// [CreateAssetMenu(menuName = "Events/Float Event Channel")]
    /// public class IntEventSO : GameEventSO&lt;int&gt; { }
    /// </code>
    /// 2. Create a scriptable object from the inherited class in the unity editor.
    /// This scriptable object becomes a pipeline for events. <br/>
    /// 3. Message senders/ receivers should serialize a field for the event pipeline.
    /// A scriptable object instance should be dragged and dropped over the serialized
    /// field in the Unity inspector to associate that member variable with a specific
    /// event channel
    /// <code>
    /// public class MyClass : MonoBehaviour {
    ///     [SerializeField] IntEventSO intEventChannel;
    /// }
    /// </code>
    /// 
    /// 4. Message senders call Raise(param)
    /// <code>
    /// public class ClassThatSendsEvents : MonoBehaviour {
    ///     [SerializeField] IntEventSO channel;
    ///     public Start(){
    ///         int someNumberToSendAsEvent = 3;
    ///         channel.Raise(someNumberToSendAsEvent);
    ///     }
    /// }
    /// </code>
    /// 5. Message receivers should subscribe to channel.OnRaised.  Receivers should
    /// unsubscribe to events in OnDestroy() and OnDisable() to prevent memory leaks.
    /// <code>
    /// public class ClassThatReceivesEvents : MonoBehaviour {
    ///     [SerializeField] IntEventSO channel;
    ///     void OnEnable() => channel.OnRaised += MethodCall;
    ///     void OnDisable() => channel.OnRaised -= MethodCall;
    ///     void OnDestroy() => channel.OnRaised -= MethodCall;
    ///     
    ///     void MethodCall(int eventValue){
    ///         print(eventValue);
    ///     }
    /// }
    /// </code>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GameEventSO<T> : ScriptableObject
    {
        public event Action<T> OnRaised;
        public void Raise(T value) => OnRaised?.Invoke(value);
    }
}