using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private Transform cameraTransform;

    private InputSystem_Actions inputSystemActions;
    private Rigidbody rigidBody;
    private Vector2 movementInput;
    private Vector2 lookInput;
    private float xRotation = 0f;

    private void Awake()
    {
        inputSystemActions = new InputSystem_Actions();
    }

    private void Start()
    {
        rigidBody = this.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        inputSystemActions.Enable();
        inputSystemActions.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputSystemActions.Player.Move.canceled += ctx => movementInput = Vector2.zero;

        inputSystemActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputSystemActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    private void OnDisable()
    {
        inputSystemActions.Player.Move.performed -= ctx => movementInput = ctx.ReadValue<Vector2>();
        inputSystemActions.Player.Move.canceled -= ctx => movementInput = Vector2.zero;

        inputSystemActions.Player.Look.performed -= ctx => lookInput = ctx.ReadValue<Vector2>();
        inputSystemActions.Player.Look.canceled -= ctx => lookInput = Vector2.zero;
        inputSystemActions.Disable();
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = transform.forward * movementInput.y + transform.right * movementInput.x;
        rigidBody.MovePosition(rigidBody.position + moveDirection * speed * Time.fixedDeltaTime);
    }
    
    private void LateUpdate()
    {
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime; // Умножаем на Time.deltaTime для стабильности
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        // Ограничиваем вращение камеры по вертикали (ось X)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Вращаем камеру вверх/вниз
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Вращаем персонажа по горизонтали (вокруг оси Y)
        transform.Rotate(Vector3.up * mouseX);
    }
}