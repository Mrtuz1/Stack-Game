using UnityEngine;

public class BottomBlocks : MonoBehaviour
{
    private Renderer blockRenderer;
    bool isFirstColorSet;
    Color _color;

    private void Awake()
    {
        isFirstColorSet = false;
        blockRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (isFirstColorSet)
        {
            SetColorBottomBlock();
        }
        isFirstColorSet = BlockSpawner.instance.GetIsFirstColorSet();
    }

    private void SetColorBottomBlock()
    {
        isFirstColorSet = false;
        BlockSpawner.instance.SetIsFirstColorSet(isFirstColorSet);
        _color = BlockSpawner.instance.GetCurColor();

        // Mevcut material'daki alpha deðerini koru (gradient için)
        Color currentColor = blockRenderer.material.GetColor("_BaseColor");
        _color.a = currentColor.a;

        // Shader Graph'daki _BaseColor property'sine rengi atama
        blockRenderer.material.SetColor("_BaseColor", _color);
    }
}