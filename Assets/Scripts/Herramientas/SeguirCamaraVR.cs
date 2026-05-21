using UnityEngine;

public class SeguirCamaraVR : MonoBehaviour
{
    #region Referencias
    [Header("Referencias")]
    public Transform camaraJugador;
    #endregion

    #region Configuracion
    [Header("Configuracion")]
    public float distanciaFrenteCamara = 1.2f;
    public float alturaExtra = -0.1f;
    public float suavizadoPosicion = 10f;
    public float suavizadoRotacion = 10f;
    public bool seguirSoloCuandoEstaActivo = true;
    #endregion

    #region Unity Methods
    private void Start()
    {
        if (camaraJugador == null && Camera.main != null)
        {
            camaraJugador = Camera.main.transform;
        }

        ActualizarPosicionInmediata();
    }

    private void LateUpdate()
    {
        if (seguirSoloCuandoEstaActivo && !gameObject.activeInHierarchy)
        {
            return;
        }

        if (camaraJugador == null)
        {
            return;
        }

        SeguirCamara();
    }
    #endregion

    #region Movimiento
    private void SeguirCamara()
    {
        Vector3 posicionObjetivo = camaraJugador.position + camaraJugador.forward * distanciaFrenteCamara;
        posicionObjetivo.y += alturaExtra;

        Quaternion rotacionObjetivo = Quaternion.LookRotation(transform.position - camaraJugador.position);

        transform.position = Vector3.Lerp(
            transform.position,
            posicionObjetivo,
            Time.deltaTime * suavizadoPosicion
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotacionObjetivo,
            Time.deltaTime * suavizadoRotacion
        );
    }

    public void ActualizarPosicionInmediata()
    {
        if (camaraJugador == null)
        {
            return;
        }

        Vector3 posicionObjetivo = camaraJugador.position + camaraJugador.forward * distanciaFrenteCamara;
        posicionObjetivo.y += alturaExtra;

        transform.position = posicionObjetivo;
        transform.rotation = Quaternion.LookRotation(transform.position - camaraJugador.position);
    }
    #endregion
}