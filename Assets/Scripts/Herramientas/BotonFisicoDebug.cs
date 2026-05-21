using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BotonFisicoInteractivo : MonoBehaviour
{
    #region Referencias
    [Header("Referencias")]
    public Transform botonVisual;
    public Light luzFeedback;
    #endregion

    #region Configuracion
    [Header("Configuracion")]
    public string mensajeDebug = "Boton fisico pulsado";
    public string tagActivador = "Hand";

    public Vector3 direccionPresionado = new Vector3(0f, -0.03f, 0f);
    public float duracionPresionado = 0.08f;
    public float tiempoAntesDeRegresar = 0.12f;
    public float tiempoEntrePulsaciones = 0.5f;

    [Header("Feedback luz")]
    public bool apagarLuzAlIniciar = true;
    public float tiempoLuzEncendida = 0.25f;

    [Header("Eventos")]
    public UnityEvent alPulsar;
    #endregion

    #region Variables privadas
    private Vector3 posicionInicialLocal;
    private bool enProceso = false;
    private bool estadoInicialLuz = false;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (botonVisual != null)
        {
            posicionInicialLocal = botonVisual.localPosition;
        }

        if (luzFeedback != null)
        {
            estadoInicialLuz = luzFeedback.enabled;

            if (apagarLuzAlIniciar)
            {
                luzFeedback.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagActivador))
        {
            return;
        }

        if (enProceso)
        {
            return;
        }

        Debug.Log(mensajeDebug);

        if (alPulsar != null)
        {
            alPulsar.Invoke();
        }

        StartCoroutine(AnimarPulsacion());
    }
    #endregion

    #region Animacion
    private IEnumerator AnimarPulsacion()
    {
        enProceso = true;

        if (luzFeedback != null)
        {
            StartCoroutine(ActivarLuzTemporal());
        }

        Vector3 posicionPresionada = posicionInicialLocal + direccionPresionado;

        yield return MoverBoton(posicionInicialLocal, posicionPresionada, duracionPresionado);
        yield return new WaitForSeconds(tiempoAntesDeRegresar);
        yield return MoverBoton(posicionPresionada, posicionInicialLocal, duracionPresionado);

        yield return new WaitForSeconds(tiempoEntrePulsaciones);

        enProceso = false;
    }

    private IEnumerator MoverBoton(Vector3 desde, Vector3 hasta, float duracion)
    {
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;

            botonVisual.localPosition = Vector3.Lerp(desde, hasta, t);

            yield return null;
        }

        botonVisual.localPosition = hasta;
    }

    private IEnumerator ActivarLuzTemporal()
    {
        luzFeedback.enabled = true;

        yield return new WaitForSeconds(tiempoLuzEncendida);

        if (apagarLuzAlIniciar)
        {
            luzFeedback.enabled = false;
        }
        else
        {
            luzFeedback.enabled = estadoInicialLuz;
        }
    }
    #endregion
}