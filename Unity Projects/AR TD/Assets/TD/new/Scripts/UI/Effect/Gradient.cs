using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class Gradient : BaseMeshEffect {
  public enum Type {
    Vertical,
    Horizontal
  }
  [SerializeField]
  public Type GradientType = Type.Vertical;

  [SerializeField]
  [Range(-1.5f, 1.5f)]
  public float Offset = 0f;

  [SerializeField]
  private Color32 StartColor = Color.white;
  [SerializeField]
  private Color32 EndColor = Color.black;

  public override void ModifyMesh(VertexHelper vertexHelper) {
    if (!this.IsActive()) {
      return;
    }

    List<UIVertex> list = new List<UIVertex>();
    vertexHelper.GetUIVertexStream(list);

    ModifyVertices(list);

    vertexHelper.AddUIVertexTriangleStream(list);
  }

  public void ModifyVertices(List<UIVertex> vertexList) {
    if (!IsActive())
      return;

    int nCount = vertexList.Count;
    switch (GradientType) {
      case Type.Vertical:
        {
          float fBottomY = vertexList[0].position.y;
          float fTopY = vertexList[0].position.y;
          float fYPos = 0f;

          for (int i = nCount - 1; i >= 1; --i) {
            fYPos = vertexList[i].position.y;
            if (fYPos > fTopY)
              fTopY = fYPos;
            else if (fYPos < fBottomY)
              fBottomY = fYPos;
          }

          float fUIElementHeight = 1f / (fTopY - fBottomY);
          for (int i = nCount - 1; i >= 0; --i) {
            UIVertex uiVertex = vertexList[i];
            uiVertex.color = Color32.Lerp(EndColor, StartColor, (uiVertex.position.y - fBottomY) * fUIElementHeight - Offset);
            vertexList[i] = uiVertex;
          }
        }
        break;
      case Type.Horizontal:
        {
          float fLeftX = vertexList[0].position.x;
          float fRightX = vertexList[0].position.x;
          float fXPos = 0f;

          for (int i = nCount - 1; i >= 1; --i) {
            fXPos = vertexList[i].position.x;
            if (fXPos > fRightX)
              fRightX = fXPos;
            else if (fXPos < fLeftX)
              fLeftX = fXPos;
          }

          float fUIElementWidth = 1f / (fRightX - fLeftX);
          for (int i = nCount - 1; i >= 0; --i) {
            UIVertex uiVertex = vertexList[i];
            uiVertex.color = Color32.Lerp(StartColor, EndColor, (uiVertex.position.x - fLeftX) * fUIElementWidth - Offset);
            vertexList[i] = uiVertex;
          }
        }
        break;
      default:
        break;
    }
  }
}
