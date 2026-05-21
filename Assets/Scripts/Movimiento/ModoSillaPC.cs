using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModoSillaPC : MonoBehaviour
{
    #region Referencias
    [Header("Referencias")]
    public Transform xrOrigin;
    public Transform camaraJugador;
    public Transform puntoSentadoPC;
    public Transform puntoSalidaSilla;

    [Header("Objetos a desactivar durante modo silla")]
    public Behaviour[] componentesLocomocion;
    public Collider[] collidersADesactivar;
    public GameObject[] objetosADesactivar;
    #endregion

    #region Input
    [Header("Input")]
    public InputActionReference accionSalirSilla;
    #endregion

    #region Configuracion
    [Header("Configuracion")]
    public string tagJugador = "Player";
    public float duracionMovimiento = 0.35f;
    public bool entrarAutomaticamente = true;
    public bool guardarPosicionAntesDeSentarse = true;

    [Header("Ajuste VR")]
    public bool alinearCamaraConPunto = true;
    public bool mantenerAlturaActualDelOrigin = false;

    [Header("Proteccion de reentrada")]
    public float tiempoBloqueoEntradaTrasSalir = 1.5f;
    #endregion

    #region Debug
    [Header("Debug")]
    public bool mostrarDebug = true;
    #endregion

    #region UI Opcional
    [Header("UI Opcional")]
    public GameObject panelModoSilla;
    #endregion

    #region Variables privadas
    private bool enModoSilla = false;
    private bool moviendo = false;
    private bool entradaBloqueadaTemporalmente = false;

    private Vector3 posicionAntesDeSentarse;
    private Quaternion rotacionAntesDeSentarse;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        Log("Awake ejecutado.");

        if (panelModoSilla != null)
        {
            panelModoSilla.SetActive(false);
            Log("Panel modo silla desactivado en Awake.");
        }

        if (camaraJugador == null && Camera.main != null)
        {
            camaraJugador = Camera.main.transform;
            Log("Camara jugador asignada automaticamente: " + camaraJugador.name);
        }

        ValidarReferencias();
    }

    private void OnEnable()
    {
        if (accionSalirSilla != null)
        {
            accionSalirSilla.action.Enable();
            accionSalirSilla.action.performed += OnSalirSilla;
            Log("Input salir silla habilitado: " + accionSalirSilla.name);
        }
        else
        {
            LogWarning("No se asigno accionSalirSilla.");
        }
    }

    private void OnDisable()
    {
        if (accionSalirSilla != null)
        {
            accionSalirSilla.action.performed -= OnSalirSilla;
            accionSalirSilla.action.Disable();
            Log("Input salir silla deshabilitado.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Log("OnTriggerEnter detectado con: " + other.name + " | Tag: " + other.tag);

        if (!entrarAutomaticamente)
        {
            Log("Entrada ignorada: entrarAutomaticamente esta desactivado.");
            return;
        }

        if (entradaBloqueadaTemporalmente)
        {
            Log("Entrada ignorada: entrada bloqueada temporalmente tras salir.");
            return;
        }

        if (!other.CompareTag(tagJugador))
        {
            Log("Entrada ignorada: el tag no coincide. Esperado: " + tagJugador + " | Recibido: " + other.tag);
            return;
        }

        Log("Jugador entro en zona de modo silla PC.");
        EntrarModoSilla();
    }

    private void OnTriggerExit(Collider other)
    {
        Log("OnTriggerExit detectado con: " + other.name + " | Tag: " + other.tag);
    }
    #endregion

    #region Entrada y salida modo silla
    public void EntrarModoSilla()
    {
        Log("Solicitud EntrarModoSilla. enModoSilla: " + enModoSilla + " | moviendo: " + moviendo);

        if (enModoSilla)
        {
            Log("No entra: ya esta en modo silla.");
            return;
        }

        if (moviendo)
        {
            Log("No entra: actualmente se esta moviendo.");
            return;
        }

        if (xrOrigin == null || puntoSentadoPC == null)
        {
            LogWarning("Faltan referencias para entrar al modo silla.");
            return;
        }

        if (guardarPosicionAntesDeSentarse)
        {
            posicionAntesDeSentarse = xrOrigin.position;
            rotacionAntesDeSentarse = xrOrigin.rotation;

            Log("Posicion antes de sentarse guardada: " + posicionAntesDeSentarse);
            Log("Rotacion antes de sentarse guardada: " + rotacionAntesDeSentarse.eulerAngles);
        }

        enModoSilla = true;

        ActivarLocomocion(false);

        if (panelModoSilla != null)
        {
            panelModoSilla.SetActive(true);
            Log("Panel modo silla activado.");
        }

        Vector3 destinoOrigin = CalcularPosicionOriginParaCamara(puntoSentadoPC);
        Quaternion destinoRotacion = puntoSentadoPC.rotation;

        Log("Entrando a modo silla PC.");
        Log("XR Origin posicion inicial: " + xrOrigin.position);
        Log("Camara posicion inicial: " + ObtenerPosicionCamaraTexto());
        Log("Punto sentado posicion: " + puntoSentadoPC.position);
        Log("Destino calculado para XR Origin: " + destinoOrigin);
        Log("Destino rotacion: " + destinoRotacion.eulerAngles);

        StartCoroutine(MoverXRiginAPunto(destinoOrigin, destinoRotacion, "ENTRAR"));
    }

    public void SalirModoSilla()
    {
        Log("Solicitud SalirModoSilla. enModoSilla: " + enModoSilla + " | moviendo: " + moviendo);

        if (!enModoSilla)
        {
            Log("No sale: no esta en modo silla.");
            return;
        }

        if (moviendo)
        {
            Log("No sale: actualmente se esta moviendo.");
            return;
        }

        Vector3 posicionDestino;
        Quaternion rotacionDestino;

        if (puntoSalidaSilla != null)
        {
            posicionDestino = CalcularPosicionOriginParaCamara(puntoSalidaSilla);
            rotacionDestino = puntoSalidaSilla.rotation;

            Log("Usando puntoSalidaSilla.");
            Log("Punto salida posicion: " + puntoSalidaSilla.position);
        }
        else
        {
            posicionDestino = posicionAntesDeSentarse;
            rotacionDestino = rotacionAntesDeSentarse;

            Log("No hay puntoSalidaSilla. Usando posicion guardada antes de sentarse.");
        }

        Log("Saliendo de modo silla PC.");
        Log("XR Origin posicion actual: " + xrOrigin.position);
        Log("Camara posicion actual: " + ObtenerPosicionCamaraTexto());
        Log("Destino salida calculado para XR Origin: " + posicionDestino);
        Log("Rotacion salida: " + rotacionDestino.eulerAngles);

        StartCoroutine(SalirModoSillaCoroutine(posicionDestino, rotacionDestino));
    }

    private IEnumerator SalirModoSillaCoroutine(Vector3 posicionDestino, Quaternion rotacionDestino)
    {
        yield return MoverXRiginAPunto(posicionDestino, rotacionDestino, "SALIR");

        enModoSilla = false;

        if (panelModoSilla != null)
        {
            panelModoSilla.SetActive(false);
            Log("Panel modo silla desactivado.");
        }

        entradaBloqueadaTemporalmente = true;
        Log("Entrada bloqueada temporalmente por " + tiempoBloqueoEntradaTrasSalir + " segundos.");

        ActivarLocomocion(true);

        yield return new WaitForSeconds(tiempoBloqueoEntradaTrasSalir);

        entradaBloqueadaTemporalmente = false;
        Log("Entrada desbloqueada. Ya puede volver a entrar a modo silla.");
    }

    private void OnSalirSilla(InputAction.CallbackContext context)
    {
        Log("Input salir silla recibido. Control: " + context.control.name + " | Fase: " + context.phase);
        SalirModoSilla();
    }
    #endregion

    #region Movimiento XR Origin
    private IEnumerator MoverXRiginAPunto(Vector3 posicionDestino, Quaternion rotacionDestino, string motivo)
    {
        moviendo = true;

        Vector3 posicionInicial = xrOrigin.position;
        Quaternion rotacionInicial = xrOrigin.rotation;

        Log("Iniciando movimiento " + motivo);
        Log("Desde posicion: " + posicionInicial);
        Log("Hacia posicion: " + posicionDestino);
        Log("Desde rotacion: " + rotacionInicial.eulerAngles);
        Log("Hacia rotacion: " + rotacionDestino.eulerAngles);

        float tiempo = 0f;

        while (tiempo < duracionMovimiento)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionMovimiento;

            xrOrigin.position = Vector3.Lerp(posicionInicial, posicionDestino, t);
            xrOrigin.rotation = Quaternion.Slerp(rotacionInicial, rotacionDestino, t);

            yield return null;
        }

        xrOrigin.position = posicionDestino;
        xrOrigin.rotation = rotacionDestino;

        Log("Movimiento " + motivo + " terminado.");
        Log("XR Origin posicion final: " + xrOrigin.position);
        Log("Camara posicion final: " + ObtenerPosicionCamaraTexto());

        moviendo = false;
    }

    private Vector3 CalcularPosicionOriginParaCamara(Transform puntoObjetivoCamara)
    {
        if (!alinearCamaraConPunto || camaraJugador == null)
        {
            Log("Calculo posicion: usando posicion directa del punto objetivo.");
            return puntoObjetivoCamara.position;
        }

        Vector3 diferencia = puntoObjetivoCamara.position - camaraJugador.position;
        Vector3 posicionCalculada = xrOrigin.position + diferencia;

        if (mantenerAlturaActualDelOrigin)
        {
            posicionCalculada.y = xrOrigin.position.y;
        }

        Log("Calculo posicion para alinear camara.");
        Log("Camara actual: " + camaraJugador.position);
        Log("Punto objetivo camara: " + puntoObjetivoCamara.position);
        Log("Diferencia aplicada al XR Origin: " + diferencia);
        Log("Posicion calculada XR Origin: " + posicionCalculada);

        return posicionCalculada;
    }
    #endregion

    #region Locomocion
    private void ActivarLocomocion(bool activar)
    {
        Log("ActivarLocomocion: " + activar);

        if (componentesLocomocion != null)
        {
            for (int i = 0; i < componentesLocomocion.Length; i++)
            {
                if (componentesLocomocion[i] != null)
                {
                    componentesLocomocion[i].enabled = activar;
                    Log("Componente locomocion [" + i + "] " + componentesLocomocion[i].name + " -> " + activar);
                }
                else
                {
                    Log("Componente locomocion [" + i + "] esta vacio.");
                }
            }
        }

        if (collidersADesactivar != null)
        {
            for (int i = 0; i < collidersADesactivar.Length; i++)
            {
                if (collidersADesactivar[i] != null)
                {
                    collidersADesactivar[i].enabled = activar;
                    Log("Collider [" + i + "] " + collidersADesactivar[i].name + " -> " + activar);
                }
                else
                {
                    Log("Collider [" + i + "] esta vacio.");
                }
            }
        }

        if (objetosADesactivar != null)
        {
            for (int i = 0; i < objetosADesactivar.Length; i++)
            {
                if (objetosADesactivar[i] != null)
                {
                    objetosADesactivar[i].SetActive(activar);
                    Log("GameObject [" + i + "] " + objetosADesactivar[i].name + " -> " + activar);
                }
                else
                {
                    Log("GameObject [" + i + "] esta vacio.");
                }
            }
        }
    }
    #endregion

    #region Validacion y debug
    private void ValidarReferencias()
    {
        Log("Validando referencias.");

        if (xrOrigin == null)
        {
            LogWarning("xrOrigin no asignado.");
        }

        if (camaraJugador == null)
        {
            LogWarning("camaraJugador no asignada.");
        }

        if (puntoSentadoPC == null)
        {
            LogWarning("puntoSentadoPC no asignado.");
        }

        if (accionSalirSilla == null)
        {
            LogWarning("accionSalirSilla no asignada.");
        }

        Collider trigger = GetComponent<Collider>();

        if (trigger == null)
        {
            LogWarning("Este objeto no tiene Collider. OnTriggerEnter no funcionara.");
        }
        else
        {
            Log("Collider de zona encontrado: " + trigger.GetType().Name + " | IsTrigger: " + trigger.isTrigger);

            if (!trigger.isTrigger)
            {
                LogWarning("El Collider de la zona no tiene Is Trigger activado.");
            }
        }
    }

    private string ObtenerPosicionCamaraTexto()
    {
        if (camaraJugador == null)
        {
            return "Camara no asignada";
        }

        return camaraJugador.position.ToString();
    }

    private void Log(string mensaje)
    {
        if (!mostrarDebug)
        {
            return;
        }

        Debug.Log("[ModoSillaPC] " + mensaje);
    }

    private void LogWarning(string mensaje)
    {
        Debug.LogWarning("[ModoSillaPC] " + mensaje);
    }
    #endregion
}