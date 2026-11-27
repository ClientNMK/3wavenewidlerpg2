작업 전 현재 프로젝트의 기존 코드 구조, 하이어라키 구조 파악 우선 진행.
게임 흐름상 발생할 수 없는 예외 처리 작성 금지.
코드를 항상 기존 구조를 유지하면서 간결하게 작성.
불필요하게 함수, 필드 변수를 늘리지 말 것.
커밋 메시지에 Co-Authored-By 작성 금지.
요청 외 추가 작업 필요 시 질문 우선.
중첩 프리팹은 부모 오버라이드가 아니라 원본 프리팹에서 수정.
푸시, 커밋을 요청 시에만.
주석 내용은 반드시 알아야 하는 내용으로만 간결하게 작성.
다시 반복될 만한 실수일 때만 배운 점을 이 파일에 한 줄로 기록하고 중복은 정리.
- Spine JSON을 문자열 치환으로 고칠 때 같은 패턴이 스킨과 애니메이션에 여러 번 나오므로 한 곳만 바뀌는지 확인.
- 새 SkeletonAnimation을 코드로 만들 때 tintBlack = true 설정 (프로젝트 Spine 재질이 Tint Black 셰이더).
- 에디터 스크립트로 UI 오브젝트를 만들 때 layer = 5(UI) 설정 (UI 카메라가 UI 레이어만 그림, 안 하면 안 보임).
- DNFBitBitv2 SDF는 Dynamic이라 1024 아틀라스가 차면 새 한글이 빠짐 → Multi Atlas Textures 켜 둠 (끄지 말 것).
- 프로젝트는 Linear 색 공간 + Straight Alpha → Spine 텍스처 임포트 sRGB 켜기 (꺼지면 색이 밝게 바램).

# CLAUDE.md

Behavioral guidelines to reduce common LLM coding mistakes. Merge with project-specific instructions as needed.

**Tradeoff:** These guidelines bias toward caution over speed. For trivial tasks, use judgment.

## 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

Before implementing:
- State your assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them - don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what's confusing. Ask.

## 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.
- If you write 200 lines and it could be 50, rewrite it.

Ask yourself: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

## 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:
- Don't "improve" adjacent code, comments, or formatting.
- Don't refactor things that aren't broken.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code, mention it - don't delete it.

When your changes create orphans:
- Remove imports/variables/functions that YOUR changes made unused.
- Don't remove pre-existing dead code unless asked.

The test: Every changed line should trace directly to the user's request.

## 4. Goal-Driven Execution

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:
- "Add validation" → "Write tests for invalid inputs, then make them pass"
- "Fix the bug" → "Write a test that reproduces it, then make it pass"
- "Refactor X" → "Ensure tests pass before and after"

For multi-step tasks, state a brief plan:
```
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.

---

**These guidelines are working if:** fewer unnecessary changes in diffs, fewer rewrites due to overcomplication, and clarifying questions come before implementation rather than after mistakes.
