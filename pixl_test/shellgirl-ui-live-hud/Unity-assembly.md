# 조수 웅덩이 메인 HUD · Unity 조합 시안

preview.png는 실제 502×891 플레이 화면에서 전투 영역을 가져와 UI를 얹은 합성입니다. 표시된 닉네임, 토글 상태, 보상 값과 하단 탭은 시안입니다. 기존 Unity 씬은 수정하지 않았습니다.

## parts 구성

| PNG | 용도 |
| --- | --- |
| plate_dark | 프로필, 재화 백판, 꺼짐 토글 |
| stage_card | 스테이지 백판, 둥지 바로가기 |
| button_coral | 보스 도전 |
| button_teal | 켜짐 토글 및 일반 행동 |
| progress_track | 진행바 트랙 |
| progress_fill | 진행바 채움 |

기존 초상화·골드·보석·알·보스 아이콘은 현재 ArtSource 자산을 재사용합니다. 미리보기 하단 탭 아이콘은 shellgirl-ui-kit의 아이콘입니다. 텍스트와 아이콘은 새 부품에 합쳐져 있지 않습니다.

## Unity 조합

- 기존 InGameScene은 720×1280 기준에 Match Width입니다. 미리보기 기준 좌표를 옮길 때 약 1.43배에서 시작합니다. 상단과 하단은 Safe Area 내에 고정합니다.
- Texture Type을 Sprite (2D and UI), Single로 가져오고 Alpha Is Transparency를 사용합니다. 생성본은 픽셀 스타일의 고해상도 래스터이므로 실사용 크기에서 Point와 Bilinear를 비교해 윤곽이 좋은 쪽을 선택합니다. 기존 픽셀 캐릭터에는 Point를 유지합니다.
- 제목·수치·토글 라벨은 TMP로 분리합니다. 채움은 트랙 위에 Image Type Filled, Method Horizontal, Origin Left로 배치합니다.
- 시안의 보상 묶음은 기존 반복 로그를 대체하는 제안이며, 하단 실제 퀵슬롯을 유지할 경우 하단 탭 위치를 기존 내비게이션과 맞춰 조정합니다.

제작 방식은 내장 이미지 생성입니다. 원본 시트와 프롬프트를 동봉했습니다.
