using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerScriptableObject _settings;

        private bool _isMoving;

        private void Awake()
        {
            Resources.LoadAll<PlayerScriptableObject>("Player");
        }

        public PlayerScriptableObject GetSettings() => _settings;
    }
}
