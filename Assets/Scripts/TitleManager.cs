using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameObject tile1;
    [SerializeField] private GameObject tile2;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject clear;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            var sequence = DOTween.Sequence();
            sequence.Append(DOVirtual.DelayedCall(0f, () => {
                Vector3 pos1 = tile1.transform.position;
                Vector3 pos2 = tile2.transform.position;
                tile1.transform.position = pos2;
                tile2.transform.position = pos1;
            }));
            sequence.Append(DOVirtual.DelayedCall(0f, () => {
                clear.SetActive(true);
            }));
            //sequence.Append(title.transform.DOMoveY(20f, 3f).SetRelative().SetEase(Ease.InOutSine));
            sequence.Append(DOVirtual.DelayedCall(1f, () => {
                SceneManager.LoadScene("GameScene");
            }));
        }
    }
}
