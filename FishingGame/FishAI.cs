using UnityEngine;

public interface IFishState
{
    void EnterState(FishAI fish);
    void UpdateState(FishAI fish);
    void ExitState(FishAI fish);
}

public class FishAI : MonoBehaviour
{
    [Header("Fish Settings")]
    public float swimSpeed = 2f;
    public float chaseSpeed = 5f;
    public float detectionRadius = 4f;
    
    // ตัวแปรเก็บตำแหน่งเหยื่อ
    public Transform currentBait { get; set; }

    private IFishState currentState;
    
    // สร้าง Instance ของ States
    public FishWanderState wanderState = new FishWanderState();
    public FishChaseBaitState chaseState = new FishChaseBaitState();

    private void Start()
    {
        // เริ่มต้นด้วยสถานะลอยเล่น
        ChangeState(wanderState);
    }

    private void Update()
    {
        currentState?.UpdateState(this);
    }

    public void ChangeState(IFishState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState?.EnterState(this);
    }

    // ฟังก์ชันให้ State อื่นๆ เรียกใช้เพื่อเช็คหาเหยื่อ
    public bool DetectBait()
    {
        Collider2D baitCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Bait"));
        if (baitCollider != null)
        {
            currentBait = baitCollider.transform;
            return true;
        }
        return false;
    }
}

//State: ลอยเล่น (Wander)
public class FishWanderState : IFishState
{
    private Vector2 targetPosition;
    private float timer;

    public void EnterState(FishAI fish)
    {
        GetNewRandomPosition(fish);
    }

    public void UpdateState(FishAI fish)
    {
        // ว่ายไปตำแหน่งสุ่ม
        fish.transform.position = Vector2.MoveTowards(fish.transform.position, targetPosition, fish.swimSpeed * Time.deltaTime);

        // หาจุดว่ายน้ำใหม่เมื่อถึงเป้าหมายหรือหมดเวลา
        timer -= Time.deltaTime;
        if (Vector2.Distance(fish.transform.position, targetPosition) < 0.1f || timer <= 0)
        {
            GetNewRandomPosition(fish);
        }

        // คอยเช็คว่ามีเบ็ดตกมาใกล้ๆ ไหม ถ้ามีให้เปลี่ยน State
        if (fish.DetectBait())
        {
            fish.ChangeState(fish.chaseState);
        }
    }

    public void ExitState(FishAI fish) { }

    private void GetNewRandomPosition(FishAI fish)
    {
        targetPosition = (Vector2)fish.transform.position + Random.insideUnitCircle * 3f;
        timer = 3f;
    }
}

//State: วิ่งไปหาเบ็ด (Chase Bait)
public class FishChaseBaitState : IFishState
{
    public void EnterState(FishAI fish)
    {
        Debug.Log("Fish saw the bait! Chasing...");
    }

    public void UpdateState(FishAI fish)
    {
        if (fish.currentBait == null)
        {
            // ถ้าเหยื่อหายไป (เช่นคนดึงหลบ) กลับไปลอยเล่นต่อ
            fish.ChangeState(fish.wanderState);
            return;
        }

        // ว่ายพุ่งไปหาเหยื่อ
        fish.transform.position = Vector2.MoveTowards(fish.transform.position, fish.currentBait.position, fish.chaseSpeed * Time.deltaTime);

        // ถ้าถึงเหยื่อแล้ว ทริกเกอร์ Event ตกปลา (เชื่อมกับ Minigame)
        if (Vector2.Distance(fish.transform.position, fish.currentBait.position) < 0.2f)
        {
            FishingEventManager.Instance.TriggerFishBite();
            
        }
    }

    public void ExitState(FishAI fish) { }
}