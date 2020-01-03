using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [HideInInspector]
    public bool m_bCanMove = true;

    [SerializeField]
    private float m_moveSpeed = 10f;
    [SerializeField]
    private HeadMovement m_head;
    [SerializeField]
    private Hunt m_hunt;
    [SerializeField]
    private GameObject m_gunPivot;
    [SerializeField]
    private LineRenderer m_laser;
    [SerializeField]
    private ParticleSystem m_smokesplosion;

    private Camera m_camera;
    private Renderer m_renderer;
    private Transform m_transform;
    private Coroutine m_currentMovementCoroutine;

    private float m_startHeight = 10f;

    private Vector3 m_rayHitPos;

    private bool m_bHunting = false;

    void Awake()
    {
        m_transform = transform;
        m_renderer = GetComponent<Renderer>();
        m_camera = Camera.main;
        m_smokesplosion.Play(false);
    }

    void Update()
    {
        // only take input from local player
        if (isLocalPlayer)
        {
            if (m_bCanMove && Input.GetMouseButton(0))
            {
                RaycastHit hit;
                // if clicked on ground
                if (Physics.Raycast(m_camera.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.collider.GetComponent<Ground>())
                    {
                        if (m_currentMovementCoroutine != null)
                            StopCoroutine(m_currentMovementCoroutine);
                        m_currentMovementCoroutine = StartCoroutine(LerpToPos(hit.point));
                    }
                }
            }

            if (!m_bHunting && Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                // if clicked on ground
                if (Physics.Raycast(m_camera.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.collider.GetComponent<Bush>())
                    {

                        m_gunPivot.SetActive(true);

                        m_rayHitPos = hit.point;

                        m_bHunting = true;
                        m_hunt.enabled = true;
                    }
                }
            }
        }

        if (m_gunPivot.activeSelf)
        {
            // rotate towards bush
            Vector3 targetRot = m_rayHitPos;
            targetRot.y = m_transform.position.y;
            m_gunPivot.transform.rotation = Quaternion.LookRotation(targetRot - m_transform.position);
            // generate laser
            Vector3[] points = new Vector3[2];
            points[0] = m_laser.transform.position;
            points[1] = m_rayHitPos;
            m_laser.SetPositions(points);
        }
        m_renderer.material.SetVector("_HeadPosition", m_head.transform.position - m_transform.position);
    }

    private IEnumerator LerpToPos(Vector3 targetPos)
    {
        targetPos.y = m_transform.position.y;
        m_gunPivot.transform.rotation = Quaternion.LookRotation(targetPos - m_transform.position);
        while (Vector3.Distance(m_transform.position, targetPos) > 0.1f)
        {
            m_transform.position -= (m_transform.position - targetPos) * Time.deltaTime * m_moveSpeed;
            yield return null;
        }
    }

    public void StopHunting()
    {
        m_bCanMove = true;
        m_bHunting = false;
        m_gunPivot.SetActive(false);
    }

    private void OnDestroy()
    {
        Destroy(m_head.gameObject);
    }
}