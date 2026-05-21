using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicioVR : MonoBehaviour
{
    #region Referencias UI
    [Header("Paneles")]
    public GameObject panelMensajeSalir;
    #endregion

    #region Configuracion
    [Header("Escenas")]
    public string nombreEscenaJuego = "EscenaJuego";
    #endregion

    #region Unity Methods
    private void Start()
    {
        if (panelMensajeSalir != null)
        {
            panelMensajeSalir.SetActive(false);
        }
    }
    #endregion

    #region Botones menu
    public void IniciarJuego()
    {
        Debug.Log("Iniciando juego");

        if (string.IsNullOrEmpty(nombreEscenaJuego))
        {
            Debug.LogWarning("No se asigno el nombre de la escena del juego.");
            return;
        }

        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void MostrarMensajeSalir()
    {
        Debug.Log("Mostrando mensaje de salida");

        if (panelMensajeSalir != null)
        {
            if(panelMensajeSalir.activeSelf)
            {
                SalirDelJuego();
            }
            else{
                panelMensajeSalir.SetActive(true);
            }
        }
    }

    public void OcultarMensajeSalir()
    {
        Debug.Log("Cancelando salida");

        if (panelMensajeSalir != null)
        {
            panelMensajeSalir.SetActive(false);
        }
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego");

        Application.Quit();
    }
    #endregion
}