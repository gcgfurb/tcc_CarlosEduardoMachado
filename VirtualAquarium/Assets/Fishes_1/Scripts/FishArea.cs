﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class FishArea : MonoBehaviour
{
    public GameObject[] prefabs;
    public int count;
    GameController gameController;
    public float speed = 10;
    public float rotationSpeed = 5f;
    public float raycastDistance = 10f;
    public FoodPoint feedPoint;
    public ParticleSystem particleFood;
    public Bounds bounds;

    public int Count
    {
        get
        {
            return count;
        }
        set
        {
            if (count != value)
            {
                count = value;
                SpawnFishes();
                transform.hasChanged = false;
            }
        }
    }

    public List<Fish> fishes;
    List<Fish> fishesOrderByFood;
    Fish.FishComparer fishComparer;
    Vector3 center;
    Vector3 max;
    Vector3 min;
    public EggFish eggFish;

    void Start()
    {
        if (!gameController)
        {
            gameController = GameObject.FindObjectOfType<GameController>();
        }
        fishComparer = new Fish.FishComparer();
        fishes = new List<Fish>(GameObject.FindObjectsOfType<Fish>());
        fishesOrderByFood = new List<Fish>(fishes);

        SpawnFishes();
        InitializeAllFishes();

    }

    public void Update()
    {
        int antes = fishes.Count;
        fishes = new List<Fish>(GameObject.FindObjectsOfType<Fish>());
        if (antes != fishes.Count)
        {
            InitializeAllFishes();
        }

        if (Input.GetKeyDown(KeyCode.Space) && AquariumProperties.foodAvailable > 0)
        {
            AquariumProperties.foodAvailable--;
            particleFood.Play();
        }

        UpdateFishes();
    }

    public void InitializeAllFishes()
    {
        max = center + bounds.size / 2f;
        min = center - bounds.size / 2f;
        for (int i = 0; i < fishes.Count; i++)
        {
            if (!fishes[i].iniciado)
            {
                fishes[i].Initialize(this);
            }
        }
    }

    public void SpawnFishes()
    {
        fishes = new List<Fish>(transform.GetComponentsInChildren<Fish>());
        center = bounds.center;
        max = center + bounds.size / 2f;
        min = center - bounds.size / 2f;
        if (fishes.Count < count)
            for (int i = fishes.Count; i < count; i++)
                SpawnFish();
        else
        if (fishes.Count > count)
            for (int i = 0; i < fishes.Count - count; i++)
#if UNITY_EDITOR
                DestroyImmediate(fishes[i].gameObject);
#else
        Destroy (fishes[i].gameObject);
#endif
        fishes = new List<Fish>(transform.GetComponentsInChildren<Fish>());
        MixPositions();
    }

    public void removeFish(Fish fish)
    {
        this.fishes.Remove(fish);
        this.fishesOrderByFood.Remove(fish);
#if UNITY_EDITOR
        DestroyImmediate(fish.gameObject);
#else
        Destroy (fish.gameObject);
#endif
    }

    public void RemoveFishes()
    {
        fishes = new List<Fish>(transform.GetComponentsInChildren<Fish>());
        for (int i = 0; i < fishes.Count; i++)
#if UNITY_EDITOR
            DestroyImmediate(fishes[i].gameObject);
#else
        Destroy (fishes[i].gameObject);
#endif
    }

    private void UpdateFishes()
    {
        for (int i = 0; i < fishes.Count; i++)
        {
            if (fishes[i].gameObject.activeSelf)
            {
                fishes[i].Move();
            }
        }
    }

    public void SpawnFish()
    {
        GameObject fish = Instantiate(GetRandomPrefab());
        fish.transform.parent = transform;



    }

    public void MixPositions()
    {
        for (int i = 0; i < fishes.Count; i++)
        {
            Transform temp = fishes[i].transform;
            temp.position = GetRandomPoint();
            temp.Rotate(0f, UnityEngine.Random.Range(0f, 360f), 0f, Space.Self);
        }

    }

    protected GameObject GetRandomPrefab()
    {
        int id = UnityEngine.Random.Range(0, prefabs.Length);
        return prefabs[id];
    }

    public Vector3 GetRandomPoint()
    {

        float x = UnityEngine.Random.Range(min.x, max.x);
        float y = UnityEngine.Random.Range(min.y, max.y);
        float z = UnityEngine.Random.Range(min.z, max.z);
        Vector3 p = new Vector3(x, y, z);
        return p;
    }

    public void removeFood()
    {
        feedPoint.removeFood();
        Destroy(feedPoint.foods[feedPoint.foods.Count - 1]);
        feedPoint.foods.RemoveAt(feedPoint.foods.Count - 1);
    }

    public void removeFood(GameObject obj)
    {
        if (feedPoint.foods.Contains(obj))
        {
            feedPoint.removeFood();
            feedPoint.foods.Remove(obj);
        }
    }

    private bool hasFishesFeeding()
    {
        for (int i = 0; i < fishesOrderByFood.Count; i++)
        {
            if (fishesOrderByFood[i].state == Fish.FStates.Feed)
            {

                return true;
            }
        }
        return false;
    }

    public void removeEggs()
    {
        foreach (EggFish egg in GetComponentsInChildren<EggFish>())
        {
            Destroy(egg.gameObject);
        }
    }

    public void mixStaticFishPositions()
    {
        foreach (StaticFish item in GetComponentsInChildren<StaticFish>())
        {
            Transform temp = item.transform;
            temp.position = GetRandomPoint();
            temp.Rotate(0f, UnityEngine.Random.Range(0f, 360f), 0f, Space.Self);
        }
    }
}