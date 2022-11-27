using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{

    [SerializeField] private Color normalColor;
    [SerializeField] private Color mouseOverColor;

    private Material _material;
    void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
        _material.color = normalColor;
    }

    private void OnMouseEnter()
    {
        _material.color = mouseOverColor;
    }

    private void OnMouseExit()
    {
        _material.color = normalColor;
    }

    private void OnDestroy()
    {
        Destroy(_material);
    }
}
