using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Waker.Injection;

namespace Ncroquis.ProjectHell
{
    public class InjectTester : MonoBehaviour
    {
        [Inject(injectFrom: InjectFrom.Self)]
        [ContextMenuItem("Inject", "Inject")]
        [SerializeField] private GameObject self;

        [Inject]
        [SerializeField] private GameObject children;

        [Inject(injectFrom: InjectFrom.Self)]
        [SerializeField] private GameObject[] selfArray;

        [Inject]
        [SerializeField] private GameObject[] childrenArray;

        [Inject(injectFrom: InjectFrom.Self)]
        [SerializeField] private List<GameObject> selfList;

        [Inject]
        [SerializeField] private List<GameObject> childrenList;

        [Inject("#Test2")]
        [SerializeField] private GameObject childrenTest2;

        [Inject("#Test6")]
        [SerializeField] private GameObject childrenTest6;

        [Inject(".Tests-1")]
        [SerializeField] private List<GameObject> childrenTests1;

        [Inject(".Tests-2")]
        [SerializeField] private List<GameObject> childrenTests2;

        [Inject(injectFrom: InjectFrom.Self)]
        [SerializeField] private SpriteRenderer selfRenderer;

        [Inject(injectFrom: InjectFrom.Self)]
        [SerializeField] private List<Collider> selfColliderList;

        [Inject(injectFrom: InjectFrom.Self)]
        [SerializeField] private Collider[] selfColliderArray;

        [Inject]
        [SerializeField] private SpriteRenderer[] allRenderers;

        [Inject("#Squere1")]
        [SerializeField] private SpriteRenderer rendererSquere1;

        [Inject("#Squere3")]
        [SerializeField] private SpriteRenderer rendererSquere3;

        [Inject(".Squeres")]
        [SerializeField] private SpriteRenderer[] rendererSqueres;

        [Inject(".Squeres-2")]
        [SerializeField] private SpriteRenderer[] rendererSqueres2;

        [Inject("#Squere2 .Squeres")]
        [SerializeField] private SpriteRenderer rendererSquere;

        private void Inject()
        {
            Injector.Inject(this);
        }
    }
}
