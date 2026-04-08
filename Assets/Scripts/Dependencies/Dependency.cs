using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace QuizCinema
{
    public abstract class Dependency : MonoBehaviour
    {
        protected virtual void BindAll(MonoBehaviour monoBehaviourInScene) { }

        protected void FindAllObjectToBind()
        {
            MonoBehaviour[] monoInScene = FindObjectsOfType<MonoBehaviour>();

            for (int i = 0; i < monoInScene.Length; i++)
            {
                BindAll(monoInScene[i]);
            }
        }


        protected void Bind<T>(MonoBehaviour bindObject, MonoBehaviour monoBehaviourInScene) where T : class
        {
            if (monoBehaviourInScene is IDependency<T>) (monoBehaviourInScene as IDependency<T>).Construct(bindObject as T);

           
        }

    }
}