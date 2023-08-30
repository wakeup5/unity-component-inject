using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Waker.Injection;

public class InjectText : MonoBehaviour
{
    //[Inject(InjectFrom.Self)]
    //public GameObject gameObject;

    //[Inject(InjectFrom.Self)]
    //public Transform transform;

    //[Inject(InjectFrom.Self)]
    //public GameObject[] gameObjects;

    //[Inject(InjectFrom.Self)]
    //public Transform[] transforms;

    //[Inject(InjectFrom.Children)]
    //public GameObject gameObject2;

    //[Inject(InjectFrom.Children)]
    //public Transform transform2;

    //[Inject(InjectFrom.Children)]
    //public GameObject[] gameObjects2;

    //[Inject(InjectFrom.Children)]
    //public Transform[] transforms2;

    //[Inject("#a")]
    //public GameObject gameObject3;

    //[Inject("#a")]
    //public Transform transform3;

    //[Inject("#a")]
    //public GameObject[] gameObjects3;

    //[Inject("#a")]
    //public Transform[] transforms3;

    [Inject("?ello World!")]
    public GameObject gameObject4;

    private void OnValidate()
    {
        Injector.Inject(this);
    }
}
