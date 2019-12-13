using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hunt : MonoBehaviour
{
    [SerializeField]
    private GameObject m_animalIcon;
    [SerializeField]
    private RectTransform m_background;
    [SerializeField]
    private GameObject m_huntReadyMarker;

    [Header("AUDIO")]
    [SerializeField]
    private AudioClip m_gunshotAudio;

    [Header("ANIMALS")]
    [SerializeField]
    private AnimalScriptableObject[] m_animals;

    [Header("VALUES")]
    [SerializeField]
    private Vector2 m_randomTimeRangeToStartHunt = new Vector2(3, 7);
    [SerializeField]
    private Vector2 m_randomTimeRangeBetweenAnimalShowing = new Vector2(2, 5);
    [SerializeField]
    private float m_baseAnimalDisplayTime = 2f;
    [SerializeField]
    private float m_animalDisplayTimeReductionPerLevel = 0.5f;

    private AudioSource m_audioSource;
    private Camera m_camera;
    private PlayerController m_player;
    private int m_nChances = 3;

    private int m_iAnimal;

    private void Awake()
    {
        m_player = FindObjectOfType<PlayerController>();
        m_audioSource = GetComponent<AudioSource>();
        m_camera = Camera.main;
    }

    private void OnEnable()
    {
        // get animal
        m_iAnimal = Random.Range(0, m_animals.Length);
        m_animals[m_iAnimal].Randomise();
        m_animalIcon.GetComponent<Button>().image.sprite = m_animals[m_iAnimal].m_huntIcon;

        // invoke hunt indicator after random time
        Invoke("EnableHunt", Random.Range(m_randomTimeRangeToStartHunt.x, m_randomTimeRangeToStartHunt.y));
    }

    private void EnableHunt()
    {
        m_huntReadyMarker.SetActive(true);
        StartCoroutine(WaitForClickOnBush());
    }

    private IEnumerator WaitForClickOnBush()
    {
        bool bBushClicked = false;

        // wait for player to click bush
        while (!bBushClicked)
        {
            // if player left clicks
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                // if left click hits bush
                if (Physics.Raycast(m_camera.ScreenPointToRay(Input.mousePosition), out hit)
                    && hit.collider.GetComponent<Bush>())
                {
                    // set bush to clicked
                    bBushClicked = true;
                    // disable player movement
                    m_player.m_bCanMove = false;
                    // disable marker above bush
                    m_huntReadyMarker.SetActive(false);
                    // enable hunt background
                    m_background.gameObject.SetActive(true);
                    // start hunt coroutines
                    StartCoroutine(DisplayAnimalAfterTime(Random.Range(1f, 5f), m_baseAnimalDisplayTime - (m_animalDisplayTimeReductionPerLevel * m_animals[m_iAnimal].m_level)));
                    StartCoroutine(CountClicks());
                }
            }
            yield return null;
        }
    }

    private IEnumerator CountClicks()
    {
        while (m_nChances > 0)
        {
            yield return null;

            if (Input.GetMouseButtonDown(0))
            {
                m_audioSource.PlayOneShot(m_gunshotAudio);
            }
        }
    }

    private IEnumerator DisplayAnimalAfterTime(float secondsUntilDisplayed, float secondsUntilDisappearing)
    {
        yield return new WaitForSeconds(secondsUntilDisplayed);

        RectTransform rect = m_animalIcon.GetComponent<RectTransform>();

        while (true)
        {
            float x = Random.Range(-m_background.sizeDelta.x + m_background.sizeDelta.x / 2 + rect.sizeDelta.x, m_background.sizeDelta.x / 2 - m_background.sizeDelta.x / 2 - rect.sizeDelta.x / 2);
            float y = Random.Range(-m_background.sizeDelta.y + m_background.sizeDelta.y / 2 + rect.sizeDelta.y, m_background.sizeDelta.y / 2 - m_background.sizeDelta.y / 2 - rect.sizeDelta.y / 2);
            rect.localPosition = new Vector3(x, y);
            m_animalIcon.SetActive(true);
            m_audioSource.PlayOneShot(m_animals[m_iAnimal].m_displaySound);

            yield return new WaitForSeconds(secondsUntilDisappearing);

            m_animalIcon.SetActive(false);

            yield return new WaitForSeconds(Random.Range(m_randomTimeRangeBetweenAnimalShowing.x, m_randomTimeRangeBetweenAnimalShowing.y));
        }
    }

    public void AnimalClicked()
    {
        // play death sound
        m_audioSource.PlayOneShot(m_animals[m_iAnimal].m_deathSound);
        m_background.gameObject.SetActive(false);
        m_animalIcon.SetActive(false);
        m_player.StopHunting();
        StopAllCoroutines();
        enabled = false;
    }
}