using System.Collections;
using UnityEngine;

public class RuntimeHandFingerCollider : MonoBehaviour
{
    #region Configuracion
    [Header("Configuracion")]
    public string nombreMano = "RightHand(Clone)";
    public string tagDedo = "Hand";
    public float radioCollider = 0.018f;
    public bool mostrarCollider = false;
    #endregion

    #region Variables privadas
    private bool colliderCreado = false;
    #endregion

    #region Unity Methods
    private IEnumerator Start()
    {
        while (!colliderCreado)
        {
            IntentarCrearCollider();
            yield return new WaitForSeconds(0.5f);
        }
    }
    #endregion

    #region Logica
    private void IntentarCrearCollider()
    {
        GameObject mano = GameObject.Find(nombreMano);

        if (mano == null)
        {
            return;
        }

        Transform puntaIndice = BuscarPuntaIndice(mano.transform);

        if (puntaIndice == null)
        {
            Debug.LogWarning("No se encontro la punta del dedo indice en " + nombreMano);
            return;
        }

        GameObject colliderDedo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        colliderDedo.name = "IndexFingerCollider";
        colliderDedo.transform.SetParent(puntaIndice);
        colliderDedo.transform.localPosition = Vector3.zero;
        colliderDedo.transform.localRotation = Quaternion.identity;
        colliderDedo.transform.localScale = Vector3.one * radioCollider;

        SphereCollider sphereCollider = colliderDedo.GetComponent<SphereCollider>();
        sphereCollider.isTrigger = false;
        sphereCollider.radius = 0.5f;

        Rigidbody rb = colliderDedo.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        colliderDedo.tag = tagDedo;

        Renderer renderer = colliderDedo.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = mostrarCollider;
        }

        colliderCreado = true;

        Debug.Log("Collider de dedo creado en " + puntaIndice.name);
    }

    private Transform BuscarPuntaIndice(Transform raiz)
    {
        Transform[] hijos = raiz.GetComponentsInChildren<Transform>(true);

        foreach (Transform hijo in hijos)
        {
            string nombre = hijo.name.ToLower();

            if (nombre.Contains("indextip") ||
                nombre.Contains("index_tip") ||
                nombre.Contains("hand_indextip") ||
                nombre.Contains("indexdistal") ||
                nombre.Contains("index_distal"))
            {
                return hijo;
            }
        }

        return null;
    }
    #endregion
}