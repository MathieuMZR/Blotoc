using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CubeObject : MonoBehaviour
{
    public float moveDuration;
    public float speedModifier = 1f;
    public Vector3 direction;
    public bool isInvisible;
    
    [SerializeField] private Ease moveCurve;
    [SerializeField] private Ease rotateCurve;
    
    [SerializeField] private Transform pivot;
    [SerializeField] private ParticleSystem psDestroy;

    private Collider _col;

    private void Awake()
    {
        _col = GetComponent<Collider>();
    }

    private void Start()
    {
        CollisionManaging(false);
        SpawnAnimation();
    }

    void SpawnAnimation()
    {
        var basePos = transform.position += Vector3.up;
        transform.position -= new Vector3(0, 2, 0);
        transform.DOMove(basePos, 1f).SetEase(Ease.OutQuart);
        
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            CollisionManaging(true);
            StartCoroutine(CubeMove());
        });
    }

    public void CollisionManaging(bool enable)
    {
        _col.enabled = enable;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle")
            || other.gameObject.layer == LayerMask.NameToLayer("Cube"))
        {
            Instantiate(psDestroy, transform.position, Quaternion.identity);
            
            GameManager.Instance.EndGame();

            var cam = GameManager.Instance.cam;
            cam.transform.DOKill();
            cam.DOShakePosition(.5f, .15f, 100).SetEase(Ease.OutSine);
            
            Destroy(gameObject);
        }
    }

    public IEnumerator CubeMove()
    {
        var pos = transform.position;
        var newPos = pos + direction * GameManager.Instance.tileSize;
        
        pivot.transform.rotation = Quaternion.identity;
        var rot = pivot.transform.rotation * GetAngleFromDirectionMoving();

        transform.DOMove(newPos, moveDuration / speedModifier).SetEase(moveCurve);
        pivot.transform.DORotateQuaternion(rot, moveDuration / speedModifier).SetEase(rotateCurve);

        yield return new WaitForSeconds((moveDuration / speedModifier) * 1.5f);

        StartCoroutine(CubeMove());
    }

    public float GetMoveDelay() => (moveDuration / speedModifier);

    public void SetNewDirection(Vector3 newDir)
    {
        direction = newDir;
    }

    public void SetNewSpeedModifier(float m)
    {
        speedModifier = m;
    }

    Quaternion GetAngleFromDirectionMoving()
    {
        var treshold = 0.5f;
        var angle = new Vector3(0,0,0);
        
        if (direction.x > treshold)
        {
            angle = new Vector3(0, 0, -90);
        }
        else if (direction.x < -treshold)
        {
            angle = new Vector3(0, 0, 90);
        }
        else if (direction.z > treshold)
        {
            angle = new Vector3(90, 0, 0);
        }
        else if (direction.z < -treshold)
        {
            angle = new Vector3(-90, 0, 0);
        }

        return Quaternion.Euler(angle);
    }

    [SerializeField] private int blinkAmount = 5;
    [SerializeField] private float blinkDelay = 0.05f;
    public IEnumerator InvisibleBlink()
    {
        var mr = GetComponentInChildren<MeshRenderer>();
        for (int i = 0; i < blinkAmount; i++)
        {
            mr.enabled = isInvisible;
            yield return new WaitForSeconds(blinkDelay);
            mr.enabled = !isInvisible;
            yield return new WaitForSeconds(blinkDelay);
        }
        
        if (!isInvisible) yield break;

        while (isInvisible)
        {
            yield return new WaitForSeconds(GetMoveDelay() * 3f);
            for (int i = 0; i < 2; i++)
            {
                mr.enabled = isInvisible;
                yield return new WaitForSeconds(blinkDelay);
                mr.enabled = !isInvisible;
                yield return new WaitForSeconds(blinkDelay);
            }
        }
    }
}