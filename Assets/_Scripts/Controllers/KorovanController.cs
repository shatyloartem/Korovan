using DG.Tweening;
using UnityEngine;

public class KorovanController : MonoBehaviour
{
    private const float Speed = 1;

    public void Initialize(Vector2[] path)
    {
        transform.position = path[0];

        float length = 0;
        for (var i = 1; i < path.Length; i++)
            length += Vector2.Distance(path[i], path[i - 1]);

        var duration = length / Speed;

        transform.DOPath(Vector2ToVector3(path), duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    private static Vector3[] Vector2ToVector3(Vector2[] array)
    {
        var result = new Vector3[array.Length];
        for(var i = 0; i < array.Length; i++)
            result[i] = array[i];

        return result;
    }
}
