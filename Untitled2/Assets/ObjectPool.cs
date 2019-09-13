using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : ScriptableObject {
    GameObject refGameObject;
    List<GameObject> pool;
    int nextUnused = 0;
    int poolSize;
    string poolName;

    public void Init(string poolName, GameObject refGameObject, int poolSize)
    {
        this.refGameObject = refGameObject;
        pool = new List<GameObject>(poolSize);
        this.poolName = poolName;
        this.poolSize = poolSize;
        for(int i = 0; i < poolSize; i++)
        {
            pool.Add(Instantiate(refGameObject));
            pool[i].SetActive(false);
        }
    }

    public void Return(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    public GameObject Fetch(Vector3 position, Quaternion rotation)
    {
        int start = nextUnused;
        for(; nextUnused < poolSize; nextUnused++)
        {
            if(!pool[nextUnused].activeInHierarchy)
            {
                nextUnused++;
                pool[nextUnused].transform.position = position;
                pool[nextUnused].transform.rotation = rotation;
                pool[nextUnused].SetActive(true);
                return pool[nextUnused];
            }
        }
        nextUnused = 0;
        for(; nextUnused < start; nextUnused++)
        {
            if (!pool[nextUnused].activeInHierarchy)
            {
                nextUnused++;
                pool[nextUnused].transform.position = position;
                pool[nextUnused].transform.rotation = rotation;
                pool[nextUnused].SetActive(true);
                return pool[nextUnused];
            }
        }

        Debug.Log(name + " object pool is empty, considering upping the pool size");
        return null;
    }

}
