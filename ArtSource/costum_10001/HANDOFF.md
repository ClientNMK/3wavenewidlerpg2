# HANDOFF — 메인 캐릭터 코스튬: 겨울 (costum_10001)

확정: 2026-11-28. **생성 비용 0** (AI로 새로 그리지 않음).

## 한 줄 요약
기본형(girl_base) 동작 프레임의 옷 색만 바꾸고 귀마개를 얹은 코스튬. 갈색 원피스·소매 → 빨강, 크림 프릴 → 흰색, 흰 귀마개 추가. 머리·얼굴·리본·단검·부츠·동작은 기본형 그대로.

## 폴더 구성
```
costum_10001/
├─ HANDOFF.md
├─ all_motions_x2.png               ← 11개 동작 한눈에 보기
├─ animations/<동작>/               ← girl_base/animations와 같은 구성
│  ├─ frames/frame_000~.png         ← 128×128, 바닥선 y=112, 중심 x=64, pivot (64,112)
│  ├─ <동작>_strip.png               ← 가로 스트립 (칸 128px)
│  ├─ <동작>_x1.gif                  ← 원본 크기, 투명
│  └─ <동작>_preview_x3.gif          ← 확인용 3배
└─ source/
   ├─ WinterCheap.cs                ← 만든 스크립트
   └─ earmuff_layer_p2.png          ← 귀마개 그림 (기본 포즈 p2 기준 96×96)
```

## 동작 (게임에서 쓰는 것만, FSM_MainCharacter.cs 기준)
idle 8, run 8, sprint 8, attack 11, attack2 11, attack3 11, hurt 9, dead 11, dash 9, vanish 14, backstab 11 (장수).
**프레임 수·속도·pivot·이펙트 타이밍은 기본형과 같다** → 기본형 엔진 설정(`girl_base/ENGINE_REQUEST.md`)을 그대로 쓰고 그림만 바꿔 끼우면 된다.
roll, appear는 게임에서 안 써서 만들지 않음.

## 만든 방법 (`source/WinterCheap.cs`)
1. 프레임마다 idle frame_000의 머리(30×60)를 찾아 이동량을 잼
2. 눈 아래부터: 어두운 갈색(밝기 0.13~0.37) → 빨강(색상 355°), 크림 프릴 색 → 흰색. 눈 둘레, 밝은 머리카락 1px 옆, 맨 아래 11줄(부츠)은 제외
3. 귀마개를 idle frame_000 기준 (19,23) + 머리 이동량에 얹음. 머리를 못 찾은 프레임(점수 3000 초과)은 생략
4. dead 8~10(누운 자세): `ApplyLying` — 부츠 오른쪽 갈색에서 이어진 옷만 칠함, 귀마개 없음
5. vanish: 겨울판 hurt frame_005를 `Dissolve.Make(…, 14, 3, 9, 4242)`로 다시 흩어뜨림 (기본형과 같은 설정)
6. backstab: `girl_base/animations/backstab/source/plain_frames`에 겨울판 적용 → `Dissolve.MaterializeInto(…, 4, …, 4242)`

귀마개 그림은 PixelLab 겨울 시안(edit_image job `6215825b-0467-40a4-a2ad-865b98828936` index 0)과 기본 포즈의 차이에서 오려냄.

## 알려진 점
- dead 8~10에는 귀마개가 없다 (쓰러지는 순간 사라져 보임). dead 9 볼에 흰 점 몇 개
- sprint 3·4·7은 뒤로 뻗은 부츠까지 빨갛다
- 머리카락에 닿은 옷 가장자리 1px은 갈색으로 남을 수 있음
- vanish 흩어지는 점 일부는 크림색
- 기본형 동작을 바꾸면 `WinterCheap.cs`로 다시 만들면 된다 (비용 0)

## 이름 규칙
`Chart/CostumeChart_Dev.xlsx`의 `Skin_Name`과 같게. 메인 캐릭터 코스튬은 Data_Id 10001~ (`costum_10001` = 이 겨울 코스튬), 짝이 되는 정령 후드 코스튬은 11001~ (`pets/pet_hood/costum_11001`). 차트 철자(`costum`)를 그대로 따른다.
