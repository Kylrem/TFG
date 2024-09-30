using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoriaRAController : MonoBehaviour
{
    public GameObject[] buttons; // botones de la escena
    public int seqLen = 5; 
    private List<int> genSeq = new List<int>(); 
    private List<int> playSeq = new List<int>();
    public float lightTime = 0.5f; 
    public float delayR = 1.0f; 
    private bool isPlayTime = false;

    private int buttonIndex;

    void Start()
    {
        // Buesca el indice del boton
        buttonIndex = System.Array.IndexOf(buttons, gameObject);
        StartGame();
    }

    void StartGame()
    {

        StopAllCoroutines(); // Detiene posibles corrutinas de ejecuciones anteriores
        playSeq.Clear();
        genSeq.Clear(); 
        GenerateSequence();
        StartCoroutine(PlaySequence());
    }

    // Genera la secuencia aleatoria de botones que el jugador tiene que memorizar
    void GenerateSequence()
    {
        for (int i = 0; i < seqLen; i++)
        {
            int randomIndex = Random.Range(0, buttons.Length);
            genSeq.Add(randomIndex);
        }
    }

    // Muestra la secuencia generada iluminando (activa la emision del material del boton) los botones unos segundos en ese orden y al acabar habilita al jugador
    IEnumerator PlaySequence()
    {
        isPlayTime = false;

        foreach (int index in genSeq)
        {
            LightOn(index);
            yield return new WaitForSeconds(lightTime);

            LightOff(index);
            yield return new WaitForSeconds(0.2f); 
        }

        isPlayTime = true; 
    }

    // Activa la emision en el material del objeto boton
    void LightOn(int index)
    {
        Renderer renderer = buttons[index].GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            if (mat != null)
            {
                mat.EnableKeyword("_EMISSION");
            }
        }
    }

    // Desactiva la emision en el material del objeto boton
    void LightOff(int index)
    {
        Renderer renderer = buttons[index].GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            if (mat != null)
            {
                mat.DisableKeyword("_EMISSION");
            }
        }
    }

    // Detecta cuando las manos del jugador tocan cada objeto boton de la escena
    public void PressButton(int buttonIndex)
    {
        if (!isPlayTime) return;

        playSeq.Add(buttonIndex);
        CheckInput();
    }

    // Verifica la secuencia del jugador (remarcar que con fallar uno ya se genera el codigo de fallado)
    void CheckInput()
    {
        int currentInputIndex = playSeq.Count - 1;

        if (playSeq[currentInputIndex] == genSeq[currentInputIndex])
        {
            // Si es correcta
            if (playSeq.Count == genSeq.Count)
            {
                StartCoroutine(CascadeL()); // Cascada si acierta
            }
        }
        //si no es correcta
        else
        {
            StartCoroutine(BlinkL()); // Parpadeo rápido si falla
        }
    }

    // Si el jugador acierta, las fichas hacen un efecto de cascada de izquiera a derecha y luego dederecha a izquierda antes de mostrar la nueva sequencia
    IEnumerator CascadeL()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            LightOn(i);
            yield return new WaitForSeconds(0.2f);
            LightOff(i);
        }

        for (int i = buttons.Length - 1; i >= 0; i--)
        {
            LightOn(i);
            yield return new WaitForSeconds(0.2f);
            LightOff(i);
        }

        yield return new WaitForSeconds(delayR); 
        StartGame(); 
    }

    // Si el jugador falla, las fichas hacen un efecto de parpadeo rápido 3 veces antes de mostrar la nueva sequencia
    IEnumerator BlinkL()
    {
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                LightOn(i);
            }
            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < buttons.Length; i++)
            {
                LightOff(i);
            }
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(delayR); 
        StartGame();
    }
}
