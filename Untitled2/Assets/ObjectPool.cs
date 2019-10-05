using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : ScriptableObject {
    GameObject refGameObject;
    List<GameObject> pool;
    int poolSize, nextUnused = 0;
    string poolName;

    public void Init(string poolName, GameObject refGameObject, int poolSize)
    {
        pool = new List<GameObject>(poolSize);
        this.refGameObject = refGameObject;
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
        System.Func<GameObject> getUnused = () => 
        {
            int index = nextUnused;
            pool[index].transform.position = position;
            pool[index].transform.rotation = rotation;
            pool[index].SetActive(true);
            return pool[index];
        };
   
        int start = nextUnused;
        for(; nextUnused < poolSize; nextUnused++)
        {
            if(!pool[nextUnused].activeInHierarchy)
            {
                return getUnused();
            }
        }
        nextUnused = 0;
        for(; nextUnused < start; nextUnused++)
        {
            if (!pool[nextUnused].activeInHierarchy)
            {
                return getUnused();
            }
        }

        Debug.Log(name + " object pool is empty, considering upping the pool size");
        return null;
    }

}
