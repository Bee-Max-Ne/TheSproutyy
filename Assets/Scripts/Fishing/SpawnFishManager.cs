// ──────────────────────────────────────────────
// TheSprouty | Fishing/SpawnFishManager.cs
// Singleton. Finds all FishSpawnZones in the scene,
// spawns FishShadow prefabs up to maxFishCount per zone,
// and respawns after a delay when a shadow is destroyed.
// ──────────────────────────────────────────────
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFishManager : MonoBehaviour
{
    // ----------------------------------------------------------
    // Singleton
    // ----------------------------------------------------------
    public static SpawnFishManager Instance { get; private set; }

    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [Header("Settings")]
    [Tooltip("Seconds before a new shadow spawns after one is destroyed.")]
    [SerializeField] private float respawnDelay = 10f;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private FishSpawnZone[] _zones;
    private readonly Dictionary<FishSpawnZone, List<FishShadow>> _activeShadows = new();

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        _zones = FindObjectsByType<FishSpawnZone>(FindObjectsSortMode.None);

        foreach (FishSpawnZone zone in _zones)
        {
            _activeShadows[zone] = new List<FishShadow>();
            FillZone(zone);
        }
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Called when a FishShadow is caught — removes it and schedules respawn.</summary>
    public void OnShadowRemoved(FishShadow shadow, FishSpawnZone zone)
    {
        if (_activeShadows.TryGetValue(zone, out List<FishShadow> list))
            list.Remove(shadow);

        Destroy(shadow.gameObject);
        StartCoroutine(RespawnRoutine(zone));
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void FillZone(FishSpawnZone zone)
    {
        int current = _activeShadows[zone].Count;
        int toSpawn = zone.MaxFishCount - current;

        for (int i = 0; i < toSpawn; i++)
            SpawnOne(zone);
    }

    private void SpawnOne(FishSpawnZone zone)
    {
        GameObject prefab = zone.GetRandomShadowPrefab();
        if (prefab == null) return;

        Vector3 spawnPos = zone.GetRandomSpawnPosition();
        GameObject go    = Instantiate(prefab, spawnPos, Quaternion.identity);
        FishShadow shadow = go.GetComponent<FishShadow>();

        if (shadow == null) return;

        _activeShadows[zone].Add(shadow);
    }

    private IEnumerator RespawnRoutine(FishSpawnZone zone)
    {
        yield return new WaitForSeconds(respawnDelay);
        if (zone != null)
            SpawnOne(zone);
    }
}
