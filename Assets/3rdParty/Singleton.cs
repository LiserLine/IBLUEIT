/*
 * Unity Singleton MonoBehaviour
 * This file is licensed under the terms of the MIT license. 
 * 
 * Copyright (c) 2014 Kleber Lopes da Silva (https://github.com/kleber-swf/Unity-Singleton-MonoBehaviour)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public bool Persistent;

    public static T Instance
    {
        get
        {
            if (_instance == null)
                CreateInstance();

            return _instance;
        }
    }

    private static void CreateInstance()
    {
        var type = typeof(T);
        var objects = FindObjectsOfType<T>();

        if (objects.Length < 1)
            return;

        if (objects.Length > 1)
        {
            Debug.LogWarning($"There is more than one instance of Singleton of type \"{type}\"." +
                             "Keeping the first one. Destroying the others.");

            for (var i = 1; i < objects.Length; i++)
                Destroy(objects[i].gameObject);
        }

        _instance = objects[0];
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            if (!Persistent)
                return;

            CreateInstance();
            DontDestroyOnLoad(gameObject);
            return;
        }

        if (Persistent)
            DontDestroyOnLoad(gameObject);

        if (GetInstanceID() != _instance.GetInstanceID())
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!Persistent)
            _instance = null;
    }
}