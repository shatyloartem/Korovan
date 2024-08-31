using System;
using System.Threading.Tasks;
using DG.Tweening;
using Managers.Cities;
using SA.EasyConsole;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerMovement : MonoBehaviour
    {
        private PlayerController _playerController;
        private bool _isMoving;

        private void Awake()
        {
            EasyConsole.RegisterCommand(new CommandObject(nameof(SetRouteCommand), this));
        }

        public async void SetRoute(Transform target)
        {
            SetRoute(target.position);

            await Task.Delay(1500);
            
            if(_isMoving)
                SetRoute(target);
        }

        [Command]
        public void SetRouteCommand(float x, float y) => SetRoute(new Vector3(x, y, 0));
        
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
            float length = 0;
            for (var i = 1; i < path.Length; i++)
                length += Vector2.Distance(path[i], path[i - 1]);

            var duration = length / _playerController.GetSettings().speed;

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