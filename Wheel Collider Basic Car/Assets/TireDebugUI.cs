using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TireDebugUI : MonoBehaviour
{
    public TMP_Text valueText;
    public Image bgImage;
    public WheelCollider wheelCollider;
    public MeshRenderer wheelMesh;

    public enum WheelDebugType { FWD_SLIP, SDW_SLIP, ALL_SLIP, FORCE };
    public WheelDebugType wheelDebugType;

    WheelHit hit = new WheelHit();
    Material rubberMaterial;

    public bool changeColor = false;

    private Color lowValueColor = new Color(0.2f, 0.5f, 1f);
    private Color highValueColor = new Color(1f, 0.0f, 0.0f);

    private MaterialPropertyBlock matBlock;

    // Start is called before the first frame update
    void Start()
    {
        if (changeColor)
        {
            //rubberMaterial = wheelMesh.materials[0];
            matBlock = new MaterialPropertyBlock();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Color materialColor = Color.white;

        if (wheelCollider.GetGroundHit(out hit))
        {
            switch (wheelDebugType)
            {
                case WheelDebugType.FWD_SLIP:
                    valueText.text = hit.forwardSlip.ToString("F1");
                    materialColor = Color.Lerp(lowValueColor, highValueColor, Mathf.Abs(hit.forwardSlip));
                    bgImage.color = materialColor;
                    break;
                case WheelDebugType.SDW_SLIP:
                    valueText.text = hit.sidewaysSlip.ToString("F1");
                    materialColor = Color.Lerp(lowValueColor, highValueColor, Mathf.Abs(hit.forwardSlip));
                    bgImage.color = materialColor;
                    break;
                case WheelDebugType.ALL_SLIP:
                    float value = Mathf.Max(Mathf.Abs(hit.sidewaysSlip), Mathf.Abs(hit.forwardSlip));
                    valueText.text = value.ToString("F1");
                    materialColor = Color.Lerp(lowValueColor, highValueColor, value);
                    bgImage.color = materialColor;
                    break;
                case WheelDebugType.FORCE:
                    valueText.text = hit.force.ToString("F0");
                    materialColor = Color.Lerp(lowValueColor, highValueColor, Mathf.Abs(hit.force / 4000f));
                    bgImage.color = materialColor;
                    break;
            }

            if (changeColor)
            {
                matBlock.SetColor("_Color", materialColor);
                wheelMesh.SetPropertyBlock(matBlock);
            }
        } else
        {
            valueText.text = "0";
            if (changeColor)
            {
                matBlock.SetColor("_Color", Color.white);
                wheelMesh.SetPropertyBlock(matBlock);
            }
        }

        //slipValueText.text = wheelCollider.rpm.ToString("F0");
    }
}
