using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class TitleManager : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject settingsPanel;
    
    [Header("BGM設定")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioClip titleBGM;

    [Header("設定パネル内容")]
    [SerializeField] private InputField apiKeyInput;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Button settingsCloseButton;
    
    [Header("シーン設定")]
    [SerializeField] private string gameSceneName = "GameScene";
    
    void Start()
    {
        InitializeTitle();
    }
    
    private void InitializeTitle()
    {
        // ボタンにイベントリスナーを追加
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitButtonClicked);
            
        if (settingsCloseButton != null)
            settingsCloseButton.onClick.AddListener(OnSettingsCloseButtonClicked);

        // 設定パネルを初期状態では非表示にする
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
            
        // BGMの設定と再生
        SetupBGM();
        
        // 設定値の読み込み
        LoadSettings();
    }
    
    private void SetupBGM()
    {
        if (bgmAudioSource == null)
        {
            // AudioSourceが設定されていない場合、自動で作成
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
        }
        
        if (titleBGM != null)
        {
            bgmAudioSource.clip = titleBGM;
            bgmAudioSource.loop = true;
            bgmAudioSource.volume = PlayerPrefs.GetFloat("BGMVolume", 0.7f);
            bgmAudioSource.Play();
        }
    }
    
    private void LoadSettings()
    {
        // 保存された設定値を読み込み
        String apiKey = PlayerPrefs.GetString("APIKey");
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.7f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.7f);


        if (apiKeyInput != null)
        {
            apiKeyInput.text = apiKey;
            apiKeyInput.onValueChanged.AddListener(OnAPIKeyChanged);
        }

        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.value = bgmVolume;
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }
            
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = sfxVolume;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        
        // BGM音量を適用
        if (bgmAudioSource != null)
            bgmAudioSource.volume = bgmVolume;
    }
    
    // スタートボタンクリック時の処理
    private void OnStartButtonClicked()
    {
        Debug.Log("ゲームを開始します");
        
        // BGMをフェードアウト（オプション）
        StartCoroutine(FadeOutBGM(1.0f));
        
        // ゲームシーンに遷移
        SceneManager.LoadScene(gameSceneName);
    }
    
    // 設定ボタンクリック時の処理
    private void OnSettingsButtonClicked()
    {
        Debug.Log("設定画面を開きます");
        
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }
    
    // 設定閉じるボタンクリック時の処理
    private void OnSettingsCloseButtonClicked()
    {
        Debug.Log("設定画面を閉じます");
        
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
            
        // 設定を保存
        SaveSettings();
    }

    // API Keyが更新されたとき
    private void OnAPIKeyChanged(String value)
    {
        Debug.Log("API Keyが入力されました");
        PlayerPrefs.SetString("APIKey", value);
    }
    
    // 終了ボタンクリック時の処理
    private void OnExitButtonClicked()
    {
        Debug.Log("ゲームを終了します");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    
    // BGM音量変更時の処理
    private void OnBGMVolumeChanged(float value)
    {
        if (bgmAudioSource != null)
            bgmAudioSource.volume = value;
            
        PlayerPrefs.SetFloat("BGMVolume", value);
    }
    
    // SFX音量変更時の処理
    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        // ここでSFXの音量を更新する処理を追加
    }
    
    private void SaveSettings()
    {
        PlayerPrefs.Save();
        Debug.Log("設定を保存しました");
    }
    
    // BGMフェードアウトのコルーチン
    private System.Collections.IEnumerator FadeOutBGM(float fadeTime)
    {
        if (bgmAudioSource == null) yield break;
        
        float startVolume = bgmAudioSource.volume;
        
        while (bgmAudioSource.volume > 0)
        {
            bgmAudioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        
        bgmAudioSource.Stop();
        bgmAudioSource.volume = startVolume;
    }
    
    // ゲーム終了時の処理
    private void OnDestroy()
    {
        // イベントリスナーのクリーンアップ
        if (startButton != null)
            startButton.onClick.RemoveListener(OnStartButtonClicked);
            
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
            
        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
            
        if (settingsCloseButton != null)
            settingsCloseButton.onClick.RemoveListener(OnSettingsCloseButtonClicked);
            
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
            
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
    }
}