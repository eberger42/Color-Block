using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{

    private PlayerInput playerInput;

    public PlayerInput PlayerInput { get => playerInput; }

    public delegate void RotateLeftPressed();
    public event RotateLeftPressed OnRotateLeftPressed;

    public delegate void RotateRightPressed();
    public event RotateRightPressed OnRotateRightPressed;

    public delegate void Fire1Pressed();
    public event Fire1Pressed OnFire1Pressed;

    public delegate void Fire2Pressed();
    public event Fire2Pressed OnFire2Pressed;

    public delegate void CycleRightPressed();
    public event CycleRightPressed OnCycleRightPressed;

    public delegate void CycleLeftPressed();
    public event CycleLeftPressed OnCycleLeftPressed;

    public delegate void MovementPressed(Vector2 direction);
    public event MovementPressed OnMovementPressed;

    public delegate void MovementReleased(Vector2 direction);
    public event MovementReleased OnMovementReleased;

    public delegate void EscapePressed();
    public event EscapePressed OnEscape;

    private int rotateDirection;
    private Vector2 movementDirection;


    #region  Singleton

    public static PlayerInputManager Instance;

    private void Awake()
    {

        if (Instance != null)
        {
            Debug.LogWarning("PlayerInputManager already exists");
            Destroy(gameObject);
            return;
        }
        playerInput = GetComponent<PlayerInput>();
        Instance = this;

    }
    #endregion

    private void Update()
    {
        Rotate();
        Move();
    }


    public void Rotate_L(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rotateDirection = -1;
        }
        else if (context.canceled)
        {
            rotateDirection = 0;
        }
    }
    public void Rotate_R(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            rotateDirection = 1;
        }
        else if(context.canceled)
        {
            rotateDirection = 0;
        }
    }


    public void Fire_1(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (context.performed)
        {
            OnFire1Pressed?.Invoke();

        }
    }
    public void Fire_2(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (context.performed)
        {
            OnFire2Pressed?.Invoke();

        }
    }
    public void CycleLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnCycleLeftPressed?.Invoke();
        }
    }
    public void CycleRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnCycleRightPressed?.Invoke();
        }



    }
    public void MovementInput(InputAction.CallbackContext context)
    {
        var direction = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            movementDirection = new Vector2(0, 0);
            
        }
        else if (context.performed)
        {
            if(direction.x != 0 && direction.y != 0)
                movementDirection = direction - movementDirection;
            else
                movementDirection = direction;
        }
        


    }
    public void Escape(InputAction.CallbackContext context)
    { 
        if (context.started)
        {
            OnEscape?.Invoke();
        }
    }


    /////////////////////////////////////////////////////////////////////////
    /// private helpers
    /////////////////////////////////////////////////////////////////////////
    private void Rotate() 
    {
        if (rotateDirection == 0)
            return;
        else if(rotateDirection == 1)
            OnRotateRightPressed?.Invoke();
        else if(rotateDirection == -1) 
            OnRotateLeftPressed?.Invoke();
    }

    private void Move()
    {
        if (movementDirection.magnitude == 0)
            return;
        else
            OnMovementPressed?.Invoke(movementDirection);
    }


}
