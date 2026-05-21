using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AcertijoPC_Reloj : MonoBehaviour
{
    #region Referencias UI
    [Header("Referencias UI")]
    public TextMeshProUGUI textoCodigo;
    public TextMeshProUGUI textoEstado;
    #endregion

    #region Configuracion
    [Header("Configuracion")]
    public string codigoCorrecto = "0400";
    public int longitudMaxima = 4;
    public bool bloquearAlResolver = true;
    public float tiempoMostrarError = 1.2f;
    #endregion

    #region Eventos
    [Header("Eventos")]
    public UnityEvent alResolver;
    public UnityEvent alFallar;
    #endregion

    #region Variables privadas
    private string codigoIngresado = "";
    private bool resuelto = false;
    private bool mostrandoError = false;
    #endregion

    #region Unity Methods
    private void Start()
    {
        ReiniciarPantalla();
    }
    #endregion

    #region Metodos para botones
    public void AgregarDigito(string digito)
    {
        if (NoPuedeInteractuar())
        {
            return;
        }

        if (codigoIngresado.Length >= longitudMaxima)
        {
            return;
        }

        codigoIngresado += digito;
        ActualizarCodigoPantalla();

        if (textoEstado != null)
        {
            textoEstado.text = "Ingresando codigo";
        }

        Debug.Log("Digito ingresado: " + digito);
    }

    public void BorrarUltimo()
    {
        if (NoPuedeInteractuar())
        {
            return;
        }

        if (codigoIngresado.Length <= 0)
        {
            return;
        }

        codigoIngresado = codigoIngresado.Substring(0, codigoIngresado.Length - 1);
        ActualizarCodigoPantalla();

        Debug.Log("Se borro el ultimo digito");
    }

    public void LimpiarCodigo()
    {
        if (resuelto && bloquearAlResolver)
        {
            return;
        }

        codigoIngresado = "";
        ActualizarCodigoPantalla();

        if (textoEstado != null)
        {
            textoEstado.text = "Sistema bloqueado";
        }

        Debug.Log("Codigo limpiado");
    }

    public void ConfirmarCodigo()
    {
        if (NoPuedeInteractuar())
        {
            return;
        }

        Debug.Log("Codigo confirmado: " + codigoIngresado);

        if (codigoIngresado.Length < longitudMaxima)
        {
            StartCoroutine(MostrarErrorTemporal("Codigo incompleto"));
            return;
        }

        if (codigoIngresado == codigoCorrecto)
        {
            CodigoCorrecto();
        }
        else
        {
            CodigoIncorrecto();
        }
    }
    #endregion

    #region Logica interna
    private void CodigoCorrecto()
    {
        resuelto = true;

        if (textoEstado != null)
        {
            textoEstado.text = "Acceso concedido";
        }

        Debug.Log("Acertijo PC resuelto correctamente");

        if (alResolver != null)
        {
            alResolver.Invoke();
        }
    }

    private void CodigoIncorrecto()
    {
        Debug.Log("Codigo incorrecto");

        if (alFallar != null)
        {
            alFallar.Invoke();
        }

        StartCoroutine(MostrarErrorTemporal("Codigo incorrecto"));
    }

    private IEnumerator MostrarErrorTemporal(string mensaje)
    {
        mostrandoError = true;

        if (textoEstado != null)
        {
            textoEstado.text = mensaje;
        }

        yield return new WaitForSeconds(tiempoMostrarError);

        codigoIngresado = "";
        ActualizarCodigoPantalla();

        if (!resuelto && textoEstado != null)
        {
            textoEstado.text = "Sistema bloqueado";
        }

        mostrandoError = false;
    }

    private bool NoPuedeInteractuar()
    {
        if (resuelto && bloquearAlResolver)
        {
            return true;
        }

        if (mostrandoError)
        {
            return true;
        }

        return false;
    }

    private void ReiniciarPantalla()
    {
        codigoIngresado = "";
        ActualizarCodigoPantalla();

        if (textoEstado != null)
        {
            textoEstado.text = "Sistema bloqueado";
        }
    }

    private void ActualizarCodigoPantalla()
    {
        if (textoCodigo == null)
        {
            return;
        }

        string textoMostrado = "";

        for (int i = 0; i < longitudMaxima; i++)
        {
            if (i < codigoIngresado.Length)
            {
                textoMostrado += codigoIngresado[i];
            }
            else
            {
                textoMostrado += "_";
            }

            if (i < longitudMaxima - 1)
            {
                textoMostrado += " ";
            }
        }

        textoCodigo.text = textoMostrado;
    }
    #endregion
}