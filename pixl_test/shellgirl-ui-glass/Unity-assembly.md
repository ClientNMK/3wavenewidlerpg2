# 청록 유리 UI 시안

parts 폴더에 분리한 투명 PNG 6개가 있습니다.

- nameplate: 청록 유리 명패
- portrait_frame: 가운데가 투명한 자개 초상화 프레임. 뒤에 기존 초상화를 배치합니다.
- nest_backplate: 둥지 카드와 스테이지 정보용 남색 백판
- button_gold: 금빛 행동 버튼 백판
- progress_track / progress_fill: 진행바 트랙과 채움. 채움을 트랙 안쪽에 배치한 뒤 Image Type = Filled, Fill Method = Horizontal, Origin = Left로 연결합니다.

Unity에서는 Sprite (2D and UI), Alpha Is Transparency를 사용합니다. 새 UI는 HD 일러스트이므로 축소할 때 Bilinear를 적용해 보고, 기존 픽셀 캐릭터와 배경에는 Point 필터를 유지합니다. 중앙 장식까지 들어간 백판은 먼저 원본 비율에 가까운 Simple Image로 조합합니다. 같은 백판을 폭에 따라 크게 늘려야 한다면 중앙 장식과 좌우 장식을 추가로 분리합니다. 한글과 수치는 TMP로 얹습니다. preview.png는 두 화면을 합성한 예시입니다.

내장 이미지 생성을 사용했습니다. 레퍼런스 JPG는 재질과 광택의 참고 자료로 사용했습니다.
