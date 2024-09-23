using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CubeObject : MonoBehaviour
{
    public float moveDuration;
    
    [SerializeField] private Ease moveCurve;
    [SerializeField] private Ease rotateCurve;
    
    [SerializeField] private Transform pivot;
    [SerializeField] private ParticleSystem psDestroy;

    public Action onMoveFinished;

    private Collider _col;
    private float _speedModifier = 1f;
    private Vector3 _direction;

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
            Destroy(gameObject);
        }
    }

    public IEnumerator CubeMove()
    {
        var pos = transform.position;
        var newPos = pos + _direction * GameManager.Instance.tileSize;
        
        pivot.transform.rotation = Quaternion.identity;
        var rot = pivot.transform.rotation * GetAngleFromDirectionMoving();

        transform.DOMove(newPos, moveDuration / _speedModifier).SetEase(moveCurve).OnComplete(()=> onMoveFinished.Invoke());
        pivot.transform.DORotateQuaternion(rot, moveDuration / _speedModifier).SetEase(rotateCurve);

        yield return new WaitForSeconds((moveDuration / _speedModifier) * 1.5f);

        StartCoroutine(CubeMove());
    }

    public float GetMoveDelay() => (moveDuration / _speedModifier);

    public void SetNewDirection(Vector3 newDir)
    {
        _direction = newDir;
    }

    public void SetNewSpeedModifier(float m)
    {
        _speedModifier = m;
    }

    Quaternion GetAngleFromDirectionMoving()
    {
        var treshold = 0.5f;
        var angle = new Vector3(0,0,0);
        
        if (_direction.x > treshold)
        {
            angle = new Vector3(0, 0, -90);
        }
        else if (_direction.x < -treshold)
        {
            angle = new Vector3(0, 0, 90);
        }
        else if (_direction.z > treshold)
        {
            angle = new Vector3(90, 0, 0);
        }
        else if (_direction.z < -treshold)
        {
            angle = new Vector3(-90, 0, 0);
        }

        return Quaternion.Euler(angle);
    }
}