using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Musica")]
    [SerializeField] public AudioClip musica;
    [SerializeField] public AudioClip sonidoGameOver;

    [Header("Efectos")]
    [SerializeField] public AudioClip sonidoSalto;
    [SerializeField] public AudioClip sonidoEsquive;

    private AudioSource[] sources;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        sources = GetComponents<AudioSource>();
    }

    void Start()
    {
        if (sources.Length > 0 && musica != null)
        {
            sources[0].clip = musica;
            sources[0].loop = true;
            sources[0].volume = 0.5f;
            sources[0].Play();
        }
    }

    public void PararMusica()
    {
        if (sources.Length > 0) sources[0].Stop();
    }

    public void ReproducirGameOver()
    {
        PararMusica();
        if (sources.Length > 1 && sonidoGameOver != null)
            sources[1].PlayOneShot(sonidoGameOver);
        else if (sources.Length > 0 && sonidoGameOver != null)
            sources[0].PlayOneShot(sonidoGameOver);
    }

    public void ReproducirSalto()
    {
        if (sources.Length > 0 && sonidoSalto != null)
            sources[0].PlayOneShot(sonidoSalto, 0.7f);
    }

    public void ReproducirEsquive()
    {
        if (sources.Length > 0 && sonidoEsquive != null)
            sources[0].PlayOneShot(sonidoEsquive, 0.5f);
    }
}
