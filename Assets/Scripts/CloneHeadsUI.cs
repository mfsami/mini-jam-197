using NUnit.Framework;
using UnityEngine;

public class CloneHeadsUI : MonoBehaviour
{
    [SerializeField] private GameObject[] heads;
    public void SetCount(int count)
    {
        if (heads == null) return;
        int n = Mathf.Clamp(count, 0, heads.Length);
        for (int i = 0; i < heads.Length; i++)
            heads[i].SetActive(i < n);
    }
}
