using System;
using Core;
using Managers;
using UnityEngine;
using Utils;

namespace MonoObjects
{
    public class Cell : MonoBehaviour
    {
        public Vector2Int location;
        public Vector3 localPosition;
        public Vector3 WorldPosition => transform.position;
        public Vector3 Down => -transform.up;
        public Item holdingItem;
        public Cell containerCell;
        public Cellular cellular;
        [SerializeField] private string colorName;
        
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Color highlightPlaceColor = Color.green;
        [SerializeField] private Color highlightCantPlaceColor = Color.red;
        
        public Color color;

        private void OnValidate()
        {
            SetMaterial(color);
        }

        private void Start()
        {
            SetMaterial(color);
            HideCell();
        }

        public void HideCell()
        {
            if(meshRenderer == null) return;
            
            meshRenderer.enabled = false;
        }

        public void ShowCell()
        {
            if(meshRenderer == null) return;
            
            meshRenderer.enabled = true;
        }

        public bool IsGridOccupied()
        {
            return holdingItem != null;
        }

        public Cell Init(Vector2Int initialLocation, Vector3 initialPosition, float cellSize, Cellular cellular)
        {
            this.location = initialLocation;
            this.localPosition = initialPosition;
            this.cellular = cellular;
            transform.localPosition = initialPosition;
            transform.localScale = (transform.localScale * cellSize).WithY(1f);
            return this;
        }

        public void SetColor(Color _color)
        {
            color = _color;
            SetMaterial(color);
        }
        
        private void SetMaterial(Color _color)
        {
            var _materialPropertyBlock = new MaterialPropertyBlock();
            _materialPropertyBlock.SetColor(colorName, _color);
            meshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }

        public void DoHighlight(bool canPlace)
        {
            SetMaterial(canPlace ? highlightPlaceColor : highlightCantPlaceColor);
        }

        public void RemoveHighlight()
        {
            SetMaterial(this.color);
        }

        private void OnDestroy()
        {
            
        }
    }
}