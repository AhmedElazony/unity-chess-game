using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    public AudioClip moveAudioClip;
    public AudioClip captureAudioClip;
    public AudioClip notifyAudioClip;
    public AudioClip endGameAudioClip;

    [Header("UI")]
    [SerializeField] private GameObject winningScreen;

    public void OnResetButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }

    public void EndGame(int WinningPlayer)
    {
        winningScreen.SetActive(true);
        winningScreen.transform.GetChild(WinningPlayer).gameObject.SetActive(true);
        PlayAudioClip(endGameAudioClip);
    }

    public void PlayAudioClip(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
