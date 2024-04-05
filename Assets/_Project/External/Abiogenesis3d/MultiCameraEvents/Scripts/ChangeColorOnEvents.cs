using UnityEngine;

namespace Abiogenesis3d
{
    public class ChangeColorOnEvents : MonoBehaviour
    {
        Renderer rend;
        Color originalColor;
        public Color hoveredColor = new Color(20, 50, 100);
        public bool log;

        Color color;
        float alpha = 1;
        float emission = 0.5f;

        void Start()
        {
            rend = GetComponent<Renderer>();
            originalColor = rend.material.color;
            color = originalColor;
        }

        // Color GetInvertedColor(Color color)
        // {
        //     return new Color (1 - color.r, 1 - color.g, 1 - color.b, color.a);
        // }

        Color GetColor()
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        void OnMouseEnter()
        {
            if (log) Debug.Log("OnMouseEnter: " + name);
            color = hoveredColor;
            UpdateColor();
        }

        void OnMouseExit()
        {
            if (log) Debug.Log("OnMouseExit: " + name);
            color = originalColor;
            emission = 0.5f;
            UpdateColor();
        }

        void UpdateColor()
        {
            rend.material.color = GetColor();
            Color emissionColor = new Color(emission, emission, emission);
            rend.material.SetColor("_EmissionColor", emissionColor);

        }

        void OnMouseDrag()
        {
            if (log) Debug.Log("OnMouseDrag: " + name);
            alpha = 0.5f + Mathf.PingPong(Time.time, 0.5f);
            UpdateColor();
        }

        void OnMouseOver()
        {
            if (log) Debug.Log("OnMouseOver: " + name);
            emission = 0.5f -Mathf.PingPong(Time.time* 0.5f, 0.5f);
            UpdateColor();
        }

        void OnMouseDown()
        {
            if (log) Debug.Log("OnMouseDown: " + name);
            color = GetRandomColor();
            UpdateColor();
        }

        void OnMouseUp()
        {
            if (log) Debug.Log("OnMouseUp: " + name);
            color = GetRandomColor();
            UpdateColor();
        }

        void OnMouseUpAsButton()
        {
            if (log) Debug.Log("OnMouseUpAsButton: " + name);
            alpha = 1;
            emission = 0;
            UpdateColor();
        }

        Color GetRandomColor()
        {
            return Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
    }
}
