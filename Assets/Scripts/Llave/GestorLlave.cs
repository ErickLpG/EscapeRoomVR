using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GestorLlave : MonoBehaviour
{
    #region Referencias UI
    [Header("Referencias UI")]
    public GameObject canvasLlave;
    public CanvasGroup canvasGroupLlave;
    public Image imagenLlaveLlena;
    public TextMeshProUGUI textoProgreso;

    [Header("Panel Final")]
    public GameObject panelFinal;
    public CanvasGroup canvasGroupPanelFinal;
    public TextMeshProUGUI textoFinal;
    public TextMeshProUGUI textoConteoMenu;
    #endregion

    #region Audio
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoParteLlave;
    public AudioClip sonidoLlaveCompleta;
    public AudioClip sonidoPanelFinal;
    #endregion

    #region Configuracion
    [Header("Configuracion")]
    public int partesNecesarias = 3;
    public float tiempoMostrarLlave = 2f;
    public float tiempoAntesDeVolverMenu = 5f;
    public string nombreEscenaMenu = "MenuInicio";

    [Header("Animaciones")]
    public float duracionFadeEntrada = 0.35f;
    public float duracionFadeSalida = 0.35f;
    public float duracionLlenadoLlave = 1.2f;
    public float pausaAntesPanelFinal = 0.8f;
    #endregion

    #region Variables privadas
    private int partesObtenidas = 0;
    private bool llaveCompleta = false;
    private bool animando = false;
    private float fillActual = 0f;
    private Coroutine rutinaActual;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        InicializarUI();
    }
    #endregion

    #region Metodos publicos
    public void RegistrarParteLlave()
    {
        if (llaveCompleta || animando)
        {
            return;
        }

        partesObtenidas++;

        if (partesObtenidas > partesNecesarias)
        {
            partesObtenidas = partesNecesarias;
        }

        Debug.Log("Parte de llave obtenida: " + partesObtenidas + " / " + partesNecesarias);

        if (rutinaActual != null)
        {
            StopCoroutine(rutinaActual);
        }

        rutinaActual = StartCoroutine(ProcesoParteLlave());
    }

    public int ObtenerPartesObtenidas()
    {
        return partesObtenidas;
    }

    public bool LlaveEstaCompleta()
    {
        return llaveCompleta;
    }
    #endregion

    #region Flujo principal
    private IEnumerator ProcesoParteLlave()
    {
        animando = true;

        if (canvasLlave != null)
        {
            canvasLlave.SetActive(true);
        }

        if (panelFinal != null)
        {
            panelFinal.SetActive(false);
        }

        if (canvasGroupPanelFinal != null)
        {
            canvasGroupPanelFinal.alpha = 0f;
        }

        ReproducirSonido(sonidoParteLlave);

        yield return FadeCanvas(canvasGroupLlave, 0f, 1f, duracionFadeEntrada);

        float fillObjetivo = ObtenerProgresoNormalizado();

        yield return LlenarLlave(fillActual, fillObjetivo, duracionLlenadoLlave);

        fillActual = fillObjetivo;
        ActualizarTextoProgreso();

        if (partesObtenidas >= partesNecesarias)
        {
            yield return ProcesoLlaveCompleta();
        }
        else
        {
            yield return new WaitForSeconds(tiempoMostrarLlave);

            yield return FadeCanvas(canvasGroupLlave, 1f, 0f, duracionFadeSalida);

            if (canvasLlave != null)
            {
                canvasLlave.SetActive(false);
            }

            animando = false;
        }
    }

    private IEnumerator ProcesoLlaveCompleta()
    {
        llaveCompleta = true;

        Debug.Log("Llave completa. Escape room terminado.");

        ReproducirSonido(sonidoLlaveCompleta);

        yield return new WaitForSeconds(pausaAntesPanelFinal);

        if (panelFinal != null)
        {
            panelFinal.SetActive(true);
        }

        if (textoFinal != null)
        {
            textoFinal.text = "Has terminado";
        }

        ReproducirSonido(sonidoPanelFinal);

        yield return FadeCanvas(canvasGroupPanelFinal, 0f, 1f, duracionFadeEntrada);

        yield return ConteoRegresivoMenu();

        CargarMenu();
    }
    #endregion

    #region UI
    private void InicializarUI()
    {
        partesObtenidas = 0;
        llaveCompleta = false;
        animando = false;
        fillActual = 0f;

        if (canvasLlave != null)
        {
            canvasLlave.SetActive(false);
        }

        if (canvasGroupLlave != null)
        {
            canvasGroupLlave.alpha = 0f;
        }

        if (panelFinal != null)
        {
            panelFinal.SetActive(false);
        }

        if (canvasGroupPanelFinal != null)
        {
            canvasGroupPanelFinal.alpha = 0f;
        }

        if (imagenLlaveLlena != null)
        {
            imagenLlaveLlena.fillAmount = 0f;
        }

        ActualizarTextoProgreso();

        if (textoFinal != null)
        {
            textoFinal.text = "";
        }

        if (textoConteoMenu != null)
        {
            textoConteoMenu.text = "";
        }
    }

    private void ActualizarTextoProgreso()
    {
        if (textoProgreso != null)
        {
            textoProgreso.text = partesObtenidas + " / " + partesNecesarias;
        }
    }

    private float ObtenerProgresoNormalizado()
    {
        if (partesNecesarias <= 0)
        {
            return 0f;
        }

        return (float)partesObtenidas / partesNecesarias;
    }

    private IEnumerator FadeCanvas(CanvasGroup canvasGroup, float desde, float hasta, float duracion)
    {
        if (canvasGroup == null)
        {
            yield break;
        }

        float tiempo = 0f;
        canvasGroup.alpha = desde;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;

            canvasGroup.alpha = Mathf.Lerp(desde, hasta, t);

            yield return null;
        }

        canvasGroup.alpha = hasta;
    }

    private IEnumerator LlenarLlave(float desde, float hasta, float duracion)
    {
        if (imagenLlaveLlena == null)
        {
            yield break;
        }

        float tiempo = 0f;
        imagenLlaveLlena.fillAmount = desde;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;

            imagenLlaveLlena.fillAmount = Mathf.Lerp(desde, hasta, t);

            yield return null;
        }

        imagenLlaveLlena.fillAmount = hasta;
    }

    private IEnumerator ConteoRegresivoMenu()
    {
        float tiempoRestante = tiempoAntesDeVolverMenu;

        while (tiempoRestante > 0f)
        {
            if (textoConteoMenu != null)
            {
                textoConteoMenu.text = "Volviendo al menú en " + Mathf.CeilToInt(tiempoRestante) + "...";
            }

            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        if (textoConteoMenu != null)
        {
            textoConteoMenu.text = "Volviendo al menú...";
        }
    }
    #endregion

    #region Audio
    private void ReproducirSonido(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }
    #endregion

    #region Escenas
    private void CargarMenu()
    {
        if (string.IsNullOrEmpty(nombreEscenaMenu))
        {
            Debug.LogWarning("No se asigno el nombre de la escena del menu.");
            return;
        }

        SceneManager.LoadScene(nombreEscenaMenu);
    }
    #endregion
}