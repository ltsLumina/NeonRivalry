using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VInspector;

public class SettingsManager : MonoBehaviour
{
    [Tab("Rumble")]
    [Header("Rumble")]
    [SerializeField] Slider player1RumbleStrength;
    [SerializeField] Slider player2RumbleStrength;
    
    [Tab("Volume")]
    [Header("Volume")]
    [SerializeField] Slider masterVolume;
    [SerializeField] Slider musicVolume;
    [SerializeField] Slider sfxVolume;

    AudioMixer mixer;
    
    // resolutions
    Resolution[] resolutions;
    
    void Start()
    {
        LoadRumble(player1RumbleStrength, player2RumbleStrength);
        
        mixer = Resources.Load<AudioMixer>("Melenitas Dev/Sounds Good/Outputs/Master");
        
        // If intro scene, return.
        if (SceneManagerExtended.ActiveScene is 0) return;
        
        player1RumbleStrength.onValueChanged.AddListener(SetPlayer1RumbleStrength);
        player2RumbleStrength.onValueChanged.AddListener(SetPlayer2RumbleStrength);
        
        masterVolume.onValueChanged.AddListener(SetMasterVolume);
        musicVolume.onValueChanged.AddListener(SetMusicVolume);
        sfxVolume.onValueChanged.AddListener(SetSFXVolume);
    }

    public static float Player1RumbleStrength => PlayerPrefs.GetFloat("Player1_RumbleStrength", 1);
    public static float Player2RumbleStrength => PlayerPrefs.GetFloat("Player2_RumbleStrength", 1);
    
    public static float MasterVolume => PlayerPrefs.GetFloat("Master", 1);
    public static float MusicVolume  => PlayerPrefs.GetFloat("Music", 1);
    public static float SFXVolume    => PlayerPrefs.GetFloat("SFX", 1);

    static void SetPlayer1RumbleStrength(float value) => PlayerPrefs.SetFloat("Player1_RumbleStrength", value);

    static void SetPlayer2RumbleStrength(float value) => PlayerPrefs.SetFloat("Player2_RumbleStrength", value);
    
    public static void LoadRumble(Slider player1_Slider, Slider player2_Slider)
    {
        if (player1_Slider == null || player2_Slider == null) return;
        
        player1_Slider.value = Player1RumbleStrength;
        player2_Slider.value = Player2RumbleStrength;
    }
    
    void SetMasterVolume(float value)
    {
        mixer.SetFloat("Master", Mathf.Log10(value) * 40);
        PlayerPrefs.SetFloat("Master", value);
    }
    
    void SetMusicVolume(float value)
    {
        mixer.SetFloat("Music", Mathf.Log10(value) * 40);
        PlayerPrefs.SetFloat("Music", value);
    }
    
    void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFX", Mathf.Log10(value) * 40);
        PlayerPrefs.SetFloat("SFX", value);
    }
    
    /// <summary>
    /// Loads the volume settings from the PlayerPrefs.
    /// The slider parameters are optional, and are used to set the sliders' values,
    /// meaning if there are no sliders, the method will only load the volume settings.
    /// </summary>
    /// <param name="masterVolume"></param>
    /// <param name="musicVolume"></param>
    /// <param name="sfxVolume"></param>
    public static void LoadVolume(Slider masterVolume = default, Slider musicVolume = default, Slider sfxVolume = default)
    {
        var audioMixer = Resources.Load<AudioMixer>("Melenitas Dev/Sounds Good/Outputs/Master");
        
        audioMixer.SetFloat("Master", Mathf.Log10(MasterVolume) * 40);
        audioMixer.SetFloat("Music", Mathf.Log10(MusicVolume) * 40);
        audioMixer.SetFloat("SFX", Mathf.Log10(SFXVolume) * 40);
        
        if (masterVolume == null || musicVolume == null || sfxVolume == null) return;
        
        masterVolume.value = MasterVolume;
        musicVolume.value  = MusicVolume;
        sfxVolume.value    = SFXVolume;
    }

    public void Rumble(int playerID)
    {
        float   speed   = PlayerPrefs.GetFloat(playerID == 1 ? "Player1_RumbleStrength" : "Player2_RumbleStrength");
        Gamepad gamepad;

        if (playerID == 1) 
             gamepad = InputDeviceManager.PlayerOneDevice as Gamepad;
        else gamepad = InputDeviceManager.PlayerTwoDevice as Gamepad;
        
        if (gamepad == null) return;

        StartCoroutine(RumbleCoroutine());
        
        return; // Local function
        IEnumerator RumbleCoroutine()
        {
            gamepad?.SetMotorSpeeds(speed, speed);
            yield return new WaitForSeconds(0.085f);
            gamepad?.SetMotorSpeeds(0, 0);
        }
    }
}
