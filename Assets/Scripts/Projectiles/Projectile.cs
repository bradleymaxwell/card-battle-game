using System;
using System.Collections;
using UnityEngine;
using Action = System.Action;

namespace Projectiles
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class Projectile : MonoBehaviour, IPoolable
    {
        [SerializeField] private float maxDistance = 30f;
        private float distanceTravelled;
        private Action<UnitPrefab> _onUnitHit;
        
        public void Fire(Vector3 startPosition, MapSpacePrefab targetSpace, float speed, Action<UnitPrefab> onUnitHit, Action onStopped, Func<bool> stopCondition = null)
        {
            StartCoroutine(FireCor(startPosition, targetSpace, speed, stopCondition, onUnitHit, onStopped));
        }
        
        private IEnumerator FireCor(Vector3 startPosition, MapSpacePrefab targetSpace, float speed, Func<bool> stopCondition, Action<UnitPrefab> onUnitHit, Action onStopped)
        {
            _onUnitHit = onUnitHit;
            transform.position = startPosition;
            transform.Face(targetSpace);
            gameObject.SetActive(true);
            var targetPosition = new Vector3(targetSpace.transform.position.x, transform.position.y, targetSpace.transform.position.z);
            while (((stopCondition != null && !stopCondition()) || Vector3.Distance(transform.position, targetPosition) > 0.1f) && distanceTravelled < maxDistance)
            {
                var frameTravelDistance = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, frameTravelDistance);
                distanceTravelled += frameTravelDistance;
                yield return null;
            }
            
            onStopped?.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out UnitPrefab unitPrefab))
            {
                _onUnitHit?.Invoke(unitPrefab);
            }
        }
        
        public void Reset()
        {
            _onUnitHit = null;
            distanceTravelled = 0;
        }
        
        public GameObject Prefab { get; set; }
    }
}