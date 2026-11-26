using System.Collections;
using UnityEngine;
using Spine.Unity;

// 메인 캐릭터 뒤쪽 위에 떠서 따라다니는 정령 펫. 포식 상태이상이 뜬 몬스터에게 날아가 삼키고 돌아옴
public class Hood : MonoBehaviour
{
    public SkeletonAnimation SkeletonAnim = null;   // 뜬 높이만큼 올라간 자식
    public MeshRenderer SkeletonRenderer = null;
    public SpriteRenderer DevourMark = null;        // 포식 대상 머리 위 상태이상 아이콘
    public SpriteRenderer MoveTrail = null;         // 멀리서 돌아오는 동안 망토 끝 반짝이 꼬리
    public Sprite[] MoveTrailSprites = null;

    const string SkinName = "hood_001";     // 후드 레벨 기능 전까지 고정 (HoodChart Resorce = 스킨 이름)
    const string DevourAnim = "devour";
    const float BehindOffset = 0.6f;        // 캐릭터 뒤쪽 거리
    const float FollowSharpness = 8f;
    const float FlyTime = 0.16f;            // devour frame_002 입 벌리기 전까지 몬스터 옆으로 날아감
    const float SwallowTime = 0.4f;         // devour frame_005 덮치는 순간
    const float BiteFlashEndTime = 0.72f;   // 덮침~우물우물 동안 번쩍임
    const float BiteFlashInterval = 0.05f;
    const float MonsterSideOffset = 0.3f;   // 몬스터 옆 거리
    static readonly Vector3 MouthOffset = new Vector3(2f, 24f, 0f) / 64f;   // 후드 128 캔버스 (66, 88)
    static readonly Vector3 MarkOffset = new Vector3(0f, 63f, 0f) / 64f;    // 몬스터 128 캔버스 머리 꼭대기 2~3px 위
    static readonly Vector3 BurstOffset = new Vector3(6f, -19f, 0f) / 64f;  // 포식 링: 후드 pivot에서 오른쪽 6, 아래 19
    static readonly Vector3 TrailOffset = new Vector3(-4f, 0f, 0f) / 64f;   // 이동 꼬리: 후드 pivot에서 왼쪽 4
    const string BurstKey = "devour_burst";
    const float TrailFrameTime = 1f / 30f;
    const float TrailFadeTime = 0.3f;
    const float TrailFarDistance = 1f;      // 따라갈 자리에서 이보다 멀면(칸) 돌아오는 중
    static readonly Color BiteFlashColor = new Color(0.3f, 0.9f, 0.6f);     // Tint Black 색: 어두운 얼굴을 청록으로 빛나게
    static readonly int BlackPID = Shader.PropertyToID("_Black");

    MaterialPropertyBlock materialPB = null;
    float trailTimer = 0f;
    float trailAlpha = 0f;

    MainCharacter MainCharacter { get { return InGameManager.Instance.MainCharacter; } }
    public bool IsDevouring { get { return SkeletonAnim.AnimationState.GetCurrent(0).Animation.Name == DevourAnim; } }
    float Flip { get { return SkeletonAnim.Skeleton.ScaleX; } }

    private void Start()
    {
        SkeletonAnim.Skeleton.SetSkin(SkinName);
        SkeletonAnim.Skeleton.SetSlotsToSetupPose();
        SkeletonAnim.AnimationState.SetAnimation(0, "idle", true);
        transform.position = FollowPosition;
    }

    Vector3 FollowPosition { get { return MainCharacter.transform.position + Vector3.left * MainCharacter.SkeletonAnim.Skeleton.ScaleX * BehindOffset; } }

    // 포식 중에는 코루틴이 위치를 정하고, 끝나면 캐릭터 뒤로 부드럽게 돌아옴
    private void Update()
    {
        if (!IsDevouring)
        {
            transform.position = Vector3.Lerp(transform.position, FollowPosition, 1f - Mathf.Exp(-FollowSharpness * Time.deltaTime));
            SkeletonAnim.Skeleton.ScaleX = MainCharacter.SkeletonAnim.Skeleton.ScaleX;
            SkeletonRenderer.sortingOrder = MainCharacter.SkeletonRenderer.sortingOrder - 1;
        }
        UpdateMoveTrail();
    }

