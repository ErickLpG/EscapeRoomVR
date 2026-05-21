using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class ReturnMainScene : MonoBehaviour
{
    #region Input
    [Header("Input")]
    public InputActionReference accionBotonX;
    #endregion

    #region Configuracion
    [Header("Configuracion")]
    public string nombreEscenaInicio = "MenuInicio";
    public float tiempoNecesario = 5f;
    public bool mostrarDebug = true;
    #endregion

    #region Referencias UI opcionales
    [Header("UI Opcional")]
    public GameObject panelVolverInicio;
    public TextMeshProUGUI textoVolverInicio;
    #endregion

    #region Variables privadas
    private float tiempoPresionado = 0f;
    private bool botonPresionado = false;
    private bool cambioEjecutado = false;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (panelVolverInicio != null)
        {
            panelVolverInicio.SetActive(false);
        }

        ActualizarTexto(0f);
    }

    private void OnEnable()
    {
        if (accionBotonX != null)
        {
            accionBotonX.action.Enable();
            accionBotonX.action.started += OnBotonIniciado;
            accionBotonX.action.canceled += OnBotonSoltado;
        }
    }

    private void OnDisable()
    {
        if (accionBotonX != null)
        {
            accionBotonX.action.started -= OnBotonIniciado;
            accionBotonX.action.canceled -= OnBotonSoltado;
            accionBotonX.action.Disable();
        }
    }

    private void Update()
    {
        if (!botonPresionado || cambioEjecutado)
        {
            return;
        }

        tiempoPresionado += Time.deltaTime;

        ActualizarTexto(tiempoPresionado);

        if (mostrarDebug)
        {
            Debug.Log("Manteniendo X: " + tiempoPresionado.ToString("F1") + " / " + tiempoNecesario);
        }

        if (tiempoPresionado >= tiempoNecesario)
        {
            VolverAlInicio();
        }
    }
    #endregion

    #region Input Methods
    private void OnBotonIniciado(InputAction.CallbackContext context)
    {
        botonPresionado = true;
        tiempoPresionado = 0f;

        if (panelVolverInicio != null)
        {
            panelVolverInicio.SetActive(true);

            SeguirCamaraVR panelSeguirCamara = panelVolverInicio.GetComponent<SeguirCamaraVR>();

            if (panelSeguirCamara != null)
            {
                panelSeguirCamara.ActualizarPosicionInmediata();
            }
        }

        ActualizarTexto(tiempoPresionado);

        if (mostrarDebug)
        {
            Debug.Log("Boton X presionado. Mantener para volver al inicio.");
        }
    }

    private void OnBotonSoltado(InputAction.CallbackContext context)
    {
        botonPresionado = false;
        tiempoPresionado = 0f;

        if (panelVolverInicio != null)
        {
            panelVolverInicio.SetActive(false);
        }

        ActualizarTexto(tiempoPresionado);

        if (mostrarDebug)
        {
            Debug.Log("Boton X soltado. Se cancelo volver al inicio.");
        }
    }
    #endregion

    #region UI
    private void ActualizarTexto(float tiempoActual)
    {
        if (textoVolverInicio == null)
        {
            return;
        }

        float tiempoRestante = Mathf.Max(0f, tiempoNecesario - tiempoActual);

        textoVolverInicio.text =
            "Mantén presionado X para volver al inicio\n" +
            "Tiempo restante: " + tiempoRestante.ToString("F1") + " s";
    }
    #endregion

    #region Cambio de escena
    private void VolverAlInicio()
    {
        cambioEjecutado = true;

        if (mostrarDebug)
        {
            Debug.Log("Volviendo a la escena de inicio.");
        }

        if (string.IsNullOrEmpty(nombreEscenaInicio))
        {
            Debug.LogWarning("No se asigno el nombre de la escena de inicio.");
            return;
        }

        SceneManager.LoadScene(nombreEscenaInicio);
    }
    #endregion
}