using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 보상 아이콘이 곡선을 그리며 재화 칸(아이템은 하단 탭)으로 날아가고, 첫 아이콘이 도착할 때 지급 (kit_cocoa LAYOUT_reward.md '닫을 때·도착')
// 공용 보상 팝업과 보스 클리어 연출이 같이 씀
public static class UIGoodsFly
{
    const float FlyTime = 0.5f;
    const float FlyInterval = 0.05f;
    const int IconCount = 4;
    public const float TotalTime = FlyTime + FlyInterval * IconCount;

    // _template: 같은 캔버스의 꺼진 아이콘 Image (복제해서 씀)
    public static void Fly(MonoBehaviour _owner, Image _template, Vector3 _from, int _goodsId, double _value)
    {
        for (int n = 0; n < IconCount; n++)
            _owner.StartCoroutine(CoFly(_template, _from, _goodsId, _value, n * FlyInterval, n == 0));
    }

    static IEnumerator CoFly(Image _template, Vector3 _from, int _goodsId, double _value, float _delay, bool _isArrive)
    {
        yield return new WaitForSecondsRealtime(_delay);
        UIInGameScene ui = InGameManager.Instance.UIInGameScene;
        Image icon = Object.Instantiate(_template, _template.transform.parent);
        icon.sprite = ui.GetGoodsIcon(_goodsId);
        icon.gameObject.SetActive(true);
        // 시작·끝 사이에서 옆으로 휘는 곡선 (아이콘마다 휘는 쪽을 조금씩 다르게)
        Vector3 to = ui.GetGoodsTarget(_goodsId).position;
        Vector3 control = (_from + to) * 0.5f + Vector3.right * Random.Range(-1f, 1f) * Vector3.Distance(_from, to) * 0.3f;
        for (float t = 0f; t < FlyTime; t += Time.unscaledDeltaTime)
        {
            float r = t / FlyTime;
            icon.transform.position = Vector3.Lerp(Vector3.Lerp(_from, control, r), Vector3.Lerp(control, to, r), r);
            yield return null;
        }
        Object.Destroy(icon.gameObject);
        if (_isArrive)
        {
            InGameManager.Instance.AddGoods(_goodsId, _value);
            ui.PlayGoodsArrive(_goodsId, _value);
        }
    }
}
