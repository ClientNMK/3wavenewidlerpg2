# HANDOFF — 0차 후드 (깨진 알껍데기 후드)

확정일: 2026-11-26. **이 폴더에는 확정본만 둡니다.**

> **보류 중 (2026-11-26):** 기본형 모션을 먼저 다 정한 뒤 후드를 한 번에 추가하기로 함. 이 폴더는 2026-11-26 시점 기본형 기준이며, 기본형 확정 후 전부 다시 합성한다.

## 한 줄 요약
메인 캐릭터(소라게 후드 소녀)가 보스에게서 얻는 첫 후드. 크림색 알껍데기 윗부분, 얼굴 둘레는 깨진 들쭉날쭉한 가장자리, 주황 반점, 앞쪽 위에 큰 호박색 눈 하나.
**동작을 새로 생성하지 않고, 후드 그림 하나를 기본형(girl_base) 확정 동작 위에 씌워 만든다** (장비 레이어 방식).

## 폴더 구성
```
ArtSource/hood_0/
├─ HANDOFF.md
├─ layer/
│  ├─ hood0_layer.png        ← 후드만 잘라낸 레이어 (112×112, 투명)
│  ├─ hood0_design_se.png    ← 0차 디자인 원본 (south-east, 112×112). 머리 윤곽 기준으로 사용
│  └─ head_anchor.csv        ← 기본형 동작 프레임별 머리 위치 (anim,frame,head_x,head_y,score)
└─ animations/
   ├─ idle/    frames/frame_000~007.png, idle_strip.png, idle_x1.gif (0.12초), idle_preview_x3.gif
   ├─ run/     frames/frame_000~005.png, run_strip.png, run_x1.gif (0.08초), run_preview_x3.gif
   ├─ attack/  frames/frame_000~010.png, attack_strip.png, attack_x1.gif (0.07초), attack_preview_x3.gif
   ├─ hurt/    frames/frame_000~008.png, hurt_strip.png, hurt_x1.gif (0.08초), hurt_preview_x3.gif
   ├─ attack2/ frames/frame_000~010.png, attack2_strip.png, attack2_x1.gif (0.07초), attack2_preview_x3.gif  (2026-11-26)
   ├─ attack3/ frames/frame_000~010.png, attack3_strip.png, attack3_x1.gif (0.07초), attack3_preview_x3.gif  (2026-11-26, 기본형 attack3 = 점프 내려찍기 기준으로 다시 합성)
   └─ dead/    frames/frame_000~010.png, dead_strip.png, dead_x1.gif (0.1초), dead_preview_x3.gif  (2026-11-26, 마지막 자세 유지)
```
`layer/dead_anchor.csv`: dead 프레임별 머리 위치와 **후드 회전 각도**(0~10°, 쓰러지며 머리가 기울어서 후드도 회전 피벗 (54,33) 기준으로 돌림)
모든 프레임: 128×128, 바닥선 y=112, pivot (64,112). 기본형 동작과 프레임 수·타이밍이 같다.

## 엔진에서 직접 겹치는 경우
- 후드 레이어 왼쪽 위 좌표 = (head_x − 8, head_y − 8) (128 캔버스 기준, `head_anchor.csv`)
- 단, 기본형 뒷머리가 후드 밖으로 삐져나오므로 그대로 겹치면 지저분하다 → 지금은 **미리 합성·정리한 프레임(`animations/`)을 쓰는 것을 권장**

## 만든 방법 (다음 후드도 같은 방식)
1. `create_character_state`(기본 캐릭터 `91ce750a-…`, override 112×112)로 후드 쓴 디자인 생성 (15회) → `90228c57-36df-455c-8980-ee79e9b01721`
2. 디자인 south-east 그림에서 껍데기 색으로 flood fill + 외곽선 추가 → 후드만 잘라냄 (60행 아래 자름)
3. 기본형 south-east 회전 그림의 머리(18~75, 5~45)를 각 프레임에 템플릿 매칭 → 머리 위치. 기본 → 디자인 오프셋 (8, 8)
4. 정리 (모두 무료, 스크립트):
   - 디자인 머리 윤곽 밖 + 디자인 54행(턱) 위의 기본형 픽셀 삭제 (뒷머리)
   - 55~64행: 후드에 붙은 머리카락색 덩어리, 몸과 안 이어진 가는 머리카락 줄 삭제
   - attack: 디자인 윤곽 오른쪽(얼굴 앞, 36~64행)은 기본형 그대로 복원 → 단검 살림
   - 4방향 연결 기준 30px 미만 떨어진 조각 삭제
5. 후드 레이어를 그 위에 그림
스크립트: `pixl_test\assets\character\hood_0\work\tools\HoodTool.cs` (PowerShell `Add-Type`)

## 알려진 점
- 후드는 기울지 않고 위치만 따라감 → hurt 가운데 프레임에서 후드가 눈 쪽을 살짝 덮음
- attack 6~7번 프레임: 단검 끝 일부가 후드 뒤로 가려짐
- dead 9~10번(엎드린 자세): 후드를 회전해서 테두리가 살짝 거칠어짐
- **기본형 roll(구르기), dash는 아직 씌우지 않음** (몸이 회전하므로 후드도 회전 필요)
- **기본형 attack이 바뀌면 attack도 다시 합성해야 함** (2026-11-26 기준 attack = 11프레임 `attack_dynamic_lunge`. 픽셀랩 세션에서 보폭 줄인 버전 검토 중)
- 따로 생성해 본 0차 전용 동작(PixelLab group idle `f29e10b4-…`, attack `8d4b3b82-…`, hurt `7c502f8a-…`, run `e25c34ed-…`)은 쓰지 않음
