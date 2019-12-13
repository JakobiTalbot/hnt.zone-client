using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntReadyMarker : MonoBehaviour
{
    [SerializeField]
    private float m_bobSpeed = 50f;
    [SerializeField]
    private float m_bobDistance = 0.05f;

    private Transform m_transform;
    private Vector3 m_origin;

    private void Awake()
    {
        m_transform = transform;
        m_origin = m_transform.position;
    }

    private void Update()
    {
        m_transform.position = m_origin + m_transform.up * Mathf.Sin(Time.time * m_bobSpeed) * 0.05f;
    }
}