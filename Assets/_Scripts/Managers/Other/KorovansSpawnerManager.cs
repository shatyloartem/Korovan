using Cities;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Korovans
{
    public class KorovansSpawnerManager : MonoBehaviour
    {
        [SerializeField] private GameObject korovanPrefab;

        [Space(6)]

        [SerializeField] private Vector2 spawnKorovansTimes = new Vector2(3, 10);

        private void Awake()
        {
            SpawnKorovans();
        }

        private async void SpawnKorovans()
        {
            Debug.Log("Spawning Korovans");

            await Task.Delay((int)(Random.Range(spawnKorovansTimes.x, spawnKorovansTimes.y) * 1000));

            var _citiesManager = CitiesManager.Instance;
            var cities = _citiesManager.GetTwoRandomCities();
            var path = _citiesManager.GetRoute(cities[0], cities[1]);

            Instantiate(korovanPrefab).GetComponent<KorovanController>().Initialize(path);

            SpawnKorovans();
        }
    }
}
