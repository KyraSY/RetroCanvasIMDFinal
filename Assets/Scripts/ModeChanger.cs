using UnityEngine;

public class ModeChanger : MonoBehaviour
{
    public WallPainter wallPainter;
    public BubblePainter bubblePainter;
    public TreePainter treePainter;
    public CubePainter cubePainter;
    public BlobPainter blobPainter;
    public RainbowPainter rainbowPainter;
    public StickerPainter stickerPainter;
    public Eraser eraser;

    void Start()
    {
        DisableAll();
    }
        void DisableAll()
    {
        wallPainter.enabled = false;
        bubblePainter.enabled = false;
        treePainter.enabled = false;
        cubePainter.enabled = false;
        blobPainter.enabled = false;
        rainbowPainter.enabled = false;
        stickerPainter.enabled = false;
        eraser.enabled = false;
    }
    public void SetEraserMode()
{
    DisableAll();
    eraser.enabled = true;
}

    public void SetWallMode()
    {
        DisableAll();
        wallPainter.enabled = true;
    }

    public void SetBubbleMode()
    {
        DisableAll();
        bubblePainter.enabled = true;
    }

    public void SetTreeMode()
    {
        DisableAll();
        treePainter.enabled = true;
    }
    public void SetCubePainter()
    {
        DisableAll();
        cubePainter.enabled = true;
    }
    public void SetBlobPainter()
    {
        DisableAll();
        blobPainter.enabled = true;
    }
    public void SetRainbowPainter()
    {
        DisableAll();
        rainbowPainter.enabled = true;
    }
    public void SetStickerPainter()
    {
        DisableAll();
        stickerPainter.enabled = true;
    }

}
