//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.1
//     from Assets/GameControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace IGameControlsActions
{
    public partial class @GameControls: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @GameControls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameControls"",
    ""maps"": [
        {
            ""name"": ""Turn"",
            ""id"": ""fd51ddb3-a778-4fde-982b-8c0c2b8245bb"",
            ""actions"": [
                {
                    ""name"": ""Right"",
                    ""type"": ""Value"",
                    ""id"": ""7ec03db6-9fb5-4a6a-b8be-396ec62dc262"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""5e619609-42f1-4a0d-ac2e-55b8ab9492df"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Negative"",
                    ""id"": ""b94600a8-3a49-43fc-8e77-b38be00f1222"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Positive"",
                    ""id"": ""d01e8a4e-173f-4847-a63b-494aaa15a316"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""1de5eede-c100-4cd3-96a7-7ef031662d09"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""6be17ce4-5981-494c-8d74-8d7fa57419a7"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""34d1c76e-1a56-40cf-98e6-d036b9936eb4"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Turn
            m_Turn = asset.FindActionMap("Turn", throwIfNotFound: true);
            m_Turn_Right = m_Turn.FindAction("Right", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Turn
        private readonly InputActionMap m_Turn;
        private List<ITurnActions> m_TurnActionsCallbackInterfaces = new List<ITurnActions>();
        private readonly InputAction m_Turn_Right;
        public struct TurnActions
        {
            private @GameControls m_Wrapper;
            public TurnActions(@GameControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Right => m_Wrapper.m_Turn_Right;
            public InputActionMap Get() { return m_Wrapper.m_Turn; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(TurnActions set) { return set.Get(); }
            public void AddCallbacks(ITurnActions instance)
            {
                if (instance == null || m_Wrapper.m_TurnActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_TurnActionsCallbackInterfaces.Add(instance);
                @Right.started += instance.OnRight;
                @Right.performed += instance.OnRight;
                @Right.canceled += instance.OnRight;
            }

            private void UnregisterCallbacks(ITurnActions instance)
            {
                @Right.started -= instance.OnRight;
                @Right.performed -= instance.OnRight;
                @Right.canceled -= instance.OnRight;
            }

            public void RemoveCallbacks(ITurnActions instance)
            {
                if (m_Wrapper.m_TurnActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(ITurnActions instance)
            {
                foreach (var item in m_Wrapper.m_TurnActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_TurnActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public TurnActions @Turn => new TurnActions(this);
        public interface ITurnActions
        {
            void OnRight(InputAction.CallbackContext context);
        }
    }
}
