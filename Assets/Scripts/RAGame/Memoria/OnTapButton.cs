using Oculus.Interaction;
using System.Collections;
using UnityEngine;

public class OnTapButton : MonoBehaviour
{
    public int buttonIndex;
    public MemoriaRAController gameController;
    private bool isCooldown = false; // Para evitar m�ltiples colisiones en poco tiempo
    public float cooldown = 2f; 

    private void OnCollisionEnter(Collision collision)
    {
        if (isCooldown) return;

        if (collision.gameObject.CompareTag("Hand") || collision.gameObject.name.Contains("Hand"))
        {
            gameController.PressButton(buttonIndex); // Env�a el �ndice del bot�n al controlador

            StartCoroutine(ButtonCooldown()); 
        }
    }

    // A�ade un enfiramiento entre toques de boton para que de tiempo a retirar la mano
    private IEnumerator ButtonCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldown); 
        isCooldown = false; 
    }
}
