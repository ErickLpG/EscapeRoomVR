using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class Move : MonoBehaviour
{
    #region Variables

    [Header("Movimiento")]
    public float speed = 2.5f;
    public float rotationSpeed = 90f;
    public float gravity = -9.81f;

    private CharacterController characterController;
    private PlayerInputAction inputActions;
    private Vector2 moveInput;
    private float rotateInput;
    private Vector3 velocity;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new PlayerInputAction();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Rotate.performed += ctx => rotateInput = ctx.ReadValue<float>();
        inputActions.Player.Rotate.canceled += ctx => rotateInput = 0f;

        inputActions.Player.Reset.performed += ctx => ReiniciarEscena();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        Mover();
        Girar();
        AplicarGravedad();
    }

    #endregion

    #region Move Methods

    private void Mover()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = transform.TransformDirection(move);

        characterController.Move(move * speed * Time.deltaTime);
    }

    private void Girar()
    {
        float rotation = rotateInput * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
    }

    private void AplicarGravedad()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    public void ReiniciarEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}