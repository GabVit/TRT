using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CivilSpawner : MonoBehaviour {
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private CivilAI civilPrefab;
    [SerializeField] private float spawnInterval;
    [SerializeField] private int maxCivilNumber;
    [SerializeField] private Player player;

    private readonly List<CivilAI> spawnedCivil = new();
    private float timeSinceLastSpawn;

    private void Start() {
        timeSinceLastSpawn = spawnInterval;
    }

    private void Update() {
        timeSinceLastSpawn += Time.deltaTime;
        if (!(timeSinceLastSpawn > spawnInterval)) return;
        timeSinceLastSpawn = 0f;
        if (spawnedCivil.Count >= maxCivilNumber) return;
        SpawnCivil();
    }

    private void SpawnCivil() {
        var civil = Instantiate(civilPrefab, transform.position, transform.rotation);
        var spawnPointIndex = spawnedCivil.Count % spawnPoints.Length;
        civil.Init(player, spawnPoints[spawnPointIndex]);
        spawnedCivil.Add(civil);
    }
}