using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    // ΩÃ±€≈Ê ∆–≈œ
    static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
                instance = GameObject.FindObjectOfType<T>();

            return instance;
        }
    }

    protected void Awake()
    {
        instance = this as T;
    }
}
