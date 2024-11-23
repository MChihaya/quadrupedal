using UnityEngine;
using TMPro;

public class DisplayName : MonoBehaviour
{
    public TextMeshProUGUI textComponent; // Text Mesh Pro UI コンポーネントへの参照

    public void SetName()
    {
        if (textComponent != null)
        {
            textComponent.text = this.name; // このオブジェクト（ロボット）の名前を表示
        }
    }
}
