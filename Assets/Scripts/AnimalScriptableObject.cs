using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AnimalScriptableObject", order = 1)]
public class AnimalScriptableObject : ScriptableObject
{
    public string m_name;
    public Sprite m_huntIcon;
    public AudioClip m_displaySound;
    public AudioClip m_deathSound;

    [HideInInspector]
    public float m_value;
    [HideInInspector]
    public int m_exp;
    [HideInInspector]
    public int m_level;

    public void Randomise()
    {
        // set random level
        m_level = Random.Range(1, 5);

        switch (m_level)
        {
            case 1:
                m_value = Random.Range(0.01f, 20f);
                m_exp = 125;
                break;
            case 2:
                m_value = Random.Range(20f, 100f);
                m_exp = Random.Range(126, 501);
                break;
            case 3:
                m_value = Random.Range(100f, 250f);
                m_exp = Random.Range(501, 1001);
                break;
            case 4:
                m_value = Random.Range(250f, 500f);
                m_exp = Random.Range(1001, 2001);
                break;
        }
    }
}