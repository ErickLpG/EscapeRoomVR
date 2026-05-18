using UnityEngine;

public class UIButtonDebug : MonoBehaviour
{
    public void ImprimirMensajeBoton(string texto)
    {
        Debug.Log("Boton pulsado: " + texto);
    }
}