using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
 [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = CreateNewObject();
            pool.Enqueue(obj);
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        return obj;
    }

    public GameObject Get()
    {
        // เขียนให้สั้นและเคลียร์ขึ้น ถ้า Queue ว่างให้สร้างใหม่ ถ้าไม่ว่างให้ดึงออกมา
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : CreateNewObject();
        
        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        // ป้องกันบั๊ก Double Return 
        if (obj.activeSelf)
        {
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}