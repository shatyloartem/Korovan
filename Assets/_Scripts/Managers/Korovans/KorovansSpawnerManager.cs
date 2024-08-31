using System.Threading.Tasks;
using Cities;
using Managers.Cities;
using UnityEngine;

namespace Managers.Korovans
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
            await Task.Delay((int)(Random.Range(spawnKorovansTimes.x, spawnKorovansTimes.y) * 1000));

            if(!Application.isPlaying)
                return;
            
            var citiesManager = CitiesManager.Instance;
            var cities = citiesManager.GetTwoRandomCities();
            var path = citiesManager.GetRoute(cities[0], cities[1]);

            Instantiate(korovanPrefab).GetComponent<KorovanController>().Initialize(path);

            SpawnKorovans();
        }
    }
}
