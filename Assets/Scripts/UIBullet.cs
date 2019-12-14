using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBullet : MonoBehaviour
{
    [SerializeField]
    private Vector2 m_randomUpVelocityOnFire = new Vector2(1, 2);
    [SerializeField]
    private float m_activeTime = 3f;
    [SerializeField]
    private float m_fallAcceleration = 5f;
    [SerializeField]
    private GameObject m_bullet;

    private Transform m_transform;
    private Rigidbody m_rb;
    private Vector3 m_startPos;
    private Quaternion m_startRot;
    private bool m_bActive = true;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_transform = transform;
        m_startPos = m_transform.position;
        m_startRot = m_transform.rotation;
    }

    private void OnEnable()
    {
        // reset values
        m_bullet.SetActive(true);
        m_transform.position = m_startPos;
        m_transform.rotation = m_startRot;
        m_rb.angularVelocity = Vector3.zero;
    }

    public IEnumerator Fire()
    {
        m_bullet.SetActive(false);
        m_bActive = true;
        Vector3 velocity = Vector3.zero;
        Vector3 downDir = -m_transform.up;

        Debug.Log(m_transform.up.y);
        velocity = m_transform.up * Random.Range(m_randomUpVelocityOnFire.x, m_randomUpVelocityOnFire.y);

        // set random angular velocity
        m_rb.angularVelocity = new Vector3(Random.Range(-90, 90), Random.Range(-90, 90), Random.Range(-90, 90));

        StartCoroutine(StopAfterTime(m_activeTime));

        while (m_bActive)
        {
            m_transform.position += velocity * Time.deltaTime;

            // fake gravity
            velocity += downDir * m_fallAcceleration * Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator StopAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        m_bActive = false;
    }
}