using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    // private string prevSceneName = SceneManager.GetActiveScene().name;
    private string prevSceneName;
    [SerializeField] private string nextSceneName;
    //if input key with change scene
    [SerializeField] private bool requireInput = false;


    //whether player in range
    private bool playerInRange = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        prevSceneName = SceneManager.GetActiveScene().name;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //入った位置を記録
        Vector3 playerPos = other.transform.position;
        //前の場所
        if (prevSceneName == "MainGameScene")
        {
        PlayerPrefs.SetFloat("prevPlayerX", playerPos.x);
        PlayerPrefs.SetFloat("prevPlayerY", playerPos.y - 1.0f);
        }

        if (nextSceneName == "MainGameScene")
        {
            float x = PlayerPrefs.GetFloat("prevPlayerX");
            float y = PlayerPrefs.GetFloat("prevPlayerY");
            PlayerPrefs.SetFloat("SpawnX", x); // 家の玄関の座標
            PlayerPrefs.SetFloat("SpawnY", y);
        }

        if (other.CompareTag("Player"))
        {
            if (requireInput)
            {
                playerInRange = true;
                // display UI message e.g. : press E to enter
                ShowInteractionUI(true);
            }
            else
            {
                // ChangeScene();
                StartCoroutine(ChangeScene());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            ShowInteractionUI(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if need input key to enter
        if (requireInput && playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ChangeScene());
        }
    }

    private IEnumerator ChangeScene()
    {
        //のちに非同期処理
        //UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);

        //データセーブ等
        yield return StartCoroutine(QuitManager.Instance.RequestChangeSceneCoroutine(nextSceneName));

        // SceneManager.LoadScene(nextSceneName);
    }

    private void ShowInteractionUI(bool show)
    {
        GameObject.Find("InteractionUI").SetActive(show);
        // if (show)
        // {
        //     GameObject.Find("InteractionUI").GetComponent<UnityEngine.UI.Text>().text = "Press E to enter";
        // }
        // else
        // {
        //     GameObject.Find("InteractionUI").GetComponent<UnityEngine.UI.Text>().text = "";
        // }
    }
}
