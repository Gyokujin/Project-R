using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    [SerializeField]
    private Reposition backgroundImage;
    [SerializeField]
    private GameObject statusPanel;
    [SerializeField]
    private GameObject itemPanel;
    [SerializeField]
    private GameObject stageName;
    [SerializeField]
    private Sprite[] stageNameSprites;
    [SerializeField]
    private GameObject fadeImage;
    [SerializeField]
    private GameObject eventCut;

    public float fadeTime = 2.5f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
                Destroy(this.gameObject);
        }
    }

    public IEnumerator FadeIn()
    {
        fadeImage.SetActive(true);
        Image fade = fadeImage.GetComponent<Image>();
        float count = fadeTime;

        yield return new WaitForSeconds(1.0f);

        // #. �̹��� ���̵���
        while (count > 0)
        {
            count -= Time.deltaTime;
            fade.color = new Color(0, 0, 0, count);

            yield return null;
        }

        fadeImage.SetActive(fade);
    }

    public IEnumerator FadeOut()
    {
        fadeImage.SetActive(true);
        Image fade = fadeImage.GetComponent<Image>();
        float count = fadeTime;

        yield return new WaitForSeconds(1.0f);

        // #. �̹��� ���̵�ƿ�
        while (count > 0)
        {
            count -= Time.deltaTime;
            fade.color = new Color(0, 0, 0, 1 - count);

            yield return null;
        }
    }

    public void EventCut(bool use)
    {
        eventCut.SetActive(use);
    }

    public IEnumerator StageNameFade()
    {
        stageName.SetActive(true);
        Image stageNameImage = stageName.GetComponent<Image>();
        stageNameImage.sprite = stageNameSprites[GameManager.instance.stageNum];
        float count = fadeTime;

        if (stageNameSprites[GameManager.instance.stageNum] != null)
        {
            // #. �������� �̸� ���̵���
            while (count > 0)
            {
                count -= Time.deltaTime;
                stageNameImage.color = new Color(1, 1, 1, 1 - count);

                yield return null;
            }

            count = fadeTime;

            // #. �������� �̸� ���̵�ƿ�
            while (count > 0)
            {
                count -= Time.deltaTime;
                stageNameImage.color = new Color(1, 1, 1, count);

                yield return null;
            }
        }

        stageName.SetActive(false);
    }

    public void HideUI()
    {
        statusPanel.SetActive(false);
        itemPanel.SetActive(false);
    }

    public void ShowUI()
    {
        statusPanel.SetActive(true);
        itemPanel.SetActive(true);
    }
}