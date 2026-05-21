using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AcertijoSecuenciaSimbolos : MonoBehaviour
{
    #region Configuracion
    [Header("Configuracion")]
    public string[] secuenciaCorrecta = { "Estrella", "Hoja", "Circulo", "Luna" };
    public bool bloquearAlResolver = true;
    #endregion

    #region Referencias UI
    [Header("Referencias UI")]
    public TextMeshProUGUI textoEstado;
    public TextMeshProUGUI textoProgreso;
    #endregion

    #region Eventos
    [Header("Eventos")]
    public UnityEvent alResolver;
    public UnityEvent alFallar;
    #endregion

    #region Variables privadas
    private int indiceActual = 0;
    private bool resuelto = false;
    #endregion

    #region Unity Methods
    private void Start()
    {
        ReiniciarSecuencia();
    }
    #endregion

    #region Metodos publicos para botones
    public void PresionarSimbolo(string simbolo)
    {
        if (resuelto && bloquearAlResolver)
        {
            return;
        }

        Debug.Log("Simbolo presionado: " + simbolo);

        if (secuenciaCorrecta == null || secuenciaCorrecta.Length == 0)
        {
            Debug.LogWarning("No se configuro la secuencia correcta.");
            return;
        }

        if (simbolo == secuenciaCorrecta[indiceActual])
        {
            SimboloCorrecto();
        }
        else
        {
            SimboloIncorrecto();
        }
    }

    public void ReiniciarSecuencia()
    {
        indiceActual = 0;

        if (textoEstado != null)
        {
            textoEstado.text = "Introduce la secuencia";
        }

        ActualizarProgreso();

        Debug.Log("Secuencia reiniciada");
    }
    #endregion

    #region Logica interna
    private void SimboloCorrecto()
    {
        indiceActual++;

        if (textoEstado != null)
        {
            textoEstado.text = "Correcto";
        }

        ActualizarProgreso();

        if (indiceActual >= secuenciaCorrecta.Length)
        {
            ResolverAcertijo();
        }
    }

    private void SimboloIncorrecto()
    {
        Debug.Log("Secuencia incorrecta");

        if (textoEstado != null)
        {
            textoEstado.text = "Secuencia incorrecta";
        }

        if (alFallar != null)
        {
            alFallar.Invoke();
        }

        indiceActual = 0;
        ActualizarProgreso();
    }

    private void ResolverAcertijo()
    {
        resuelto = true;

        if (textoEstado != null)
        {
            textoEstado.text = "Compartimento desbloqueado";
        }

        if (textoProgreso != null)
        {
            textoProgreso.text = "Secuencia completa";
        }

        Debug.Log("Acertijo de simbolos resuelto");

        if (alResolver != null)
        {
            alResolver.Invoke();
        }
    }

    private void ActualizarProgreso()
    {
        if (textoProgreso == null)
        {
            return;
        }

        string progreso = "";

        for (int i = 0; i < secuenciaCorrecta.Length; i++)
        {
            if (i < indiceActual)
            {
                progreso += secuenciaCorrecta[i];
            }
            else
            {
                progreso += "_";
            }

            if (i < secuenciaCorrecta.Length - 1)
            {
                progreso += "  ";
            }
        }

        textoProgreso.text = progreso;
    }
    #endregion
}