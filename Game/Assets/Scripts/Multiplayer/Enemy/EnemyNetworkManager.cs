using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class EnemyNetworkManager : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private NetworkObject enemyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private List<Transform> spawnPoints; // Danh sách các vị trí có thể spawn
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private float respawnDelay = 3f;

    // Danh sách để theo dõi các Enemy đang tồn tại
    [Networked] private int currentEnemyCount { get; set; }

    // Timer để quản lý việc quét và spawn lại
    [Networked] private TickTimer spawnLoopTimer { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            // Bắt đầu spawn đợt đầu tiên ngay khi manager xuất hiện
            InitialSpawn();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        // Cứ mỗi khoảng thời gian, kiểm tra xem có cần spawn thêm quái không
        if (spawnLoopTimer.ExpiredOrNotRunning(Runner))
        {
            if (currentEnemyCount < maxEnemies)
            {
                SpawnRandomEnemy();
            }
            // Reset timer để kiểm tra lần tiếp theo
            spawnLoopTimer = TickTimer.CreateFromSeconds(Runner, respawnDelay);
        }
    }

    private void InitialSpawn()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnRandomEnemy();
        }
    }

    public void SpawnRandomEnemy()
    {
        if (spawnPoints.Count == 0 || enemyPrefab == null) return;

        // Chọn một vị trí ngẫu nhiên từ danh sách
        int randomIndex = Random.Range(0, spawnPoints.Count);
        Vector3 spawnPos = spawnPoints[randomIndex].position;
        Quaternion spawnRot = spawnPoints[randomIndex].rotation;

        Runner.Spawn(enemyPrefab, spawnPos, spawnRot, null, (runner, obj) => {
            // Đăng ký sự kiện khi quái chết (nếu cần) hoặc setup thông số
            currentEnemyCount++;

            // Link ngược lại Manager để khi chết Enemy có thể thông báo
            if (obj.TryGetComponent<EnemyNetworkBase>(out var ai))
            {
                ai.SetupEnemy(this);
            }
        });
    }

    // Hàm này sẽ được Enemy gọi trước khi Despawn
    public void DecerementEnemyCount()
    {
        if (Object.HasStateAuthority)
        {
            currentEnemyCount--;
        }
    }
}