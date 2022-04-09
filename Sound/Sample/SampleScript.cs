using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScript : MonoBehaviour
{
    [SerializeField][Header("クリック時に鳴らすSE")] private SE_Enum seEnum;
    public void OnClicked()
    {
        SoundManager.PlaySE(seEnum);
    }
}