    // 후드 기준점에서 _offset(px)만큼, 왼쪽을 볼 때는 x 반대
    Vector3 PivotPosition(Vector3 _offset)
    {
        _offset.x *= Flip;
        return SkeletonAnim.transform.position + _offset * transform.localScale.x;
    }

    // 포식 후처럼 멀리서 돌아오는 동안 30fps 반복, 가까워지면 TrailFadeTime에 걸쳐 사라짐
    void UpdateMoveTrail()
    {
        bool isReturning = !IsDevouring && Vector3.Distance(transform.position, FollowPosition) > TrailFarDistance;
        trailAlpha = Mathf.MoveTowards(trailAlpha, isReturning ? 1f : 0f, Time.deltaTime / TrailFadeTime);
        MoveTrail.enabled = trailAlpha > 0f;
        if (!MoveTrail.enabled)
            return;

        trailTimer += Time.deltaTime;
        MoveTrail.sprite = MoveTrailSprites[(int)(trailTimer / TrailFrameTime) % MoveTrailSprites.Length];
        MoveTrail.color = new Color(1f, 1f, 1f, trailAlpha);
        MoveTrail.flipX = Flip < 0f;
        MoveTrail.transform.position = PivotPosition(TrailOffset);
        MoveTrail.sortingOrder = SkeletonRenderer.sortingOrder - 1;
    }

    public void Devour(Monster _target)
    {
        StartCoroutine(CoDevour(_target));
    }

    // 몬스터 머리 위에 포식 아이콘을 띄우고 옆으로 날아가 입 벌리는 동안 몬스터를 입으로 끌어오며 줄이고,
    // 덮치는 순간 아이콘을 지우고 반짝 링과 함께 처치, 물어뜯는 동안 번쩍임
    IEnumerator CoDevour(Monster _target)
    {
        DevourMark.gameObject.SetActive(true);
        _target.ChangeState(State.Pause);
        float side = _target.transform.position.x < transform.position.x ? 1f : -1f;
        SkeletonAnim.Skeleton.ScaleX = -side;
        SkeletonAnim.AnimationState.SetAnimation(0, DevourAnim, false);
        SkeletonAnim.AnimationState.AddAnimation(0, "idle", true, 0f);

        Vector3 flyStart = transform.position;
        Vector3 flyEnd = _target.transform.position + Vector3.right * side * MonsterSideOffset;
        Vector3 targetStart = _target.transform.position;
        float targetScale = _target.transform.localScale.x;
        float timer = 0f;
        while (timer < SwallowTime)
        {
            transform.position = Vector3.Lerp(flyStart, flyEnd, Mathf.Clamp01(timer / FlyTime));
            float t = Mathf.Clamp01((timer - FlyTime) / (SwallowTime - FlyTime));
            _target.transform.position = Vector3.Lerp(targetStart, PivotPosition(MouthOffset), t * t);
            _target.transform.localScale = Vector3.one * targetScale * (1f - t * 0.7f);
            SkeletonRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
            _target.SkeletonRenderer.sortingOrder = SkeletonRenderer.sortingOrder + 1;
            DevourMark.transform.position = _target.transform.position + MarkOffset * _target.transform.localScale.x;
            DevourMark.sortingOrder = _target.SkeletonRenderer.sortingOrder + 1;
            timer += Time.deltaTime;
            yield return null;
        }
        DevourMark.gameObject.SetActive(false);

        // 그 사이 보스 도전 등으로 정리된 몬스터는 처치하지 않음
        if (InGameManager.Instance.Monsters.Contains(_target))
        {
            MainCharacter.PlayEffect(BurstKey, PivotPosition(BurstOffset), Flip, SkeletonRenderer.sortingOrder + 1);
            _target.transform.localScale = Vector3.zero;
            _target.CurHp = BigDouble.Zero;
            _target.ChangeState(State.Dead);
        }

        while (timer < BiteFlashEndTime)
        {
            SetBlackColor((int)(timer / BiteFlashInterval) % 2 == 0 ? BiteFlashColor : Color.black);
            timer += Time.deltaTime;
            yield return null;
        }
        SetBlackColor(Color.black);
    }

    void SetBlackColor(Color _color)
    {
        if (materialPB == null)
            materialPB = new MaterialPropertyBlock();

        materialPB.SetColor(BlackPID, _color);
        SkeletonRenderer.SetPropertyBlock(materialPB);
    }
}
