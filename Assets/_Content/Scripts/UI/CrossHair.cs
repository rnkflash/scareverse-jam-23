using System;
using TMPro;
using UnityEngine;

namespace _Content.Scripts.UI
{
    public class CrossHair: MonoBehaviour
    {
        [SerializeField] private TMP_Text dot;

        public void TurnRed(bool on)
        {
            if (dot == null) return;
            dot.color = on ? Color.red : Color.white;
        }
    }
}