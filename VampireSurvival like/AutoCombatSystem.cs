using UnityEngine;

public class AutoCombatSystem : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public LayerMask enemyLayer;
    [Header("Combat Stats")]
     public float attacksPerSecond = 1.5f; 
    public float baseDamage = 10f;
    [Range(0f, 1f)] public float critChance = 0.2f;
    public float critMultiplier = 2.0f;

    [Header("References")]
    public Transform firePoint;
    public string projectilePoolTag = "PlayerBullet"; 
    private float attackTimer;

    void Update()
    {
        HandleCombatLoop();
    }

    private void HandleCombatLoop()
    {
        attackTimer += Time.deltaTime;

        // คำนวณ Cooldown จากค่า APS 
        float cooldown = 1f / attacksPerSecond;

        if (attackTimer >= cooldown)
        {
            Transform nearestTarget = FindNearestTarget();
            
            if (nearestTarget != null)
            {
                ExecuteAttack(nearestTarget);
                attackTimer = 0f; // Reset timer หลังโจมตีเสร็จ
            }
        }
    }

    // ระบบ Nearest-Target Selection + Physics2D Overlap
    private Transform FindNearestTarget()
    {
        // ใช้ OverlapCircleAll เพื่อดึงศัตรูทั้งหมดในรัศมี  
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);

        if (colliders.Length == 0) return null;

        Transform nearest = null;
        float minDistance = float.MaxValue;

        foreach (Collider2D col in colliders)
        {
            //   ใช้ sqrMagnitude แทน Vector2.Distance เพื่อเลี่ยงการถอดรูท  
            float distanceSqr = (transform.position - col.transform.position).sqrMagnitude;
            
            if (distanceSqr < minDistance)
            {
                minDistance = distanceSqr;
                nearest = col.transform;
            }
        }

        return nearest;
    }

    // ระบบจัดการ Projectile และ Critical Calculation
    private void ExecuteAttack(Transform target)
    {
        // 1. คำนวณ Critical Damage Algorithm
        float finalDamage = baseDamage;
        bool isCrit = Random.value <= critChance;
        if (isCrit)
        {
            finalDamage *= critMultiplier;
        }

        // 2. ดึงกระสุนจาก Object Pool 
     
        GameObject projectileObj = ObjectPoolManager.Instance.SpawnFromPool(projectilePoolTag, firePoint.position, Quaternion.identity);

        if (projectileObj != null)
        {
            // คำนวณทิศทางยิงและส่งค่า Damage ไปให้กระสุน
            Vector2 direction = (target.position - firePoint.position).normalized;
            
            // ใช้ TryGetComponent เพื่อความปลอดภัยและ Performance
            if (projectileObj.TryGetComponent<Projectile>(out Projectile projectileInfo))
            {
                projectileInfo.Setup(direction, finalDamage, isCrit);
            }
        }
    }

    // วาดเส้นรัศมีใน Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}