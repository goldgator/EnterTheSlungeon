// GENERATED AUTOMATICALLY FROM 'Assets/Data/BaseControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @BaseControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @BaseControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""BaseControls"",
    ""maps"": [
        {
            ""name"": ""Controls"",
            ""id"": ""1700c719-917f-4d6d-9fd5-3fd49d758967"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""824d6ff4-b300-4f8b-a708-cc5a1cf86579"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Value"",
                    ""id"": ""8c3c705d-b050-4b26-a6cf-4f884ee15caa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dodge"",
                    ""type"": ""Value"",
                    ""id"": ""7701e788-6fc6-4978-a704-39741962f245"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""StickAim"",
                    ""type"": ""Value"",
                    ""id"": ""bbd079fc-e832-4d29-a7e4-cec171cfb258"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MousePos"",
                    ""type"": ""Value"",
                    ""id"": ""d7f6ddd2-41ff-4f9b-9f90-078d65c97d9d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Map"",
                    ""type"": ""Value"",
                    ""id"": ""03be97e8-56e7-4d9f-b405-2ecd0f070b00"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""NextWeapon"",
                    ""type"": ""Value"",
                    ""id"": ""f7239539-8100-4247-a20e-bf41477ce464"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Value"",
                    ""id"": ""0daab417-e460-4f80-80eb-3525cf918df8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7c9d519d-608b-4059-8038-d24f913edefe"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2f0e6f97-0a18-448c-9a45-b87b8ea5039a"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""65bfcf2f-9e74-4355-a1ff-b08fb16acb2b"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dodge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1170e7d2-669e-4a1c-aa68-230d282ceabe"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dodge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""ef22a7ef-b189-4f7b-a155-2a7024044e2a"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6b3b9b1e-0add-42f2-8330-e95c2a54aee5"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""9932daf7-c8ee-44b8-a414-8ae0976d3f97"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""5aca5877-f599-4fb1-9d97-bfca7d60284a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""c79a98d6-ba93-4a86-bf5a-13de359bc38a"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""536c1c1a-a042-4e62-b978-d9e542b996e5"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""57efcf07-0e6e-4dd2-bbc4-a7fd1950cc2f"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""StickAim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""12518c3e-fcd3-48b4-a42f-11dd82f7adf4"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePos"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a96d7a9f-8190-4f27-82f2-cbb0e9d0b502"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Map"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1e9f251a-27ac-44fe-9d8a-78a0070f0af1"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Map"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f1708ec9-40ea-46c8-bf3a-6a1f1f2d41eb"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NextWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""552e3078-6efd-44a1-8656-e1b35c7b743a"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Controls
        m_Controls = asset.FindActionMap("Controls", throwIfNotFound: true);
        m_Controls_Move = m_Controls.FindAction("Move", throwIfNotFound: true);
        m_Controls_Fire = m_Controls.FindAction("Fire", throwIfNotFound: true);
        m_Controls_Dodge = m_Controls.FindAction("Dodge", throwIfNotFound: true);
        m_Controls_StickAim = m_Controls.FindAction("StickAim", throwIfNotFound: true);
        m_Controls_MousePos = m_Controls.FindAction("MousePos", throwIfNotFound: true);
        m_Controls_Map = m_Controls.FindAction("Map", throwIfNotFound: true);
        m_Controls_NextWeapon = m_Controls.FindAction("NextWeapon", throwIfNotFound: true);
        m_Controls_Reload = m_Controls.FindAction("Reload", throwIfNotFound: true);
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

    // Controls
    private readonly InputActionMap m_Controls;
    private IControlsActions m_ControlsActionsCallbackInterface;
    private readonly InputAction m_Controls_Move;
    private readonly InputAction m_Controls_Fire;
    private readonly InputAction m_Controls_Dodge;
    private readonly InputAction m_Controls_StickAim;
    private readonly InputAction m_Controls_MousePos;
    private readonly InputAction m_Controls_Map;
    private readonly InputAction m_Controls_NextWeapon;
    private readonly InputAction m_Controls_Reload;
    public struct ControlsActions
    {
        private @BaseControls m_Wrapper;
        public ControlsActions(@BaseControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Controls_Move;
        public InputAction @Fire => m_Wrapper.m_Controls_Fire;
        public InputAction @Dodge => m_Wrapper.m_Controls_Dodge;
        public InputAction @StickAim => m_Wrapper.m_Controls_StickAim;
        public InputAction @MousePos => m_Wrapper.m_Controls_MousePos;
        public InputAction @Map => m_Wrapper.m_Controls_Map;
        public InputAction @NextWeapon => m_Wrapper.m_Controls_NextWeapon;
        public InputAction @Reload => m_Wrapper.m_Controls_Reload;
        public InputActionMap Get() { return m_Wrapper.m_Controls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ControlsActions set) { return set.Get(); }
        public void SetCallbacks(IControlsActions instance)
        {
            if (m_Wrapper.m_ControlsActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMove;
                @Fire.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnFire;
                @Fire.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnFire;
                @Fire.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnFire;
                @Dodge.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnDodge;
                @Dodge.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnDodge;
                @Dodge.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnDodge;
                @StickAim.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnStickAim;
                @StickAim.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnStickAim;
                @StickAim.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnStickAim;
                @MousePos.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMousePos;
                @MousePos.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMousePos;
                @MousePos.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMousePos;
                @Map.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMap;
                @Map.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMap;
                @Map.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMap;
                @NextWeapon.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnNextWeapon;
                @NextWeapon.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnNextWeapon;
                @NextWeapon.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnNextWeapon;
                @Reload.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnReload;
                @Reload.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnReload;
                @Reload.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnReload;
            }
            m_Wrapper.m_ControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Fire.started += instance.OnFire;
                @Fire.performed += instance.OnFire;
                @Fire.canceled += instance.OnFire;
                @Dodge.started += instance.OnDodge;
                @Dodge.performed += instance.OnDodge;
                @Dodge.canceled += instance.OnDodge;
                @StickAim.started += instance.OnStickAim;
                @StickAim.performed += instance.OnStickAim;
                @StickAim.canceled += instance.OnStickAim;
                @MousePos.started += instance.OnMousePos;
                @MousePos.performed += instance.OnMousePos;
                @MousePos.canceled += instance.OnMousePos;
                @Map.started += instance.OnMap;
                @Map.performed += instance.OnMap;
                @Map.canceled += instance.OnMap;
                @NextWeapon.started += instance.OnNextWeapon;
                @NextWeapon.performed += instance.OnNextWeapon;
                @NextWeapon.canceled += instance.OnNextWeapon;
                @Reload.started += instance.OnReload;
                @Reload.performed += instance.OnReload;
                @Reload.canceled += instance.OnReload;
            }
        }
    }
    public ControlsActions @Controls => new ControlsActions(this);
    public interface IControlsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnFire(InputAction.CallbackContext context);
        void OnDodge(InputAction.CallbackContext context);
        void OnStickAim(InputAction.CallbackContext context);
        void OnMousePos(InputAction.CallbackContext context);
        void OnMap(InputAction.CallbackContext context);
        void OnNextWeapon(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
    }
}
