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
    public LayerMask surfaceMask;
    
    [SerializeField] private Ease moveCurve;
    [SerializeField] private Ease rotateCurve;
    
    [SerializeField] private Transform pivot;
    [SerializeField] private ParticleSystem psDestroy;
    [SerializeField] private ParticleSystem psWalk;
    [SerializeField] private ParticleSystem psAppear;

    public bool prePlaced;

    private Collider _col;

    private void Awake()
    {
        _col = GetComponent<Collider>();
    }

    private void Start()
    {
        CollisionManaging(false);
        Spawn();
    }

    void Spawn()
    {
        if (!prePlaced)
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
        else
        {
            CollisionManaging(true);
            StartCoroutine(CubeMove());
        }
    }

    public void CollisionManaging(bool enable)
    {
        _col.enabled = enable;
    }

    public void SetHided(bool hided)
    {
        if (!hided) psAppear.Play();
        GetComponentInChildren<MeshRenderer>().enabled = !hided;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle")
            || other.gameObject.layer == LayerMask.NameToLayer("Cube"))
        {
            DestroyCube();
        }
    }

    private void DestroyCube()
    {
        Instantiate(psDestroy, transform.position, Quaternion.identity);
            
        GameManager.Instance.EndGame();
            
        CameraManager.Instance.CenterCameraOnTarget(transform, 2f);
        CameraManager.Instance.CameraShake(0.5f, 0.15f, 100);
            
        Destroy(gameObject);
    }

    public IEnumerator CubeMove()
    {
        var pos = transform.position;
        var newPos = pos + direction;
        
        pivot.transform.rotation = Quaternion.identity;
        var rot = pivot.transform.rotation * GetAngleFromDirectionMoving();

        transform.DOMove(newPos, moveDuration / speedModifier).SetEase(moveCurve);
        pivot.transform.DORotateQuaternion(rot, moveDuration / speedModifier).SetEase(rotateCurve).OnComplete(() =>
        {
            psWalk.Stop();
            psWalk.Play();
            
            AudioManager.Instance?.PlaySound("CubeLand");
        });

        yield return new WaitForSeconds((moveDuration / speedModifier) * 1.5f);

        GetSurface();
        
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

    private RaycastHit _hitSurface;
    private void GetSurface()
    {
        Physics.Raycast(transform.position, Vector3.down, out _hitSurface, 20f, surfaceMask);
        if(!_hitSurface.collider) DestroyCube();
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
}