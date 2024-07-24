using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICurrencyDropOverTimeView
{
    void Show();
    void Hide();
    void UpdateProgressView(float value01);
    void PlayDropAnimation();
}
