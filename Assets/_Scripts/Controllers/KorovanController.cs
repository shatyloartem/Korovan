using DG.Tweening;
using UnityEngine;

public class KorovanController : MonoBehaviour
{
    private float speed = 1;

    public void Initialize(Vector2[] path)
    {
        transform.position = path[0];

        float lenght = 0;
        for (int i = 1; i < path.Length; i++)
            lenght += Vector2.Distance(path[i], path[i - 1]);

        float duration = lenght / speed;

        transform.DOPath(Vector2ToVector3(path), duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    public Vector3[] Vector2ToVector3(Vector2[] array)
    {
        var result = new Vector3[array.Length];
        for(int i = 0; i < array.Length; i++)
            result[i] = array[i];

        return result;
    }
}
