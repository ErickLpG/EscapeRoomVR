using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AcertijoConteoObjetos : MonoBehaviour
{
    #region Referencias UI
    [Header("Referencias UI")]
    public TextMeshProUGUI textoAnimales;
    public TextMeshProUGUI textoCarros;
    public TextMeshProUGUI textoBarcos;
    public TextMeshProUGUI textoEstado;
    #endregion

    #region Configuracion conteo correcto
    [Header("Conteo correcto")]
    public int animalesCorrectos = 3;
    public int carrosCorrectos = 2;
    public int barcosCorrectos = 4;
    #endregion

    #region Configuracion general
    [Header("Configuracion")]
    public bool resolverAutomaticamente = true;
    public bool bloquearAlResolver = true;
    public int valorMaximoPorCategoria = 9;
    #endregion

    #region Eventos
    [Header("Eventos")]
    public UnityEvent alResolver;
    public UnityEvent alFallar;
    #endregion

    #region Variables privadas
    private int animalesActuales = 0;
    private int carrosActuales = 0;
    private int barcosActuales = 0;
    private bool resuelto = false;
    #endregion

    #region Unity Methods
    private void Start()
    {
        ReiniciarConteo();
    }
    #endregion

    #region Botones de conteo
    public void SumarAnimal()
    {
        if (NoPuedeInteractuar())
        {
            return;
        }

        animalesActuales++;
        animalesActuales = LimitarValor(animalesActuales);

        Debug.Log("Animales: " + animalesActuales);

        ActualizarPantalla();
        RevisarSolucionAutomatica();
    }

    public void SumarCarro()
    {
        if (NoPuedeInteractuar())
        {
            return;
        }

        carrosActuales++;
        carrosActuales = LimitarValor(carrosActuales);

        Debug.Log("Carros: " + carrosActuales);

        ActualizarPantalla();
        RevisarSolucionAutomatica();
    }

    public void SumarBarco()
    {
        if (NoPuedeInteractuar())
        {
            return;
        }

        barcosActuales++;
        barcosActuales = LimitarValor(barcosActuales);

        Debug.Log("Barcos: " + barcosActuales);

        ActualizarPantalla();
        RevisarSolucionAutomatica();
    }
    #endregion

    #region Botones extra opcionales
    public void ConfirmarConteo()
    {
        if (NoPuedeInteractuar())
        {
            return;
        }

        if (ConteoCorrecto())
        {
            ResolverAcertijo();
        }
        else
        {
            ConteoIncorrecto();
        }
    }

    public void ReiniciarConteo()
    {
        if (resuelto && bloquearAlResolver)
        {
            return;
        }

        animalesActuales = 0;
        carrosActuales = 0;
        barcosActuales = 0;

        if (textoEstado != null)
        {
            textoEstado.text = "Cuenta: animales, carros y barcos";
        }

        ActualizarPantalla();

        Debug.Log("Conteo reiniciado");
    }
    #endregion

    #region Logica
    private void RevisarSolucionAutomatica()
    {
        if (!resolverAutomaticamente)
        {
            return;
        }

        if (ConteoCorrecto())
        {
            ResolverAcertijo();
        }
    }

    private bool ConteoCorrecto()
    {
        return animalesActuales == animalesCorrectos &&
               carrosActuales == carrosCorrectos &&
               barcosActuales == barcosCorrectos;
    }

    private void ResolverAcertijo()
    {
        resuelto = true;

        if (textoEstado != null)
        {
            textoEstado.text = "Conteo correcto";
        }

        Debug.Log("Acertijo de conteo resuelto");

        if (alResolver != null)
        {
            alResolver.Invoke();
        }
    }

    private void ConteoIncorrecto()
    {
        if (textoEstado != null)
        {
            textoEstado.text = "Conteo incorrecto";
        }

        Debug.Log("Conteo incorrecto");

        if (alFallar != null)
        {
            alFallar.Invoke();
        }

        ReiniciarConteo();
    }

    private int LimitarValor(int valor)
    {
        if (valor > valorMaximoPorCategoria)
        {
            return 0;
        }

        return valor;
    }

    private bool NoPuedeInteractuar()
    {
        if (resuelto && bloquearAlResolver)
        {
            return true;
        }

        return false;
    }

    private void ActualizarPantalla()
    {
        if (textoAnimales != null)
        {
            textoAnimales.text = "Animales: " + animalesActuales;
        }

        if (textoCarros != null)
        {
            textoCarros.text = "Carros: " + carrosActuales;
        }

        if (textoBarcos != null)
        {
            textoBarcos.text = "Barcos: " + barcosActuales;
        }
    }
    #endregion
}