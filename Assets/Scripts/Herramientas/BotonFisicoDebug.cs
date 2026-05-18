using System.Collections;
using UnityEngine;

public class BotonFisicoDebug : MonoBehaviour
{
    #region Referencias
    [Header("Referencias")]
    public Transform botonVisual;
    #endregion

    #region Configuracion
    [Header("Configuracion")]
    public string mensajeDebug = "Boton fisico pulsado";
    public Vector3 direccionPresionado = new Vector3(0f, -0.03f, 0f);
    public float duracionPresionado = 0.08f;
    public float tiempoAntesDeRegresar = 0.12f;
    public bool permitirRepetir = true;
    public float tiempoEntrePulsaciones = 0.5f;
    #endregion

    #region Variables privadas
    private Vector3 posicionInicialLocal;
    private bool presionando = false;
    private bool enCooldown = false;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (botonVisual != null)
        {
            posicionInicialLocal = botonVisual.localPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (botonVisual == null)
        {
            Debug.LogWarning("No se asigno el botonVisual en " + gameObject.name);
            return;
        }

        if (presionando || enCooldown)
        {
            return;
        }

        Debug.Log(mensajeDebug);
        StartCoroutine(AnimarPulsacion());
    }
    #endregion

    #region Animacion
    private IEnumerator AnimarPulsacion()
    {
        presionando = true;
        enCooldown = true;

        Vector3 posicionPresionada = posicionInicialLocal + direccionPresionado;

        yield return MoverBoton(posicionInicialLocal, posicionPresionada, duracionPresionado);

        yield return new WaitForSeconds(tiempoAntesDeRegresar);

        yield return MoverBoton(posicionPresionada, posicionInicialLocal, duracionPresionado);

        presionando = false;

        if (permitirRepetir)
        {
            yield return new WaitForSeconds(tiempoEntrePulsaciones);
            enCooldown = false;
        }
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
    #endregion
}