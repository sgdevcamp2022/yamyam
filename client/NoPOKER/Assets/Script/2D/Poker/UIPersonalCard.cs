using System.Collections;
using UnityEngine;

public class UIPersonalCard : MonoBehaviour
{
    Vector3 _targetPos;
    [SerializeField] bool _selfCard = false;
    void Start()
    {
        InitSetting();
    }

    void InitSetting()
    {
        Sound.Instance.PlayCardSound();

        _targetPos = transform.position;
        transform.localPosition = new Vector3(0, 0, 0);
        StartCoroutine(ShowDistributeCard());
    }

    IEnumerator ShowDistributeCard()
    {
        while ((transform.position - _targetPos).sqrMagnitude > 0.3)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPos, 0.3f);
            yield return new WaitForSeconds(0.1f);
        }
        PokerGameManager.Instance.UpDistributeNum();
    }
}
