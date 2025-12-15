using UnityEngine;

public class ColorButton : MonoBehaviour
{
    public WallPainter wallPainter;
    public CubePainter cubePainter;
    public Color colorToSet;

    public void ChangeColor()
    {
            wallPainter.currentColor = colorToSet;
            cubePainter.cubeColor = colorToSet;
    }
}
