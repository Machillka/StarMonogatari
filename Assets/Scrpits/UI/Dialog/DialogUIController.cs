using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogUIController : MonoBehaviour
{
    public GameObject dialogue;
    public TextMeshProUGUI dialogText;
    public Image faceLeft, faceRight;
    public TextMeshProUGUI nameLeft, nameRight;
    public GameObject tipsBox;

    private void Awake()
    {
        tipsBox.SetActive(false);
    }
}
