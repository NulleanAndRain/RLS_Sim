using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class IkoTarget : MonoBehaviour
{
    public bool IsGroup;
    public bool IsOurs;

    public Vector2 MotionVel;
    public Vector2 StartPos;
    public Transform Center;

    public GameObject TargetPrefab;
    public GameObject TargetPrefabDetermined;

    private bool _isRevealed;

    private float _lineLen;
    private Vector2 _lineStartPos;

    public Vector2 currentPos { get; private set; }

    private void Start()
    {
        transform.position = StartPos;
        _lineLen = Random.Range(
            IkoController.Instance.MinTargetLineLength,
            IkoController.Instance.MaxTargetLineLength
        );
        _lineStartPos = StartPos;
    }

    private void FixedUpdate()
    {
        Vector2 newPos;
        newPos.x = transform.position.x;
        newPos.y = transform.position.y;
        currentPos = newPos + MotionVel * Time.deltaTime;
        transform.position = currentPos;

        if ((currentPos - _lineStartPos).magnitude >= _lineLen)
        {
            _lineStartPos = currentPos;
            var angle = Random.Range(
                -IkoController.Instance._maxTargetAngleDeviation,
                +IkoController.Instance._maxTargetAngleDeviation
            );
            MotionVel = Quaternion.Euler(0, 0, angle) * MotionVel;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Line") return;
        GameObject instance = null;
        if (!_isRevealed)
        {
            instance = Instantiate(TargetPrefab);
            _isRevealed = true;
        }
        else
        {
            instance = Instantiate(TargetPrefabDetermined);
        }

        instance.transform.position = transform.position;
        instance.transform.rotation = Quaternion.Euler(
            0, 
            0, 
            Vector2.SignedAngle(
                Vector2.up, 
                Center.position - transform.position
            )
        );
        instance.transform.SetParent(IkoController.Instance.TargetsFolder, true);
        instance.transform.localScale = Vector3.one;

    }
}
