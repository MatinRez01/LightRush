using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridsManager : MonoBehaviour
{
    [SerializeField] UvScroll[] grids;
    bool shouldIncreaseSpeed = false;
    public Vector2 resetVector;

    Animation animationCom;
    
    public void ScrollGrids(Vector2 scrollVec)
    {
        foreach (var grid in grids)
        {
            grid.Material.mainTextureOffset += scrollVec * grid.scrollSpeed * Time.deltaTime;
            if(Mathf.Abs(grid.Material.mainTextureOffset.x) >= resetVector.x)
            {
                grid.Material.mainTextureOffset = new Vector2(0, grid.Material.mainTextureOffset.y);
            }
            if (Mathf.Abs(grid.Material.mainTextureOffset.y) >= resetVector.y)
            {
                grid.Material.mainTextureOffset = new Vector2(grid.Material.mainTextureOffset.x, 0);
            }
        }
    }
    private void Update()
    {
        if (shouldIncreaseSpeed)
        {
            foreach (var grid in grids)
            {
                grid.scrollSpeed += Time.deltaTime / 1000;
                grid.scrollSpeed = Mathf.Clamp(grid.scrollSpeed, 0, grid.maxScrollSpeed);
            }
        }
    }
    public void StartIncreaseSpeed()
    {
        shouldIncreaseSpeed = true;
        animationCom = GetComponent<Animation>();
        animationCom.Play();
    }
    public void StopScrolling()
    {
        foreach (var grid in grids)
        {
            grid.scrollSpeed = 0;
        }
        shouldIncreaseSpeed = false;
    }
    public void IncreaseSpeedOfGrids(float speedAdd)
    {
        foreach(var grid in grids)
        {
            float s = grid.scrollSpeed + speedAdd;
            DOTween.To(() => grid.scrollSpeed, x => grid.scrollSpeed = x, s, 5);
        }
    }
}
