using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Soli.Controls
{
    public class InputControl : MonoBehaviour, MasterInput.IMouseActions, MasterInput.ITouchActions
    {
        private MasterInput m_masterInput;
        private Camera m_camera;
        private Vector2 m_inputPosition = new Vector2();
        private static InputControl instance;

        //mouse drag pos
        //public Vector2 InputPosition { get => m_inputPosition; } //MOUSE
        public Vector2 InputPosition { get => TrackInputLocation(); }

        //On mouse hold
        public event Action<bool> OnMouseDownEvent;

        //On mouse release
        public event Action OnMouseUpEvent;


        private void Awake()
        {
            m_camera = Camera.main;

            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            m_masterInput = new MasterInput();
            Input.multiTouchEnabled = false;

#if UNITY_ANDROID
             m_masterInput.Touch.SetCallbacks(this);
#elif UNITY_EDITOR
            m_masterInput.Mouse.SetCallbacks(this);
#endif
            m_masterInput.Enable();
        }

        private void OnDestroy()
        {
            m_masterInput.Disable();
        }

        public static InputControl GetInstance()
        {
            return instance;
        }


        private Vector2 TrackInputLocation()
        {
            Vector2 temp = Touchscreen.current.position.ReadValue();
            m_inputPosition = m_camera.ScreenToWorldPoint(temp);
            return m_inputPosition;
        }

        public void OnMouseDown(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                //Holding
                OnMouseDownEvent?.Invoke(false);
            }

            if (context.canceled)
            {
                //Stopped Holding
                OnMouseUpEvent?.Invoke();
                
            }
        }

        public void OnMouseDrag(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                //Moving
                Vector3 screenPos = context.ReadValue<Vector2>();
                m_inputPosition = m_camera.ScreenToWorldPoint(screenPos);
            }
        }

        public void OnPrimaryTouch(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                TrackInputLocation();
                //Holding
                OnMouseDownEvent?.Invoke(false);
            }

            if (context.canceled)
            {
                TrackInputLocation();
                //Stopped Holding
                OnMouseUpEvent?.Invoke();
            }
        }

        public void OnDoubleTap(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnMouseDownEvent?.Invoke(true);
            }
        }
    }

}
