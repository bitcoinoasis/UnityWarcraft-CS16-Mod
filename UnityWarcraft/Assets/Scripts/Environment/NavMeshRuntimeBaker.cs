using Unity.AI.Navigation;
using UnityEngine;

namespace Warcraft.Environment
{
    [RequireComponent(typeof(NavMeshSurface))]
    public class NavMeshRuntimeBaker : MonoBehaviour
    {
        [SerializeField] private bool buildOnStart = true;

        private void Start()
        {
            if (!buildOnStart)
            {
                return;
            }

            var surface = GetComponent<NavMeshSurface>();
            surface.BuildNavMeshAsync();
        }
    }
}
