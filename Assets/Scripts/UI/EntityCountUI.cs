using System;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace TDGame.UI
{
    public class EntityCountUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;

        private EntityManager entityManager;

        private void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void Update()
        {
            var entities = entityManager.GetAllEntities();

            text.text = (entities.Length / 5 - 1) .ToString();

            entities.Dispose();
        }
    }
}