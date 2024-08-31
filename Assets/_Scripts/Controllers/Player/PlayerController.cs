using System;
using System.Threading.Tasks;
using Cities;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerScriptableObject _settings;

        private bool _isMoving;

        private void Awake()
        {
            Resources.LoadAll<PlayerScriptableObject>("Player");
        }

        public async void SetRoute(Transform target)
        {
            SetRoute(target.position);

            await Task.Delay(1500);
            
            if(_isMoving)
                SetRoute(target);
        }
        
        public void SetRoute(Vector3 targetPosition)
        {
            var path = CitiesManager.Instance.GetRoute(transform.position, targetPosition);
            SetPath(path);
        }

        private void OnPathReached()
        {
            Debug.Log("Reached path");

            _isMoving = false;
        }
        
        private void SetPath(Vector2[] path)
        {
            float lenght = 0;
            for (var i = 1; i < path.Length; i++)
                lenght += Vector2.Distance(path[i], path[i - 1]);

            var duration = lenght / _settings.speed;

            transform.DOKill();
            transform.DOPath(Vector2ToVector3(path), duration).SetEase(Ease.Linear).OnComplete(OnPathReached);
        }
        
        
        #region Utilities

        private static Vector3[] Vector2ToVector3(Vector2[] array)
        {
            var result = new Vector3[array.Length];
            for(var i = 0; i < array.Length; i++)
                result[i] = array[i];

            return result;
        }

        #endregion
    }
}
