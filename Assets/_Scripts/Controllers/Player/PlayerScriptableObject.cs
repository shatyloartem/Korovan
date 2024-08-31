using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "SO/Player", fileName = "Player Settings")]
    public class PlayerScriptableObject : ScriptableObject
    {
        public float speed = 2;
    }
}