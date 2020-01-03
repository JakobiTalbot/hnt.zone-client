using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMovement : MonoBehaviour
{
    [SerializeField]
    private Transform m_bodyTransform;
    [SerializeField]
    private float m_headElasticity = 0.1f;

    private Transform m_transform;

    private Vector3 m_headOffset;
    private Vector3 m_headVelocity;

    void Awake()
    {
        m_transform = transform;
        m_transform.parent = null;

        m_headOffset = m_transform.position - m_bodyTransform.position;
    }

    private void Update()
    {
        m_headVelocity = Vector3.Lerp(m_headVelocity, ((m_bodyTransform.position + m_headOffset) - m_transform.position) * 0.5f, m_headElasticity);
        m_transform.position += m_headVelocity;
    }

    public void MoveHead(Vector3 targetPos)
    {
        m_headVelocity = Vector3.Lerp(m_headVelocity, (targetPos - m_transform.position), m_headElasticity);
        m_transform.position += m_headVelocity;
    }
}