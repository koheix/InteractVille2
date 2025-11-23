/* QuitManager.cs
 * ゲーム終了時に実行する処理を管理するマネージャークラス
 * セーブ処理やリソース解放など、終了前に行いたい処理を登録する
 * 友ハムのメモリーを保存したりする
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuitManager : MonoBehaviour
{
    public static QuitManager Instance { get; private set; }

    private bool quitting = false;
    private bool return2title = false;

    // 終了時に実行する処理をリストで管理
    private List<IEnumerator> quitTasks = new List<IEnumerator>();
    private List<IEnumerator> return2TitleTasks = new List<IEnumerator>();
    // loading panel
    [SerializeField] private GameObject loadPanel;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // シーン変化を監視
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene pre, Scene next)
    {
        // loading panelを非表示にする
        loadPanel.SetActive(false);
    }

    // 終了処理を追加する
    public void AddQuitTask(IEnumerator task)
    {
        quitTasks.Add(task);
    }

    // 終了ボタンから呼ぶメソッド
    public void RequestQuit()
    {
        // loading panelを表示する
        loadPanel.SetActive(true);

        if (quitting) return;

        quitting = true;
        StartCoroutine(QuitFlow());
    }

    // タイトルへ戻る際の処理を追加する
    public void AddReturn2TitleTask(IEnumerator task)
    {
        return2TitleTasks.Add(task);
    }

    // タイトルへ戻るボタンから呼ぶメソッド
    public void RequestReturn2Title()
    {
        // loading panelを表示する
        loadPanel.SetActive(true);

        if (return2title) return;

        return2title = true;
        StartCoroutine(Return2TitleFlow());
    }

    // 終了処理
    private IEnumerator QuitFlow()
    {
        Debug.Log("終了処理開始…");

        // 登録されてるタスクを順番に全部実行して待つ
        foreach (var task in quitTasks)
            yield return StartCoroutine(task);

        Debug.Log("終了処理完了");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    // タイトルへ戻る処理
    // private IEnumerator Return2TitleFlow()
    // {
    //     Debug.Log("タイトルへ戻る処理開始…");

    //     // 登録されてるタスクを順番に全部実行して待つ
    //     foreach (var task in return2TitleTasks)
    //         yield return StartCoroutine(task);

    //     return2TitleTasks.Clear();
    //     Debug.Log("タイトルへ戻る処理完了");
    //     SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    // }

    // 同時実行版タイトルへ戻る処理
    private IEnumerator Return2TitleFlow()
    {
        Debug.Log("タイトルへ戻る処理開始…");

        // すべてのタスクを同時に開始
        List<Coroutine> runningCoroutines = new List<Coroutine>();
        foreach (var task in return2TitleTasks)
        {
            runningCoroutines.Add(StartCoroutine(task));
        }

        // すべてのタスクの完了を待つ
        foreach (var coroutine in runningCoroutines)
        {
            yield return coroutine;
        }

        return2TitleTasks.Clear();
        Debug.Log("タイトルへ戻る処理完了");
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
}
